using HarmonyLib;
using UnityEngine;

namespace PlanetwideShield
{
    [HarmonyPatch(typeof(PlanetATField), nameof(PlanetATField.UpdateGeneratorMatrix))]
    public static class Patch_PlanetATField_UpdateGeneratorMatrix
    {
        private const int MaxGeneratorCount = 80;
        private const int VirtualPointCount = 48;
        /// <summary>两塔中心距离小于此值时视为“挨着”并启用全球护盾（与扫描日志中的“距离”同单位）。</summary>
        private const float AdjacentDistanceThreshold = 40f;

        private static Vector4[]? _cachedVirtualPoints;
        private static float _cachedRadius = -1f;

        [HarmonyPostfix]
        public static void Postfix(PlanetATField __instance)
        {
            if (__instance?.planet == null) return;
            if (__instance.defense.fieldGenerators.count < 2) return;
            if (!AreTwoGeneratorsWithinThreshold(__instance)) return;

            var radius = __instance.planet.realRadius;
            if (_cachedVirtualPoints == null || Mathf.Abs(_cachedRadius - radius) > 0.01f)
            {
                _cachedRadius = radius;
                _cachedVirtualPoints = BuildVirtualPoints(radius, VirtualPointCount);
            }

            var n = Mathf.Min(_cachedVirtualPoints!.Length, MaxGeneratorCount);
            if (__instance.generatorMatrix == null || __instance.generatorMatrix.Length < n)
                __instance.generatorMatrix = new Vector4[MaxGeneratorCount];

            for (var i = 0; i < n; i++)
                __instance.generatorMatrix[i] = _cachedVirtualPoints[i];
            for (var i = n; i < MaxGeneratorCount; i++)
                __instance.generatorMatrix[i] = default;

            __instance.generatorCount = n;
            __instance.SetPhysicsChangeSensitivity(10000f);
        }

        /// <summary>两塔中心距离平方小于此值时视为“挨着”（= AdjacentDistanceThreshold²）。</summary>
        private static bool AreTwoGeneratorsWithinThreshold(PlanetATField field)
        {
            var pool = field.defense.fieldGenerators;
            var buffer = pool.buffer;
            var cursor = pool.cursor;
            var thresholdSq = AdjacentDistanceThreshold * AdjacentDistanceThreshold;

            for (var i = 1; i < cursor; i++)
            {
                if (buffer[i].id != i) continue;
                var a = buffer[i].holder;
                var posA = new Vector3(a.x, a.y, a.z);
                for (var j = i + 1; j < cursor; j++)
                {
                    if (buffer[j].id != j) continue;
                    var b = buffer[j].holder;
                    var posB = new Vector3(b.x, b.y, b.z);
                    if ((posA - posB).sqrMagnitude < thresholdSq)
                        return true;
                }
            }
            return false;
        }

        private static Vector4[] BuildVirtualPoints(float planetRadius, int count)
        {
            var arr = new Vector4[count];
            for (var i = 0; i < count; i++)
            {
                var t = (float)(i + 1) / (count + 1);
                var inclination = Mathf.Acos(1f - 2f * t);
                var azimuth = (Mathf.PI * 2f * (1f + Mathf.Sqrt(5f)) / 2f) * i;
                var x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                var y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                var z = Mathf.Cos(inclination);
                var pos = new Vector3(x, y, z) * planetRadius;
                arr[i] = new Vector4(pos.x, pos.y, pos.z, 1f);
            }
            return arr;
        }
    }
}
