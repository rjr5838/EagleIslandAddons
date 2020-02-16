using Eagle_Island;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static Eagle_Island.Map;

namespace TAS
{
    [Serializable]
    public class SaveState
    {
        public int Index { get; set; }
        public Vector2 QuinnPos { get; set; }
        public Vector2 RoomCoords { get; set; }
        public bool SavedInHub { get; set; }
        public Levels Level { get; set; }
        public int Coins { get; set; }
        public int Seeds { get; set; }
        public int Health { get; set; }
        public int Gems { get; set; }
        public int EnemiesKilled { get; set; }
        public List<Perk.Names> Perks { get; set; }

        private SaveState() { }

        public static SaveState Create(int index)
        {
            SaveState state = new SaveState();

            state.Index = index;
            // Save the position a couple pixels up from where Quinn is standing to avoid Quinn spawning in the ground
            state.QuinnPos = new Vector2(GameState.Quinn.Pos.X, GameState.Quinn.Pos.Y - 10);
            state.RoomCoords = new Vector2(GameState.Quinn.Room.CoordX, GameState.Quinn.Room.CoordY);
            state.Coins = GameState.Quinn.Coins;
            state.Seeds = GameState.Quinn.Seeds;
            state.Health = GameState.Quinn.Health;
            state.Gems = GameState.Quinn.Gems;
            state.EnemiesKilled = Map.EnemiesDefeated.Count;
            state.SavedInHub = Map.Hub;
            state.Level = Map.Level;
            state.Perks = new List<Perk.Names>();
            
            for (int i = 0; i < GameState.Quinn.Perks.Length; i++)
            {
                if (GameState.Quinn.Perks[i] != null)
                {
                    state.Perks.Add(GameState.Quinn.Perks[i].Name);
                }
            }

            return state;
        }
    }
}
