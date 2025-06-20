using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using UnityEngine.SceneManagement;

namespace DiscordFP2
{
    [BepInPlugin("techmuse_FP2DiscordRPC", "FP2DiscordRPC", "1.0.0")]
    [BepInDependency("000.kuborro.libraries.fp2.fp2lib")]
    public class Plugin : BaseUnityPlugin
    {
        public static string DisplayMissionObjectiveKey = "Display Full Mission Name";

        public static ConfigEntry<bool> DisplayMissionObjective;

        public static FP2Discord gameDiscord;

        public static ManualLogSource consoleLog;


        private void Awake()
        {
            consoleLog = Logger;
            consoleLog.LogInfo($"Plugin techmuse_FP2DiscordRPC loading...");

            DisplayMissionObjective = Config.Bind("General", DisplayMissionObjectiveKey, true);
            gameDiscord = new FP2Discord();

            consoleLog.LogInfo($"Plugin techmuse_FP2DiscordRPC 1.0.0 is now running!");
        }
        private void Update()
        {
    #if DEBUG
            Scene currentScene = SceneManager.GetActiveScene();
            consoleLog.LogInfo($"Active Character ID: {FPSaveManager.character}");
            consoleLog.LogInfo(FPAudio.GetCurrentMusic()); // 
            consoleLog.LogInfo($"Active Stage: {FPStage.currentStage.stageName}");
    #endif
            gameDiscord.UpdateDiscord();
        }
    }
}
