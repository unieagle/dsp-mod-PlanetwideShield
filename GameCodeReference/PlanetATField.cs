using System;
using System.IO;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class PlanetATField
{
	// Token: 0x06000D92 RID: 3474 RVA: 0x000CA65C File Offset: 0x000C885C
	public PlanetATField(PlanetData _planet)
	{
		this.planet = _planet;
		this.factory = this.planet.factory;
		this.defense = this.factory.defenseSystem;
		this.gameData = this.factory.gameData;
		this.spaceSector = this.gameData.spaceSector;
		this.physicsMeshVertsOriginal = Configs.combat.planetATFieldPhysicsVerts;
		this.physicsMeshIndicesOriginal = Configs.combat.planetATFieldPhysicsIndices;
		this.physicsMeshVerts = null;
		this.physicsMeshNorms = null;
		this.physicsArgs = null;
		this.physicsMeshBuffer = null;
		this.physicsArgsBuffer = null;
		this.fieldGenerateShader = Configs.combat.planetATFieldGenerateShader;
		this.physicsMesh = null;
		this.physicsMeshCollider = null;
		this.isEmpty = true;
		this.isSpherical = false;
		this.displayMesh = Configs.combat.planetATFieldDisplayMesh;
		this.displayMaterial = Object.Instantiate<Material>(Configs.combat.planetATFieldDisplayMat);
		this.energy = 0L;
		this.energyMax = 0L;
		this.energyMaxTarget = 0L;
		this.recoverCD = 0;
		this.rigidTime = 0;
		this.generatorCount = 0;
		this.generatorMatrix = null;
		this.generatorCountPresent = 0;
		this.generatorMatrixPresent = null;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x000CA79C File Offset: 0x000C899C
	public PlanetATField(PlanetData _planet, bool import)
	{
		this.planet = _planet;
		this.factory = this.planet.factory;
		this.defense = this.factory.defenseSystem;
		this.gameData = this.factory.gameData;
		this.spaceSector = this.gameData.spaceSector;
		this.physicsMeshVertsOriginal = Configs.combat.planetATFieldPhysicsVerts;
		this.physicsMeshIndicesOriginal = Configs.combat.planetATFieldPhysicsIndices;
		this.physicsMeshVerts = null;
		this.physicsMeshNorms = null;
		this.physicsArgs = null;
		this.physicsMeshBuffer = null;
		this.physicsArgsBuffer = null;
		this.fieldGenerateShader = Configs.combat.planetATFieldGenerateShader;
		this.physicsMesh = null;
		this.physicsMeshCollider = null;
		this.isEmpty = true;
		this.isSpherical = false;
		this.displayMesh = Configs.combat.planetATFieldDisplayMesh;
		this.displayMaterial = Object.Instantiate<Material>(Configs.combat.planetATFieldDisplayMat);
		this.energy = 0L;
		this.energyMax = 0L;
		this.energyMaxTarget = 0L;
		this.recoverCD = 0;
		this.rigidTime = 0;
		this.generatorCount = 0;
		this.generatorMatrix = null;
		this.generatorCountPresent = 0;
		this.generatorMatrixPresent = null;
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000CA8DC File Offset: 0x000C8ADC
	public void Free()
	{
		this.energy = 0L;
		this.energyMax = 0L;
		this.energyMaxTarget = 0L;
		this.recoverCD = 0;
		this.rigidTime = 0;
		this.generatorCount = 0;
		this.generatorMatrix = null;
		this.generatorCountPresent = 0;
		this.generatorMatrixPresent = null;
		this.gameData = null;
		this.spaceSector = null;
		this.planet = null;
		this.factory = null;
		this.spaceSector = null;
		this.defense = null;
		this.planetUPos = VectorLF3.zero;
		this.planetPrevUPos = VectorLF3.zero;
		this.planetURot = Quaternion.identity;
		this.planetPrevURot = Quaternion.identity;
		this.physicsMeshVertsOriginal = null;
		this.physicsMeshIndicesOriginal = null;
		this.physicsMeshVerts = null;
		this.physicsMeshNorms = null;
		this.physicsArgs = null;
		this.isEmpty = false;
		this.isSpherical = false;
		if (this.physicsMeshBuffer != null)
		{
			this.physicsMeshBuffer.Release();
			this.physicsMeshBuffer = null;
		}
		if (this.physicsArgsBuffer != null)
		{
			this.physicsArgsBuffer.Release();
			this.physicsArgsBuffer = null;
		}
		this.fieldGenerateShader = null;
		if (this.physicsMeshCollider != null)
		{
			Object.Destroy(this.physicsMeshCollider.gameObject);
			this.physicsMeshCollider = null;
		}
		if (this.physicsMesh != null)
		{
			Object.Destroy(this.physicsMesh);
			this.physicsMesh = null;
		}
		this.displayMesh = null;
		if (this.displayMaterial != null)
		{
			Object.Destroy(this.displayMaterial);
			this.displayMaterial = null;
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000D95 RID: 3477 RVA: 0x000CAA5C File Offset: 0x000C8C5C
	public double energyFill
	{
		get
		{
			double num = ((double)this.energyMax < 400000000.0) ? 400000000.0 : ((double)this.energyMax);
			return (double)this.energy / num;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000D96 RID: 3478 RVA: 0x000CAA97 File Offset: 0x000C8C97
	// (set) Token: 0x06000D97 RID: 3479 RVA: 0x000CAA9F File Offset: 0x000C8C9F
	public double fillDemandRatio { get; private set; }

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000D98 RID: 3480 RVA: 0x000CAAA8 File Offset: 0x000C8CA8
	public double globeFillRatio
	{
		get
		{
			return this.physicsArgs[0] / ((double)this.physicsMeshVertsOriginal.Length * 1000.0);
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000D99 RID: 3481 RVA: 0x000CAAC8 File Offset: 0x000C8CC8
	public double globeDefenceCoveryRatio
	{
		get
		{
			return this.physicsArgs[1] / ((double)this.physicsMeshVertsOriginal.Length * 1000.0);
		}
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x000CAAE8 File Offset: 0x000C8CE8
	public void GameTick(long tick, bool isActive)
	{
		bool flag = this.defense.fieldGenerators.count > 0;
		if (flag)
		{
			DeepProfiler.BeginSample(DPEntry.PlanetATField, -1, (long)this.planet.id);
		}
		if (isActive)
		{
			if (this.factory.index % 60 == (int)(tick % 60L))
			{
				this.SetPhysicsChangeSensitivity(85.8f);
			}
		}
		else if (this.factory.index % 180 == (int)(tick % 180L))
		{
			this.SetPhysicsChangeSensitivity(28.6f);
		}
		this.UpdatePlanetPose(tick);
		if (flag)
		{
			DeepProfiler.BeginSample(DPEntry.PlanetATFieldGenerator, -1, (long)this.planet.id);
		}
		this.UpdateGeneratorMatrix();
		this.CheckChangesForRecalculatePhysics();
		if (flag)
		{
			DeepProfiler.EndSample(-1, -2L);
		}
		this.UpdateColliderHotTicks();
		this.UpdateEnergy();
		this.ScrollFieldResistHistory();
		if (flag)
		{
			DeepProfiler.EndSample(-1, -2L);
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000CABBF File Offset: 0x000C8DBF
	public void UpdatePhysicsShape(bool updateGeneratorMatrix = false)
	{
		if (updateGeneratorMatrix)
		{
			this.UpdateGeneratorMatrix();
		}
		this.SetPhysicsChangeSensitivity(10000f);
		this.CheckChangesForRecalculatePhysics();
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000CABDC File Offset: 0x000C8DDC
	public void UpdateGeneratorMatrix()
	{
		DataPool<FieldGeneratorComponent> fieldGenerators = this.defense.fieldGenerators;
		int count = fieldGenerators.count;
		this.generatorCount = 0;
		if (this.generatorMatrix != null)
		{
			Array.Clear(this.generatorMatrix, 0, 80);
		}
		if (count > 0)
		{
			if (this.generatorMatrix == null)
			{
				this.generatorMatrix = new Vector4[80];
			}
			if (this.generatorMatrixPresent == null)
			{
				this.generatorMatrixPresent = new Vector4[80];
			}
			FieldGeneratorComponent[] buffer = fieldGenerators.buffer;
			int cursor = fieldGenerators.cursor;
			Array.Clear(this.generatorMatrix, 0, 80);
			for (int i = 1; i < cursor; i++)
			{
				if (buffer[i].id == i)
				{
					this.generatorMatrix[this.generatorCount] = buffer[i].holder;
					this.generatorCount++;
					if (this.generatorCount == 80)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x000CACB4 File Offset: 0x000C8EB4
	public void UpdateFillDemandRatio()
	{
		lock (this)
		{
			if (this.energyMax == 0L || this.isEmpty || this.recoverCD > 0)
			{
				this.fillDemandRatio = 0.0;
			}
			else
			{
				this.fillDemandRatio = 3.001 - this.energyFill * 3.0;
				this.fillDemandRatio = ((this.fillDemandRatio < 1.0) ? ((this.fillDemandRatio < 0.0) ? 0.0 : this.fillDemandRatio) : 1.0);
			}
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x000CAD78 File Offset: 0x000C8F78
	private void UpdateEnergy()
	{
		if (this.energyMaxTarget > 0L)
		{
			this.energyMax = (long)((double)this.energyMax * 0.99 + (double)this.energyMaxTarget * 0.01 + 0.9999);
		}
		else
		{
			this.energyMax = 0L;
		}
		if (this.energy > this.energyMax)
		{
			this.energy = this.energyMax;
		}
		if (this.recoverCD > 0)
		{
			this.recoverCD--;
		}
		if (this.rigidTime > 0)
		{
			this.rigidTime--;
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000CAE18 File Offset: 0x000C9018
	private void UpdatePlanetPose(long time)
	{
		this.planetUPos = this.planet.uPosition;
		this.planetURot = this.planet.runtimeRotation;
		if (this.energy > 0L)
		{
			this.planet.PredictUPose((double)(time - 1L) * 0.016666666666666666, out this.planetPrevUPos, out this.planetPrevURot);
			return;
		}
		this.planetPrevUPos = this.planetUPos;
		this.planetPrevURot = this.planetURot;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x000CAE90 File Offset: 0x000C9090
	public void BreakShield()
	{
		this.energy = 0L;
		if (this.rigidTime == 0)
		{
			this.recoverCD = 360;
		}
		this.ClearFieldResistHistory();
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x000CAEB4 File Offset: 0x000C90B4
	private void CheckChangesForRecalculatePhysics()
	{
		if (this.physicsChangeSensitivity > 0f)
		{
			float num = 0f;
			int num2 = (this.generatorCountPresent > this.generatorCount) ? this.generatorCountPresent : this.generatorCount;
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					float num3 = this.generatorMatrixPresent[i].w;
					float num4 = this.generatorMatrix[i].w;
					float num5 = num4 - num3;
					if (num5 != 0f)
					{
						if (num4 == 1f)
						{
							num4 += 2f;
						}
						else if (num4 == 0f)
						{
							num4 -= 2f;
						}
						if (num3 == 1f)
						{
							num3 += 2f;
						}
						else if (num3 == 0f)
						{
							num3 -= 2f;
						}
						num5 = num4 - num3;
					}
					float num6 = this.generatorMatrix[i].x - this.generatorMatrixPresent[i].x;
					float num7 = this.generatorMatrix[i].y - this.generatorMatrixPresent[i].y;
					float num8 = this.generatorMatrix[i].z - this.generatorMatrixPresent[i].z;
					num += ((num6 < 0f) ? (-num6) : num6);
					num += ((num7 < 0f) ? (-num7) : num7);
					num += ((num8 < 0f) ? (-num8) : num8);
					num += ((num5 < 0f) ? (-num5) : num5);
					if (num * this.physicsChangeSensitivity > 0.9999f)
					{
						this.RecalculatePhysicsShape();
						break;
					}
				}
			}
		}
		this.physicsChangeSensitivity = 0f;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x000CB070 File Offset: 0x000C9270
	private void CreatePhysics()
	{
		int num = this.physicsMeshVertsOriginal.Length;
		if (this.physicsMeshVerts == null)
		{
			this.physicsMeshVerts = new Vector3[num];
		}
		if (this.physicsMeshNorms == null)
		{
			this.physicsMeshNorms = new Vector3[num];
		}
		if (this.physicsArgs == null)
		{
			this.physicsArgs = new uint[10];
		}
		if (this.physicsMeshBuffer == null)
		{
			this.physicsMeshBuffer = new ComputeBuffer(num * 2, 12, ComputeBufferType.Default);
		}
		if (this.physicsArgsBuffer == null)
		{
			this.physicsArgsBuffer = new ComputeBuffer(10, 4, ComputeBufferType.Default);
		}
		if (this.physicsMesh == null)
		{
			this.physicsMesh = new Mesh();
			this.physicsMesh.vertices = this.physicsMeshVertsOriginal;
			this.physicsMesh.normals = this.physicsMeshNorms;
			this.physicsMesh.triangles = this.physicsMeshIndicesOriginal;
			float num2 = (this.planet.realRadius + 60.8f + 0.2f) * 2f * 0.05f;
			this.physicsMesh.bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(num2, num2, num2));
		}
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x000CB194 File Offset: 0x000C9394
	private void ClearPhysics()
	{
		this.physicsMeshVerts = null;
		this.physicsMeshNorms = null;
		this.physicsArgs = null;
		if (this.physicsMeshBuffer != null)
		{
			this.physicsMeshBuffer.Release();
			this.physicsMeshBuffer = null;
		}
		if (this.physicsArgsBuffer != null)
		{
			this.physicsArgsBuffer.Release();
			this.physicsArgsBuffer = null;
		}
		if (this.physicsMesh != null)
		{
			Object.Destroy(this.physicsMesh);
			this.physicsMesh = null;
		}
		if (this.physicsMeshCollider != null)
		{
			Object.Destroy(this.physicsMeshCollider.gameObject);
			this.physicsMeshCollider = null;
		}
		this.isEmpty = true;
		this.isSpherical = false;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x000CB23D File Offset: 0x000C943D
	public void SetPhysicsChangeSensitivity(float sens)
	{
		if (sens > this.physicsChangeSensitivity)
		{
			this.physicsChangeSensitivity = sens;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000DA5 RID: 3493 RVA: 0x000CB24F File Offset: 0x000C944F
	// (set) Token: 0x06000DA6 RID: 3494 RVA: 0x000CB257 File Offset: 0x000C9457
	public int colliderHotTicks { get; private set; }

	// Token: 0x06000DA7 RID: 3495 RVA: 0x000CB260 File Offset: 0x000C9460
	private void RecalculatePhysicsShape()
	{
		HighStopwatch highStopwatch = new HighStopwatch();
		highStopwatch.Begin();
		if (this.generatorCount > 0)
		{
			this.CreatePhysics();
			int num = this.physicsMeshVertsOriginal.Length;
			Array.Copy(this.physicsMeshVertsOriginal, this.physicsMeshVerts, num);
			Array.Clear(this.physicsArgs, 0, this.physicsArgs.Length);
			this.physicsMeshBuffer.SetData(this.physicsMeshVerts, 0, 0, num);
			this.physicsArgsBuffer.SetData(this.physicsArgs, 0, 0, this.physicsArgs.Length);
			this.fieldGenerateShader.SetInt("_VertexCount", num);
			this.fieldGenerateShader.SetBuffer(0, "_VertexBuffer", this.physicsMeshBuffer);
			this.fieldGenerateShader.SetBuffer(0, "_ArgsBuffer", this.physicsArgsBuffer);
			this.fieldGenerateShader.SetFloat("_PlanetRadius", this.planet.realRadius);
			this.fieldGenerateShader.SetFloat("_PhysicsScale", 0.05f);
			this.fieldGenerateShader.SetFloat("_FieldAltitude", 60.8f);
			this.fieldGenerateShader.SetFloat("_K", this.displayMaterial.GetFloat("_K"));
			this.fieldGenerateShader.SetInt("_GeneratorCount", this.generatorCount);
			this.fieldGenerateShader.SetVectorArray("_GeneratorMatrix", this.generatorMatrix);
			this.fieldGenerateShader.Dispatch(0, (num - 1) / 256 + 1, 1, 1);
			this.physicsMeshBuffer.GetData(this.physicsMeshVerts, 0, 0, num);
			this.physicsMeshBuffer.GetData(this.physicsMeshNorms, 0, num, num);
			this.physicsArgsBuffer.GetData(this.physicsArgs, 0, 0, this.physicsArgs.Length);
			if (this.physicsArgs[0] > 0U && this.isEmpty)
			{
				this.rigidTime = 600;
			}
			if (this.physicsArgs[0] == 0U)
			{
				this.isEmpty = true;
				this.isSpherical = false;
			}
			else if ((ulong)this.physicsArgs[0] >= (ulong)((long)(num * 1000)))
			{
				this.isEmpty = false;
				this.isSpherical = true;
			}
			else
			{
				this.isEmpty = false;
				this.isSpherical = false;
			}
			double num2 = this.physicsArgs[0] / ((double)num * 1000.0);
			this.energyMaxTarget = (long)(1200000000000.0 * num2 + 0.5);
			if (this.isEmpty || this.isSpherical)
			{
				this.CloseColliderObject();
			}
			else if (this.colliderHotTicks > 0)
			{
				this.OpenColliderObject();
			}
			else
			{
				this.CloseColliderObject();
			}
			if (this.physicsMeshCollider != null)
			{
				this.physicsMeshCollider.sharedMesh = null;
			}
			if (!this.isEmpty)
			{
				this.physicsMesh.vertices = this.physicsMeshVerts;
				this.physicsMesh.normals = this.physicsMeshNorms;
			}
			if (this.physicsMeshCollider != null)
			{
				this.physicsMeshCollider.sharedMesh = this.physicsMesh;
			}
			PlanetATField.recalculatePhysCounter++;
		}
		else
		{
			this.ClearPhysics();
			this.energyMaxTarget = 0L;
		}
		this.generatorCountPresent = this.generatorCount;
		if (this.generatorMatrix != null)
		{
			if (this.generatorMatrixPresent == null)
			{
				this.generatorMatrixPresent = new Vector4[80];
			}
			Array.Copy(this.generatorMatrix, this.generatorMatrixPresent, 80);
		}
		else
		{
			this.generatorMatrixPresent = null;
		}
		PlanetATField.recalculateCounter++;
		PlanetATField.recalculateTimeCost += highStopwatch.duration;
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x000CB5C2 File Offset: 0x000C97C2
	// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x000CB5C9 File Offset: 0x000C97C9
	public static int recalculateCounter { get; private set; }

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000DAA RID: 3498 RVA: 0x000CB5D1 File Offset: 0x000C97D1
	// (set) Token: 0x06000DAB RID: 3499 RVA: 0x000CB5D8 File Offset: 0x000C97D8
	public static int recalculatePhysCounter { get; private set; }

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000CB5E0 File Offset: 0x000C97E0
	// (set) Token: 0x06000DAD RID: 3501 RVA: 0x000CB5E7 File Offset: 0x000C97E7
	public static double recalculateTimeCost { get; private set; }

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000DAE RID: 3502 RVA: 0x000CB5EF File Offset: 0x000C97EF
	public static double recalculateAverageTimeCost
	{
		get
		{
			return PlanetATField.recalculateTimeCost / ((double)PlanetATField.recalculatePhysCounter + 1E-06);
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x000CB607 File Offset: 0x000C9807
	public void SetColliderHot(int hotTicks = 900)
	{
		this.colliderHotTicks = ((this.colliderHotTicks > hotTicks) ? this.colliderHotTicks : hotTicks);
		if (hotTicks > 0)
		{
			if (this.isEmpty || this.isSpherical)
			{
				this.CloseColliderObject();
				return;
			}
			this.OpenColliderObject();
		}
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x000CB644 File Offset: 0x000C9844
	private void UpdateColliderHotTicks()
	{
		if (this.colliderHotTicks > 0)
		{
			int colliderHotTicks = this.colliderHotTicks;
			this.colliderHotTicks = colliderHotTicks - 1;
			if (this.colliderHotTicks <= 0)
			{
				this.CloseColliderObject();
			}
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x000CB67C File Offset: 0x000C987C
	private static Transform colliderGroup
	{
		get
		{
			if (PlanetATField._colliderGroup == null)
			{
				PlanetATField._colliderGroup = new GameObject("ATF Colliders")
				{
					transform = 
					{
						position = Vector3.zero,
						rotation = Quaternion.identity
					}
				}.transform;
			}
			return PlanetATField._colliderGroup;
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x000CB6D0 File Offset: 0x000C98D0
	private void OpenColliderObject()
	{
		if (this.physicsMeshCollider == null)
		{
			GameObject gameObject = new GameObject(this.planet.displayName);
			gameObject.transform.SetParent(PlanetATField.colliderGroup, false);
			gameObject.transform.SetIdentity();
			gameObject.layer = 1;
			gameObject.SetActive(true);
			this.physicsMeshCollider = gameObject.AddComponent<MeshCollider>();
			this.physicsMeshCollider.convex = false;
			this.physicsMeshCollider.cookingOptions = MeshColliderCookingOptions.None;
			if (this.physicsMesh != null)
			{
				this.physicsMeshCollider.sharedMesh = this.physicsMesh;
			}
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x000CB76C File Offset: 0x000C996C
	private void CloseColliderObject()
	{
		if (this.physicsMeshCollider != null)
		{
			if (PlanetATField.colliderGroup.childCount <= 1)
			{
				Object.Destroy(PlanetATField.colliderGroup.gameObject);
			}
			else
			{
				Object.Destroy(this.physicsMeshCollider.gameObject);
			}
			this.physicsMeshCollider = null;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000DB4 RID: 3508 RVA: 0x000CB7BC File Offset: 0x000C99BC
	// (set) Token: 0x06000DB5 RID: 3509 RVA: 0x000CB7C4 File Offset: 0x000C99C4
	public bool rayTesting { get; private set; }

	// Token: 0x06000DB6 RID: 3510 RVA: 0x000CB7CD File Offset: 0x000C99CD
	public void BeginRayTests(int hotTicks = 900)
	{
		if (!this.rayTesting)
		{
			this.SetColliderHot(hotTicks);
			this.rayTesting = true;
			this.rayTestCursor = 1;
			if (this.rayTests == null)
			{
				this.rayTests = new PlanetATField.RayTest[32];
			}
		}
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000CB804 File Offset: 0x000C9A04
	public void AddRaycastTest(ERayTestSkillType skillType, int skillId, ref Vector3 beginPos, ref Vector3 endPos, float extendDist)
	{
		if (!this.rayTesting)
		{
			return;
		}
		if (this.rayTestCursor == this.rayTests.Length)
		{
			PlanetATField.RayTest[] array = this.rayTests;
			this.rayTests = new PlanetATField.RayTest[this.rayTests.Length * 2];
			Array.Copy(array, this.rayTests, array.Length);
		}
		ref PlanetATField.RayTest ptr = ref this.rayTests[this.rayTestCursor];
		ptr.skillType = skillType;
		ptr.skillId = skillId;
		ptr.testState = 0;
		ptr.beginPos = beginPos;
		ptr.endPos = endPos;
		ptr.endNormal.x = endPos.x - beginPos.x;
		ptr.endNormal.y = endPos.y - beginPos.y;
		ptr.endNormal.z = endPos.z - beginPos.z;
		ptr.dist = ptr.endNormal.magnitude;
		if (ptr.dist > 0f)
		{
			ref PlanetATField.RayTest ptr2 = ref ptr;
			ptr2.endNormal.x = ptr2.endNormal.x / ptr.dist;
			ref PlanetATField.RayTest ptr3 = ref ptr;
			ptr3.endNormal.y = ptr3.endNormal.y / ptr.dist;
			ref PlanetATField.RayTest ptr4 = ref ptr;
			ptr4.endNormal.z = ptr4.endNormal.z / ptr.dist;
		}
		ptr.dist += extendDist;
		this.rayTestCursor++;
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000CB958 File Offset: 0x000C9B58
	public void DoRayTests()
	{
		if (!this.rayTesting)
		{
			return;
		}
		if (this.rayTestCursor <= 1)
		{
			return;
		}
		if (this.isEmpty)
		{
			return;
		}
		if (this.energy <= 0L)
		{
			return;
		}
		this.SetColliderHot(1);
		float num = this.planet.realRadius - 2f;
		num *= num;
		float radius = this.planet.realRadius + 60.8f;
		for (int i = 1; i < this.rayTestCursor; i++)
		{
			ref PlanetATField.RayTest ptr = ref this.rayTests[i];
			float dist = ptr.dist;
			Vector3 endNormal = ptr.endNormal;
			if (this.isSpherical)
			{
				ptr.testState = 0;
				if (Phys.RayCastSphere(ref ptr.beginPos, ref endNormal, dist, ref this._zeroVector, radius, out this._rchcpu))
				{
					ptr.testState = 3;
					ptr.dist = this._rchcpu.dist;
					ptr.endPos = this._rchcpu.point;
					ptr.endNormal = this._rchcpu.normal;
				}
			}
			else
			{
				ptr.testState = 0;
				if (this.physicsMeshCollider.Raycast(new Ray(ptr.beginPos * 0.05f, endNormal), out this._rch, dist * 0.05f))
				{
					ptr.endPos = this._rch.point;
					ref PlanetATField.RayTest ptr2 = ref ptr;
					ptr2.endPos.x = ptr2.endPos.x * 20f;
					ref PlanetATField.RayTest ptr3 = ref ptr;
					ptr3.endPos.y = ptr3.endPos.y * 20f;
					ref PlanetATField.RayTest ptr4 = ref ptr;
					ptr4.endPos.z = ptr4.endPos.z * 20f;
					if (ptr.endPos.sqrMagnitude >= num)
					{
						ptr.testState = 2;
						ptr.dist = this._rch.distance * 20f;
						ptr.endNormal = this._rch.normal;
					}
					else
					{
						ptr.testState = 1;
						ptr.dist = this._rch.distance * 20f;
						ptr.endNormal = this._rch.normal;
					}
				}
			}
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x000CBB58 File Offset: 0x000C9D58
	public void EndRayTests()
	{
		int astroId = this.factory.planet.astroId;
		for (int i = 1; i < this.rayTestCursor; i++)
		{
			if (this.rayTests[i].skillId > 0 && this.rayTests[i].testState > 0)
			{
				this.factory.skillSystem.PlanetATFieldRaycastHitCallback(astroId, i, ref this.rayTests[i]);
			}
		}
		this.rayTesting = false;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x000CBBD4 File Offset: 0x000C9DD4
	public int TakeDamage(int damage, long energyRate)
	{
		long num = this.energy / energyRate;
		if (num > (long)damage)
		{
			num = (long)damage;
		}
		this.energy -= (long)damage * energyRate;
		this.MarkFieldResist(damage, energyRate);
		if (this.energy < energyRate)
		{
			this.BreakShield();
		}
		damage -= (int)num;
		return damage;
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x000CBC24 File Offset: 0x000C9E24
	public bool TestRelayCondition(Vector3 relayPos)
	{
		if (this.rayTesting)
		{
			return false;
		}
		if (this.isEmpty || this.energy <= 0L)
		{
			return true;
		}
		if (this.isSpherical)
		{
			return false;
		}
		this.BeginRayTests(90);
		Vector3 vector = relayPos * 1.4f;
		Vector3 vector2 = relayPos * 0.5f;
		this.AddRaycastTest(ERayTestSkillType.None, 0, ref vector, ref vector2, 0f);
		this.DoRayTests();
		this.EndRayTests();
		return this.rayTests[1].testState <= 1;
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000DBC RID: 3516 RVA: 0x000CBCB0 File Offset: 0x000C9EB0
	public long atFieldResistCurrent
	{
		get
		{
			if (this.atFieldResistHistory == null)
			{
				return 0L;
			}
			long num = 0L;
			int num2 = this.atFieldResistHistory.Length;
			for (int i = 0; i < num2; i++)
			{
				num += this.atFieldResistHistory[i];
			}
			return num;
		}
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000CBCEC File Offset: 0x000C9EEC
	public void MarkFieldResist(int damage, long rate)
	{
		if (this.atFieldResistHistory == null)
		{
			this.atFieldResistHistory = new long[60];
		}
		long num = (long)damage * rate;
		this.atFieldResistHistory[0] += num;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000CBD24 File Offset: 0x000C9F24
	private void ScrollFieldResistHistory()
	{
		if (this.energy == 0L)
		{
			return;
		}
		if (this.atFieldResistHistory != null)
		{
			Array.Copy(this.atFieldResistHistory, 0, this.atFieldResistHistory, 1, this.atFieldResistHistory.Length - 1);
			this.atFieldResistHistory[0] = 0L;
		}
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x000CBD5E File Offset: 0x000C9F5E
	public void ClearFieldResistHistory()
	{
		this.atFieldResistHistory = null;
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x000CBD68 File Offset: 0x000C9F68
	public void RenderDisplay()
	{
		if (this.generatorCount > 0 && this.generatorMatrix != null)
		{
			float realRadius = this.planet.realRadius;
			float value = (float)(((float)this.energy > 0.001f) ? 0 : 1);
			this.displayMaterial.SetFloat("_IsBroken", value);
			this.displayMaterial.SetFloat("_PlanetRadius", realRadius);
			this.displayMaterial.SetFloat("_FieldAltitude", 60.8f);
			this.displayMaterial.SetInt("_GeneratorCount", this.generatorCount);
			this.displayMaterial.SetVectorArray("_GeneratorMatrix", this.generatorMatrix);
			float num = (realRadius + 60.8f + 0.2f) * 2f;
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.m00 = num;
			matrix.m11 = num;
			matrix.m22 = num;
			matrix.m33 = 1f;
			Graphics.DrawMesh(this.displayMesh, matrix, this.displayMaterial, 1);
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x000CBE64 File Offset: 0x000CA064
	public void Export(BinaryWriter w)
	{
		w.Write(0);
		w.Write(this.energy);
		w.Write(this.energyMax);
		w.Write(this.energyMaxTarget);
		w.Write(this.recoverCD);
		w.Write(this.rigidTime);
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000CBEB4 File Offset: 0x000CA0B4
	public void Import(BinaryReader r)
	{
		r.ReadInt32();
		this.energy = r.ReadInt64();
		this.energyMax = r.ReadInt64();
		this.energyMaxTarget = r.ReadInt64();
		this.recoverCD = r.ReadInt32();
		this.rigidTime = r.ReadInt32();
	}

	// Token: 0x04000EB6 RID: 3766
	public long energy;

	// Token: 0x04000EB7 RID: 3767
	public long energyMax;

	// Token: 0x04000EB8 RID: 3768
	public long energyMaxTarget;

	// Token: 0x04000EB9 RID: 3769
	public int recoverCD;

	// Token: 0x04000EBA RID: 3770
	public int rigidTime;

	// Token: 0x04000EBC RID: 3772
	public int generatorCount;

	// Token: 0x04000EBD RID: 3773
	public Vector4[] generatorMatrix;

	// Token: 0x04000EBE RID: 3774
	public int generatorCountPresent;

	// Token: 0x04000EBF RID: 3775
	public Vector4[] generatorMatrixPresent;

	// Token: 0x04000EC0 RID: 3776
	public const int MAX_GENERATOR_COUNT = 80;

	// Token: 0x04000EC1 RID: 3777
	public Vector3[] physicsMeshVertsOriginal;

	// Token: 0x04000EC2 RID: 3778
	public int[] physicsMeshIndicesOriginal;

	// Token: 0x04000EC3 RID: 3779
	public Vector3[] physicsMeshVerts;

	// Token: 0x04000EC4 RID: 3780
	public Vector3[] physicsMeshNorms;

	// Token: 0x04000EC5 RID: 3781
	public uint[] physicsArgs;

	// Token: 0x04000EC6 RID: 3782
	public ComputeBuffer physicsMeshBuffer;

	// Token: 0x04000EC7 RID: 3783
	public ComputeBuffer physicsArgsBuffer;

	// Token: 0x04000EC8 RID: 3784
	public ComputeShader fieldGenerateShader;

	// Token: 0x04000EC9 RID: 3785
	public Mesh physicsMesh;

	// Token: 0x04000ECA RID: 3786
	public MeshCollider physicsMeshCollider;

	// Token: 0x04000ECB RID: 3787
	public bool isEmpty;

	// Token: 0x04000ECC RID: 3788
	public bool isSpherical;

	// Token: 0x04000ECD RID: 3789
	public Mesh displayMesh;

	// Token: 0x04000ECE RID: 3790
	public Material displayMaterial;

	// Token: 0x04000ECF RID: 3791
	public GameData gameData;

	// Token: 0x04000ED0 RID: 3792
	public SpaceSector spaceSector;

	// Token: 0x04000ED1 RID: 3793
	public PlanetData planet;

	// Token: 0x04000ED2 RID: 3794
	public PlanetFactory factory;

	// Token: 0x04000ED3 RID: 3795
	public DefenseSystem defense;

	// Token: 0x04000ED4 RID: 3796
	public VectorLF3 planetUPos;

	// Token: 0x04000ED5 RID: 3797
	public VectorLF3 planetPrevUPos;

	// Token: 0x04000ED6 RID: 3798
	public Quaternion planetURot;

	// Token: 0x04000ED7 RID: 3799
	public Quaternion planetPrevURot;

	// Token: 0x04000ED8 RID: 3800
	private float physicsChangeSensitivity;

	// Token: 0x04000EDA RID: 3802
	private const float kPhysicsScale = 0.05f;

	// Token: 0x04000EDB RID: 3803
	private const float kInvPhysicsScale = 20f;

	// Token: 0x04000EDF RID: 3807
	private static Transform _colliderGroup;

	// Token: 0x04000EE0 RID: 3808
	public int rayTestCursor;

	// Token: 0x04000EE1 RID: 3809
	public PlanetATField.RayTest[] rayTests;

	// Token: 0x04000EE3 RID: 3811
	private RaycastHit _rch;

	// Token: 0x04000EE4 RID: 3812
	private RCHCPU _rchcpu;

	// Token: 0x04000EE5 RID: 3813
	private Vector3 _zeroVector = Vector3.zero;

	// Token: 0x04000EE6 RID: 3814
	public long atFieldRechargeCurrent;

	// Token: 0x04000EE7 RID: 3815
	public long[] atFieldResistHistory;

	// Token: 0x04000EE8 RID: 3816
	public const float kFieldAltitude = 60.8f;

	// Token: 0x02000C06 RID: 3078
	public struct RayTest
	{
		// Token: 0x04007C6B RID: 31851
		public ERayTestSkillType skillType;

		// Token: 0x04007C6C RID: 31852
		public int skillId;

		// Token: 0x04007C6D RID: 31853
		public int testState;

		// Token: 0x04007C6E RID: 31854
		public Vector3 beginPos;

		// Token: 0x04007C6F RID: 31855
		public Vector3 endPos;

		// Token: 0x04007C70 RID: 31856
		public float dist;

		// Token: 0x04007C71 RID: 31857
		public Vector3 endNormal;
	}
}
