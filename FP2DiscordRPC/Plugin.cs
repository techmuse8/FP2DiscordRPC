using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiscordFP2
{
    [BepInPlugin("techmuse_FP2DiscordRPC", "FP2DiscordRPC", "1.0.0")]
    [BepInDependency("000.kuborro.libraries.fp2.fp2lib")]
    public class Plugin : BaseUnityPlugin
    {
        
        public static FP2Discord gameDiscord;
        public static ManualLogSource consoleLog;

        private void Awake()
        {
            consoleLog = Logger;
            consoleLog.LogInfo($"Plugin FP2DiscordRPC loading...");
            gameDiscord = new FP2Discord();
            consoleLog.LogInfo($"Plugin FP2DiscordRPC 1.0.0 is now running!");
        }
        private void Update()
        { 
            gameDiscord.UpdateDiscord();
        }

        private void OnApplicationQuit()
        {
            gameDiscord.DeinitDiscordRPC();
        }
    }
}
