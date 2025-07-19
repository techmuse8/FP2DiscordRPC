using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private void Awake()
        {
            consoleLog = Logger;
            consoleLog.LogInfo($"Plugin FP2DiscordRPC loading...");


            // Before we do anything, we need to load the Discord Game SDK DLL
            string pluginDir = Path.GetDirectoryName(Info.Location);
            string dllPath = Path.Combine(pluginDir, "discord_game_sdk.dll");

            if (!File.Exists(dllPath))
            {
                consoleLog.LogError($"discord_game_sdk.dll not found at: {dllPath}!");
                return;
            }

            IntPtr handle = LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                consoleLog.LogError($"Failed to load discord_game_sdk.dll! Win32 Error Code: {err}");
                return;
            }

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
