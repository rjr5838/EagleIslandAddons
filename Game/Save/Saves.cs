using System;
using System.Collections.Generic;
using System.Linq;

namespace TAS.Save
{
    [Serializable]
    public class Saves
    {
        public List<SaveState> states { get; set; }

        public void SetState(int index, SaveState state)
        {
            int listIndex = states.FindIndex(s => s.Index == index);
            if (listIndex >= 0)
            {
                states[listIndex] = state;
            } 
            else
            {
                states.Add(state);
            }
        }

        public SaveState GetState(int index)
        {
            return states.FirstOrDefault(s => s.Index == index);
        }
    }
}
