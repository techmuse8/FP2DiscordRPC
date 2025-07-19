using Discord;
using FP2Lib.Player;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiscordFP2
{
    public class FP2Discord
    {
        private Discord.Discord _discord = new Discord.Discord(FP2Constants.applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        public FP2Status status = new FP2Status();
        private long startTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds; // Done this way because ToUnixTimeSeconds() doesn't exist in dotnet35

        public void FetchGameInformation()
        {

                status.details = string.Empty;
                status.state = string.Empty;
                status.largeImage = "main";
                status.largeText = "Freedom Planet 2";

                status.smallImage = string.Empty;
                status.smallText = string.Empty;
                

           Scene currentScene = SceneManager.GetActiveScene();

           if (FPSaveManager.character >= (FPCharacterID)5) // Account for any additional custom character slots
            {
                status.smallImage = "unkchar";
                status.smallText = PlayerHandler.currentCharacter.Name; // Current custom character name attribute from FP2Lib
            }
            switch (FPSaveManager.character) // Displays the character you are currently playing as
            {
                case (FPCharacterID)0: // Lilac
                    status.smallImage = "lilacicon"; 
                    status.smallText = "Lilac the Dragon";
                    break;

                case (FPCharacterID)1: // Carol
                    status.smallImage = "carolicon";
                    status.smallText = "Carol the Wildcat";
                    break;

                case (FPCharacterID)3: // Milla, we skip an ID because 2 is Carol's bike form and that is already accounted for
                    status.smallImage = "millaicon"; 
                    status.smallText = "Milla the Hound";
                    break;

                case (FPCharacterID)4: // Neera
                    status.smallImage = "neeraicon"; 
                    status.smallText = "Neera the Frost Knight";
                    break;
            }

            // If the string for the stage title isn't blank then we're in a stage!
            if (FPStage.currentStage.stageName != "") 
            {
                status.details = FPStage.currentStage.stageName;
                status.stageImageName = FPStage.currentStage.stageName;
                GetStageImage();
                status.largeImage = status.stageImageName;
                status.largeText = FPStage.currentStage.stageName;
                status.state = "In a stage";
                CheckIfPaused();
            }

            // In the main menu
            if (currentScene.name == "MainMenu")
            {
                status.details = "Main Menu";
            }

            // On the world map (in Adventure Mode)
            else if (currentScene.name == "AdventureMenu")
            {
                status.details = "World Map (Adventure Mode)";
            }

            // On loading screens
            else if (currentScene.name == "Loading")
            {
                status.details = "Loading...";
            }

            // On the world map (in Classic Mode)
            else if (currentScene.name == "ClassicMenu")
            {
                status.details = "World Map (Classic Mode)";
            }

            // On the Battlesphere Area menu
            else if (currentScene.name == "ArenaMenu")
            {
                status.details = "Battlesphere Arena Menu";
                status.largeImage = "battlespherelobby";
            }

            // In a Battlesphere Home Run contest
            else if (currentScene.name == "Battlesphere_Homerun")
            {
                status.details = "The Battlesphere (Home Run)";
                status.largeImage = "battlespherehomerun";
            }

            // In a cutscene
            if (FPStage.eventIsActive)
            {
                status.state = "Watching a cutscene";
            }
        
        }

        public void UpdateGameActivity()
        {
            var activity = new Discord.Activity
            {
                Details = status.details,
                State = status.state,
                Assets =
                    {
                        LargeImage = status.largeImage,
                        LargeText = status.largeText,
                        SmallImage = status.smallImage,
                        SmallText = status.smallText,
                    },
                Timestamps =
                    {
                        Start = startTime,
                    }
            };

            _discord.GetActivityManager().UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                {
                    //Debug.LogWarning("Failed connecting to Discord!");
                    _discord?.Dispose();

                }
            });
            
        }

        public void UpdateDiscord()
        {
            FetchGameInformation();
            UpdateGameActivity();

            _discord.RunCallbacks(); // TODO: try to reconnect if Discord can't be detected
        }
        public void GetStageImage()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Bakunawa5") // Check if the active stage is Weapon's Core, as an edge case scenario for how this stage works
            {
                status.stageImageName = "weaponscore";
                status.details = "Weapon's Core";
                status.largeText = "Weapon's Core";
            }
            else
            {
                // I'm honestly kind of a genius for coming up with this solution so that I don't have to manually set the stage icon asset names, which results in a more modular solution
                // For example, "Dragon Valley" becomes "dragonvalley", which is the name of the appropriate Discord RPC asset
                status.stageImageName = FPStage.currentStage.stageName.ToLower();
                status.stageImageName = status.stageImageName.Replace(" ", "");
    #if DEBUG
                Debug.LogWarning($"Lowercase stage img name: {status.stageImageName}");
    #endif
            }
        }

        public void CheckIfPaused()
        {
            switch (FPStage.state)
            {
                case (FPStageState)2: // Check if the game is paused
                    status.state = status.state + " (Paused)";
                    break;

                case (FPStageState)1:
                    status.state = "In a stage";
                    break;
            }
        }

        public void DeinitDiscordRPC() // Properly cleans up Discord RPC when the game is closed
        {
            _discord?.Dispose();
        }
    }
}
