using Eagle_Island;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TAS.Save;

namespace TAS
{
    public static class SaveManager
    {
        private static string filePath = Directory.GetCurrentDirectory() + "\\TASSaves.xml";
        private static Saves saves = new Saves();
        private static bool savesInitialized = false;
        private static int saveSlot = 1;
        private static Status status= Status.Loaded;
        public static void Save()
        {
            if (!savesInitialized)
            {
                InitializeSaveStates();
            }

            SaveState state = SaveState.Create(saveSlot);
            saves.SetState(saveSlot, state);
            WriteSaves();
            InterfaceManager.SetNotification("Slot " + saveSlot + " Saved");
        }

        /// <summary>
        /// Main update loop for loading saves. Called every frame after somebody hits the load button
        /// until the Status is set back to Loaded
        /// </summary>
        /// <returns>True if the save is still being loaded, or false if we're done</returns>
        public static bool Load()
        {
            if (!savesInitialized)
            {
                InitializeSaveStates();
            }

            SaveState state = saves.GetState(saveSlot);
            if (state != null)
            {
                switch (status)
                {
                    case Status.Loaded:
                        Map.Fade = 1f;
                        if (state.SavedInHub)
                        {
                            Map.LoadHub(Map.Outcomes.ExitLevel);
                        }
                        else
                        {
                            Map.Level = state.Level;
                            Map.RestartLevelFromMenu();
                        }
                        status = Status.Loading;
                        break;
                    case Status.Loading:
                        if (Map.Ready && Map.StartRoomReady)
                        {
                            foreach (Room room in Map.Rooms)
                            {
                                if (room.CoordVec.Equals(state.RoomCoords))
                                {
                                    LoadRoom(state, room);
                                    LoadCollectables(state, room);
                                    LoadTimer();
                                    LoadDestroyedEnemies(state, room);
                                    Upgrades.Load(state.Upgrades);
                                    status = Status.Loaded;
                                }
                            }
                        }
                        break;
                }
            }

            return status != Status.Loaded;
        }

        public static void IncreaseSlot()
        {
            saveSlot++;
            NotifySlot();
        }

        public static void DecreaseSlot()
        {
            if (saveSlot > 1)
            {
                saveSlot--;
            }
            NotifySlot();
        }

        public static void NotifySlot()
        {
            if (!savesInitialized)
            {
                InitializeSaveStates();
            }

            SaveState state = saves.GetState(saveSlot);
            string level = "";
            if (state != null)
            {
                if (state.SavedInHub)
                {
                    level = "Hub ";
                }
                level += Text.Get(Map.LevelsStrings_withLevelPrepended[state.Level]);
            }
            else
            {
                level = "Empty";
            }
            InterfaceManager.SetNotification("Slot " + saveSlot + " - " + level);
        }

        /// <summary>
        /// Handle resetting the timer and saving the last split
        /// </summary>
        private static void LoadTimer()
        {
            // Set the last split timer
            Raven.Set(15, Raven.Frame, (int)Map.Level);
            // Reset timer to 1 frame, setting to 0 deletes the save (sometimes?)
            Raven.SetStuff(0, 1);
        }

        /// <summary>
        /// Set the correct number of destroyed enemies for Quinn.
        /// </summary>
        private static void LoadDestroyedEnemies(SaveState state, Room room)
        {
            WorldObject destroyedEnemy = new BlankObject(Vector2.Zero, room);
            destroyedEnemy.SetEnemy(WorldObject.EnemyTypes.Plant, "ForestShroom");
            for (int e = 0; e < state.EnemiesKilled; e++)
            {
                Map.EnemiesDefeated.Add(destroyedEnemy);
            }
        }

        /// <summary>
        /// Set coins, seeds, gems, health, and perks for Quinn
        /// </summary>
        private static void LoadCollectables(SaveState state, Room room)
        {
            Coin coin = new Coin(state.QuinnPos, room);
            GoldSeed seed = new GoldSeed(state.QuinnPos, room);
            Gem gem = new Gem(state.QuinnPos, room);

            for (int c = 0; c < state.Coins; c++)
            {
                GameState.Quinn.GetCoin(coin);
            }

            for (int s = 0; s < state.Seeds; s++)
            {
                GameState.Quinn.GetSeed(seed);
            }

            for (int g = 0; g < state.Gems; g++)
            {
                GameState.Quinn.GetGem(gem);
            }

            Audio.StopAll("item_coin");
            Audio.StopAll("seed_collect");
            Audio.StopAll("item_gem");

            for (int p = 0; p < state.Perks.Count; p++)
            {
                GameState.Quinn.Perks[p] = new Perk(state.Perks[p]);
            }

            while (GameState.Quinn.Health > state.Health)
            {
                GameState.Quinn.GainHealth(-1);
            }
        }

        /// <summary>
        /// Load room state, Quinn position, and destroy all the enemies
        /// </summary>
        private static void LoadRoom(SaveState state, Room room)
        {
            if (!room.StartRoom)
            {
                Map.LeftStartRoom = true;
            }
            GameState.Quinn.Action = Quill.Actions.Warping;
            Map.ChangeRoom(state.QuinnPos, true);
            GameState.Quinn.Pos = state.QuinnPos;
            GameState.Quinn.Action = Quill.Actions.ExitDoor;
            foreach (Room.Exit exit in room.Exits)
            {
                exit.Open(0);
            }
            LevelStarted(room);
            room.Enemies.Clear();
            for (int o = room.WorldObjects.Count - 1; o >= 0; o--)
            {
                if (room.WorldObjects[o].Enemy)
                {
                    room.WorldObjects.RemoveAt(o);
                }
            }
        }

        /// <summary>
        /// Mark the level started. This flag is necessary for the rest of the game update loop.
        /// </summary>
        private static void LevelStarted(Room room)
        {
            Map.LevelStarted = true;
            if (!Map.Hub)
            {
                MusicPlayer.Play(room.Section.Theme, false);
                return;
            }
            if (!Audio.SpecialMusic)
            {
                MusicPlayer.Play(room.Section.Theme, false);
            }
        }

        /// <summary>
        /// Load the file of save states, or create a new one if one doesn't exist
        /// </summary>
        private static void InitializeSaveStates()
        {
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(Saves));
                    saves = (Saves)xml.Deserialize(fs);
                }
            }
            catch
            {
                saves.states = new List<SaveState>();
                WriteSaves();
                InitializeSaveStates();
            }

            savesInitialized = true;
        }

        private static void WriteSaves()
        {
            using (FileStream fs = File.Create(filePath))
            {
                XmlSerializer xml = new XmlSerializer(typeof(Saves));
                xml.Serialize(fs, saves);
            }
        }

        private enum Status
        {
            Loaded,
            Loading
        }
    }
}
