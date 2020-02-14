using Eagle_Island;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TAS
{
    public static class SaveManager
    {
        private static Room savedRoom = null;
        private static Vector2 quinnPos = Vector2.Zero;
        private static Vector2 roomCoords = Vector2.Zero;
        private static bool savedInHub = false;
        private static int coins, seeds, health, gems, enemiesKilled;
        private static List<Perk.Names> perks;
        private static State state = State.Loaded;
        public static void Save()
        {
            savedRoom = GameState.Quinn.Room;
            quinnPos = new Vector2(GameState.Quinn.Pos.X, GameState.Quinn.Pos.Y - 10);
            roomCoords = new Vector2(savedRoom.CoordX, savedRoom.CoordY);
            coins = GameState.Quinn.Coins;
            seeds = GameState.Quinn.Seeds;
            health = GameState.Quinn.Health;
            gems = GameState.Quinn.Gems;
            enemiesKilled = Map.EnemiesDefeated.Count;
            perks = new List<Perk.Names>();
            savedInHub = Map.Hub;
            for(int i = 0; i < GameState.Quinn.Perks.Length; i++)
            {
                if (GameState.Quinn.Perks[i] != null)
                {
                    perks.Add(GameState.Quinn.Perks[i].Name);
                }
            }
        }

        public static bool Load()
        {
            switch (state)
            {
                case State.Loaded:
                    Map.Fade = 1f;
                    if (savedInHub)
                    {
                        Map.LoadHub(Map.Outcomes.ExitLevel);   
                    }
                    else
                    {
                        Map.RestartLevelFromMenu();
                    }
                    state = State.Loading;
                    break;
                case State.Loading:
                    if (Map.Ready && Map.StartRoomReady)
                    {
                        foreach (Room room in Map.Rooms)
                        {
                            if (room.CoordVec.Equals(roomCoords))
                            {
                                if (!room.StartRoom)
                                {
                                    Map.LeftStartRoom = true;
                                }
                                Map.CurrentRoom = room;
                                Map.ChangeRoom(quinnPos, false);
                                GameState.Quinn.Pos = quinnPos;

                                Coin coin = new Coin(quinnPos, room);
                                GoldSeed seed = new GoldSeed(quinnPos, room);
                                Gem gem = new Gem(quinnPos, room);
                                
                                for (int c = 0; c < coins; c++)
                                {
                                    GameState.Quinn.GetCoin(coin);
                                }

                                for (int s = 0; s < seeds; s++)
                                {
                                    GameState.Quinn.GetSeed(seed);
                                }

                                for (int g = 0; g < gems; g++)
                                {
                                    GameState.Quinn.GetGem(gem);
                                }

                                Audio.StopAll("item_coin");
                                Audio.StopAll("seed_collect");
                                Audio.StopAll("item_gem");

                                for (int p = 0; p < perks.Count; p++)
                                {
                                    GameState.Quinn.Perks[p] = new Perk(perks[p]);
                                }

                                while (GameState.Quinn.Health > health)
                                {
                                    GameState.Quinn.GainHealth(-1);
                                }


                                room.Enemies.Clear();
                                for(int o = room.WorldObjects.Count-1; o >= 0; o--)
                                {
                                    if (room.WorldObjects[o].Enemy)
                                    {
                                        room.WorldObjects.RemoveAt(o);
                                    }
                                }

                                Raven.Set(15, Raven.Frame, (int)Map.Level);
                                Raven.SetStuff(0, 0);
                                

                                WorldObject destroyedEnemy = new BlankObject(Vector2.Zero, room);
                                destroyedEnemy.SetEnemy(WorldObject.EnemyTypes.Plant, "ForestShroom");
                                for (int e = 0; e < enemiesKilled; e++) {
                                    Map.EnemiesDefeated.Add(destroyedEnemy);
                                }

                                LevelStarted(room);

                                state = State.Loaded;
                            }
                        }
                    }
                    break;
            }

            return state != State.Loaded;
        }

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

        private enum State
        {
            Loaded,
            Loading
        }
    }
}
