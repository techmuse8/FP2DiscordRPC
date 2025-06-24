using Discord;
using FP2Lib.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.PlayerLoop;

namespace DiscordFP2
{
    public class FP2Discord
    {
        private Discord.Discord _discord = new Discord.Discord(FP2Constants.applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        public FP2Status status = new FP2Status();
        private FPCharacterID currentPlayerID;

        public void FetchGameInformation()
        {

                status.details = string.Empty;
                status.state = string.Empty;
                status.largeImage = "main";
                status.largeText = "Freedom Planet 2";

                status.smallImage = string.Empty;
                status.smallText = string.Empty;
                
                //status.startTime = 0;

           Scene currentScene = SceneManager.GetActiveScene();

           if (FPSaveManager.character >= (FPCharacterID)5) // Account for any additional custom character slots
            {
                status.smallImage = "unkchar";
                status.smallText = PlayerHandler.currentCharacter.Name; // Current custom character name attribute from FP2Lib
            }
            switch (FPSaveManager.character)
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

            if (FPStage.currentStage.stageName != "")
            {
                status.details = FPStage.currentStage.stageName;
                status.stageImageName = FPStage.currentStage.stageName;
                GetStageImage();
                status.largeImage = status.stageImageName;
            }
            // In the main menu
            //Debug.LogWarning($"Active Scene: {currentScene.name}");
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

            // On the level select (in Classic Mode)
            else if (currentScene.name == "ClassicMenu")
            {
                status.details = "Level Select (Classic Mode)";
            }

              
            {
               
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
                        Start = status.startTime,
                        End =  status.endTime,
                    }
            };

            _discord.GetActivityManager().UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                {
                    Debug.LogWarning("Failed connecting to Discord!");
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
                status.details = "Weapon's Core";  // TODO: Find out how to get the string of Weapon's Core dynamically
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

        public void DeinitDiscordRPC()
        {
            _discord?.Dispose();
        }
    }
}
