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
        private Discord.Discord _discord = new Discord.Discord(FP2Constants.applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord); //TODO, put in constants?
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
                //status.currentStage = Stage.NONE;

            //Debug.LogWarning($"Active Player: {currentPlayerID}");
            Scene currentScene = SceneManager.GetActiveScene();
           // char currentPlayer = FPSaveManager.character;
           if (FPSaveManager.character >= (FPCharacterID)5) // Account for any additional custom character slots
            {
                status.smallImage = "unkchar"; // Replace with unknown character icon
                status.smallText = PlayerHandler.currentCharacter.Name; // Current custom character name attribute from FP2Lib
            }
            switch (FPSaveManager.character)
            {
                case 0: // Lilac
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
                status.details = FPStage.currentStage.stageName; // TODO: Add the images and crap
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
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
            
        }

        public void UpdateDiscord()
        {
            FetchGameInformation();
            UpdateGameActivity();

            _discord.RunCallbacks();
        }
    }
}
