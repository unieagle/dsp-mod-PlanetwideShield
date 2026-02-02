using UnityEngine;

namespace PlanetwideShield
{
    /// <summary>
    /// 每 10 秒扫描当前星球上的护盾塔，若有多个则输出两两距离到日志，用于测试游戏的最小放置距离。
    /// </summary>
    public class ShieldDistanceScanner : MonoBehaviour
    {
        private const float Interval = 10f;
        private float _nextScanTime;

        private void Start()
        {
            _nextScanTime = Time.realtimeSinceStartup + Interval;
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup < _nextScanTime) return;
            _nextScanTime = Time.realtimeSinceStartup + Interval;

            var planet = GameMain.localPlanet;
            if (planet?.factory?.defenseSystem == null) return;

            var defense = planet.factory.defenseSystem;
            var pool = defense.fieldGenerators;
            if (pool.count < 2) return;

            var buffer = pool.buffer;
            var cursor = pool.cursor;
            var planetName = planet.displayName ?? planet.name;
            var planetId = planet.id;

            Plugin.Log?.LogInfo($"[全球护盾] 护盾距离扫描 | 星球: {planetName} (id={planetId}), 护盾塔数: {pool.count}");

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
                    var sqrDist = (posA - posB).sqrMagnitude;
                    var dist = Mathf.Sqrt(sqrDist);
                    Plugin.Log?.LogInfo($"[全球护盾]   塔 #{i} ↔ #{j} | 距离 = {dist:F3} (平方 = {sqrDist:F3})");
                }
            }
        }
    }
}
