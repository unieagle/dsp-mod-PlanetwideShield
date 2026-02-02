using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006AC RID: 1708
public class UIPlanetShieldDetail : ManualBehaviour
{
	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x060042E5 RID: 17125 RVA: 0x003671C0 File Offset: 0x003653C0
	// (set) Token: 0x060042E6 RID: 17126 RVA: 0x003671C8 File Offset: 0x003653C8
	public PlanetData planet
	{
		get
		{
			return this._planet;
		}
		set
		{
			if (value != this._planet)
			{
				this._planet = value;
				this.menuButton.SetInfo(UIGenericMenuButton.EInfoType.PlanetShieldUI, (this._planet == null) ? 0 : this._planet.astroId, 0, 0, 0, 0);
			}
		}
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x00367200 File Offset: 0x00365400
	protected override bool _OnInit()
	{
		this.energySB = new StringBuilder("         J", 10);
		this.energySB_1 = new StringBuilder("         J", 10);
		this.damageSB = new StringBuilder("         ", 10);
		this.damageResistSB = new StringBuilder("         W", 10);
		this.maxRecoverRateSB = new StringBuilder("          ", 10);
		return true;
	}

	// Token: 0x060042E8 RID: 17128 RVA: 0x00367268 File Offset: 0x00365468
	protected override void _OnFree()
	{
		base._OnFree();
	}

	// Token: 0x060042E9 RID: 17129 RVA: 0x00367270 File Offset: 0x00365470
	protected override void _OnRegEvent()
	{
		this.shieldButton.onClick += this.OnShieldButtonClicked;
	}

	// Token: 0x060042EA RID: 17130 RVA: 0x00367289 File Offset: 0x00365489
	protected override void _OnUnregEvent()
	{
		this.shieldButton.onClick -= this.OnShieldButtonClicked;
	}

	// Token: 0x060042EB RID: 17131 RVA: 0x003672A2 File Offset: 0x003654A2
	protected override void _OnCreate()
	{
		this.uiGame = UIRoot.instance.uiGame;
	}

	// Token: 0x060042EC RID: 17132 RVA: 0x003672B4 File Offset: 0x003654B4
	protected override void _OnDestroy()
	{
		this.uiGame = null;
	}

	// Token: 0x060042ED RID: 17133 RVA: 0x003672BD File Offset: 0x003654BD
	protected override void _OnOpen()
	{
		if (this.planet == null)
		{
			base._Close();
			return;
		}
	}

	// Token: 0x060042EE RID: 17134 RVA: 0x003672D0 File Offset: 0x003654D0
	protected override void _OnClose()
	{
		if (!this.generatorWindow.active && this.factoryModel != null)
		{
			this.factoryModel.drawATFieldShape = false;
		}
		this.factory = null;
		this.field = null;
		this.powerSystem = null;
		this.defenseSystem = null;
		this.factoryModel = null;
		this.planet = null;
		this.dfMonitorConflict = false;
	}

	// Token: 0x060042EF RID: 17135 RVA: 0x00367335 File Offset: 0x00365535
	protected override void _OnUpdate()
	{
		this.Refresh();
	}

	// Token: 0x060042F0 RID: 17136 RVA: 0x00367340 File Offset: 0x00365540
	public void Refresh()
	{
		long energy = this.field.energy;
		long energyMax = this.field.energyMax;
		if (UIGame.viewMode == EViewMode.Globe && this.planet.factoryLoaded)
		{
			this.isShowShield = this.factoryModel.drawATFieldShape;
			this.visibleCheck.enabled = this.isShowShield;
		}
		if (this.planet.factory.defenseSystem.fieldGenerators.count == 0)
		{
			base._Close();
			return;
		}
		bool flag = this.uiGame.dfMonitor.active && this.uiGame.dfMonitor.contentRect.gameObject.activeInHierarchy && (this.uiGame.dfMonitor.contentRect.sizeDelta.y > 30f || this.uiGame.dfMonitor.displayModeComboBox.gameObject.activeSelf);
		bool flag2 = this.uiGame.mechaLab.active && this.uiGame.mechaLab.miniLabGroup.anchoredPosition.x > -20f;
		float num = flag ? (this.uiGame.dfMonitor.contentRect.sizeDelta.y + (this.uiGame.starmap.isFullOpened ? 70f : 22f)) : 0f;
		float num2 = flag2 ? (this.uiGame.mechaLab.miniLabBg.sizeDelta.y - this.uiGame.mechaLab.miniLabGroup.anchoredPosition.y - 146f) : 0f;
		float x;
		float y;
		if (flag && flag2)
		{
			x = -26f;
			y = -num2;
			this.dfMonitorConflict = false;
		}
		else if (!flag && flag2)
		{
			x = -26f;
			y = -num2;
			this.dfMonitorConflict = false;
		}
		else if (flag && !flag2)
		{
			if (num > 300f)
			{
				x = -26f;
				y = 0f;
				this.dfMonitorConflict = true;
			}
			else
			{
				x = 20f;
				y = -num;
				this.dfMonitorConflict = false;
			}
		}
		else
		{
			x = 0f;
			y = 0f;
			this.dfMonitorConflict = false;
		}
		if ((this.rectTrans.anchoredPosition - new Vector2(x, y)).sqrMagnitude > 1f)
		{
			this.rectTrans.anchoredPosition = new Vector2(x, y);
		}
		double num3 = this.field.physicsArgs[0] / ((double)this.field.physicsMeshVertsOriginal.Length * 1000.0);
		double num4 = this.field.physicsArgs[1] / ((double)this.field.physicsMeshVertsOriginal.Length * 1000.0);
		this.value_1.text = num3.ToString("0.00%") + " / " + num4.ToString("0.00%");
		StringBuilderUtility.WriteKMG(this.energySB, 8, energy, true, '\u2009', ' ');
		StringBuilderUtility.WriteKMG(this.energySB_1, 8, energyMax, true, '\u2009', ' ');
		if (energy > 0L)
		{
			this.value_2.text = string.Format("有能量的护盾值格式".Translate(), this.energySB.ToString(), this.energySB_1.ToString().TrimStart());
		}
		else
		{
			this.value_2.text = string.Format("无能量的护盾值格式".Translate(), this.energySB.ToString(), this.energySB_1.ToString().TrimStart());
		}
		long num5 = energy / GameMain.history.planetaryATFieldEnergyRate;
		num5 /= 100L;
		if (num5 > 99999L)
		{
			StringBuilderUtility.WriteKMG(this.damageSB, 8, num5, true, '\u2009', ' ');
			this.value_4.text = string.Format("可抵御伤害单位".Translate(), this.damageSB.ToString());
		}
		else
		{
			this.value_4.text = string.Format("可抵御伤害单位".Translate(), num5.ToString());
		}
		PowerNetwork[] netPool = this.factory.powerSystem.netPool;
		int netCursor = this.factory.powerSystem.netCursor;
		long num6 = 0L;
		long num7 = 0L;
		long num8 = 0L;
		for (int i = 1; i < netCursor; i++)
		{
			PowerNetwork powerNetwork = netPool[i];
			if (powerNetwork != null && powerNetwork.id == i && powerNetwork.energyExport > 0L)
			{
				num6 += powerNetwork.energyCapacity * 60L;
				num7 += powerNetwork.energyRequired * 60L;
				num8 += powerNetwork.energyExchangedOutputTotal * 60L;
			}
		}
		long num9 = num6 - num7 + num8;
		if (num9 < 0L)
		{
			num9 = 0L;
		}
		num9 /= GameMain.history.planetaryATFieldEnergyRate;
		num9 /= 100L;
		StringBuilderUtility.WriteKMG(this.maxRecoverRateSB, 8, num9, true, '\u2009', ' ');
		this.value_5.text = "+ " + string.Format("最大回复速度单位".Translate(), this.maxRecoverRateSB.ToString().Trim());
		this.value_5.color = ((num9 > 0L) ? this.powerFullTextColor : this.powerOutTextColor);
		this.value_1.color = ((num3 > 0.0) ? this.powerInColor : this.powerOutTextColor);
		this.value_4.color = ((num5 > 0L) ? this.powerInColor : this.powerOutTextColor);
	}

	// Token: 0x060042F1 RID: 17137 RVA: 0x003678BC File Offset: 0x00365ABC
	public void DetermineVisible()
	{
		if (this.planet == null || this.planet.type == EPlanetType.Gas || this.planet.factory == null || this.planet.factory.defenseSystem == null || this.planet.factory.defenseSystem.fieldGenerators.count == 0 || VFInput.inCombatScreenGUI)
		{
			base._Close();
			return;
		}
		this.factory = this.planet.factory;
		this.field = this.factory.planetATField;
		this.powerSystem = this.factory.powerSystem;
		this.defenseSystem = this.factory.defenseSystem;
		this.factoryModel = this.factory.planet.factoryModel;
		if (this.factoryModel != null)
		{
			this.isShowShield = this.factoryModel.drawATFieldShape;
		}
		this.checkGroup.SetActive(this.factoryModel != null && UIGame.viewMode != EViewMode.Starmap);
		base._Open();
		this.Refresh();
	}

	// Token: 0x060042F2 RID: 17138 RVA: 0x003679D4 File Offset: 0x00365BD4
	private void OnShieldButtonClicked(int obj)
	{
		if (this.factory == null)
		{
			return;
		}
		this.visibleCheck.enabled = !this.visibleCheck.enabled;
		this.isShowShield = !this.isShowShield;
		this.factoryModel.drawATFieldShape = this.isShowShield;
	}

	// Token: 0x04004E83 RID: 20099
	private UIGame uiGame;

	// Token: 0x04004E84 RID: 20100
	[SerializeField]
	public RectTransform rectTrans;

	// Token: 0x04004E85 RID: 20101
	[SerializeField]
	public UIGenericMenuButton menuButton;

	// Token: 0x04004E86 RID: 20102
	[SerializeField]
	public Text value_1;

	// Token: 0x04004E87 RID: 20103
	[SerializeField]
	public Text value_2;

	// Token: 0x04004E88 RID: 20104
	[SerializeField]
	public Text value_4;

	// Token: 0x04004E89 RID: 20105
	[SerializeField]
	public Text value_5;

	// Token: 0x04004E8A RID: 20106
	[SerializeField]
	public GameObject checkGroup;

	// Token: 0x04004E8B RID: 20107
	[SerializeField]
	public UIButton shieldButton;

	// Token: 0x04004E8C RID: 20108
	[SerializeField]
	public Image visibleCheck;

	// Token: 0x04004E8D RID: 20109
	[SerializeField]
	public UIFieldGeneratorWindow generatorWindow;

	// Token: 0x04004E8E RID: 20110
	[SerializeField]
	public Color powerInColor;

	// Token: 0x04004E8F RID: 20111
	[SerializeField]
	public Color powerOutTextColor;

	// Token: 0x04004E90 RID: 20112
	[SerializeField]
	public Color powerFullTextColor;

	// Token: 0x04004E91 RID: 20113
	[SerializeField]
	public Color powerLowTextColor;

	// Token: 0x04004E92 RID: 20114
	public PlanetFactory factory;

	// Token: 0x04004E93 RID: 20115
	public PlanetATField field;

	// Token: 0x04004E94 RID: 20116
	public PowerSystem powerSystem;

	// Token: 0x04004E95 RID: 20117
	public DefenseSystem defenseSystem;

	// Token: 0x04004E96 RID: 20118
	public FactoryModel factoryModel;

	// Token: 0x04004E97 RID: 20119
	public bool isShowShield;

	// Token: 0x04004E98 RID: 20120
	[NonSerialized]
	public bool dfMonitorConflict;

	// Token: 0x04004E99 RID: 20121
	private PlanetData _planet;

	// Token: 0x04004E9A RID: 20122
	private StringBuilder energySB;

	// Token: 0x04004E9B RID: 20123
	private StringBuilder energySB_1;

	// Token: 0x04004E9C RID: 20124
	private StringBuilder damageSB;

	// Token: 0x04004E9D RID: 20125
	private StringBuilder damageResistSB;

	// Token: 0x04004E9E RID: 20126
	private StringBuilder maxRecoverRateSB;
}
