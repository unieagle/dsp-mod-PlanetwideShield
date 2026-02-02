using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PlanetwideShield
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? Log;
        public static ConfigEntry<bool> EnableDebugLog = null!;

        /// <summary>
        /// 调试日志开关：由配置文件控制
        /// </summary>
        public static bool DebugLog() => EnableDebugLog?.Value ?? false;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"[{PluginInfo.PLUGIN_NAME}] 正在加载... (GUID: {PluginInfo.PLUGIN_GUID})");

            EnableDebugLog = Config.Bind(
                "General",
                "EnableDebugLog",
                false,
                "为 true 时在日志中输出详细的调试信息，用于排查问题。正常使用时建议设置为 false。");

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Plugin).Assembly);

            var scannerGo = new GameObject("PlanetwideShield_Scanner");
            DontDestroyOnLoad(scannerGo);
            scannerGo.AddComponent<ShieldDistanceScanner>();

            Log?.LogInfo($"[{PluginInfo.PLUGIN_NAME}] ✅ 加载完成！（每 10 秒扫描当前星球护盾距离并输出到日志）");
        }
    }
}
