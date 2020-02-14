using System;
using System.Text;
namespace EagleIslandStudio.Entities {
	[Flags]
	public enum Actions
	{
		None,
		Up = 1,
		Down = 2,
		Left = 4,
		Right = 8,
		Jump = 16,
		Attack = 32,
		Feathers = 64,
		ToggleLeft = 128,
		ToggleRight = 256,
		Menu = 512,
		Map = 1024
	}
	public class InputRecord {
		public static char Delimiter = ',';
		public int Frames { get; set; }
		public Actions Actions { get; set; }
		public float Angle { get; set; }
		public string Notes { get; set; }
		public int ZeroPadding { get; set; }
		public bool FastForward { get; set; }
		public InputRecord(int frameCount, Actions actions, string notes = null) {
			Frames = frameCount;
			Actions = actions;
			Notes = notes;
			FastForward = false;
		}
		public InputRecord(string line) {
			Notes = string.Empty;

			int index = 0;
			Frames = ReadFrames(line, ref index);
			if (Frames == 0) {
				Notes = line;
				if (Notes.StartsWith("***")) {
					FastForward = true;
				}
				return;
			}

			while (index < line.Length) {
				char c = line[index];

				switch (char.ToUpper(c))
				{
					case 'L': Actions ^= Actions.Left; break;
					case 'R': Actions ^= Actions.Right; break;
					case 'U': Actions ^= Actions.Up; break;
					case 'D': Actions ^= Actions.Down; break;
					case 'X': Actions ^= Actions.Jump; break;
					case 'C': Actions ^= Actions.Attack; break;
					case 'Z': Actions ^= Actions.Feathers; break;
					case 'Q': Actions ^= Actions.ToggleLeft; break;
					case 'E': Actions ^= Actions.ToggleRight; break;
					case 'S': Actions ^= Actions.Menu; break;
					case 'M': Actions ^= Actions.Map; break;
				}

				index++;
			}
		}
		private int ReadFrames(string line, ref int start) {
			bool foundFrames = false;
			int frames = 0;

			while (start < line.Length) {
				char c = line[start];

				if (!foundFrames) {
					if (char.IsDigit(c)) {
						foundFrames = true;
						frames = c ^ 0x30;
						if (c == '0') { ZeroPadding = 1; }
					} else if (c != ' ') {
						return frames;
					}
				} else if (char.IsDigit(c)) {
					if (frames < 9999) {
						frames = frames * 10 + (c ^ 0x30);
						if (c == '0' && frames == 0) { ZeroPadding++; }
					} else {
						frames = 9999;
					}
				} else if (c != ' ') {
					return frames;
				}

				start++;
			}

			return frames;
		}
		
		public float GetX()
		{
			if (HasActions(Actions.Right))
			{
				return 1f;
			}
			else if (HasActions(Actions.Left))
			{
				return -1f;
			}
			return 0f;
		}
		public float GetY()
		{
			if (HasActions(Actions.Up))
			{
				return 1f;
			}
			else if (HasActions(Actions.Down))
			{
				return -1f;
			}
			return 0f;
		}
		public bool HasActions(Actions actions) {
			return (Actions & actions) != 0;
		}
		public override string ToString() {
			return Frames == 0 ? Notes : Frames.ToString().PadLeft(ZeroPadding, '0').PadLeft(4, ' ') + ActionsToString();
		}
		public string ActionsToString()
		{
			StringBuilder sb = new StringBuilder();
			if (HasActions(Actions.Left)) { sb.Append(",L"); }
			if (HasActions(Actions.Right)) { sb.Append(",R"); }
			if (HasActions(Actions.Up)) { sb.Append(",U"); }
			if (HasActions(Actions.Down)) { sb.Append(",D"); }
			if (HasActions(Actions.Jump)) { sb.Append(",X"); }
			if (HasActions(Actions.Attack)) { sb.Append(",C"); }
			if (HasActions(Actions.Feathers)) { sb.Append(",Z"); }
			if (HasActions(Actions.ToggleLeft)) { sb.Append(",Q"); }
			if (HasActions(Actions.ToggleRight)) { sb.Append(",E"); }
			if (HasActions(Actions.Menu)) { sb.Append(",S"); }
			if (HasActions(Actions.Map)) { sb.Append(",M"); }
			return sb.ToString();
		}
		public override bool Equals(object obj) {
			return obj is InputRecord && ((InputRecord)obj) == this;
		}
		public override int GetHashCode() {
			return Frames ^ (int)Actions;
		}
		public static bool operator ==(InputRecord one, InputRecord two) {
			bool oneNull = (object)one == null;
			bool twoNull = (object)two == null;
			if (oneNull != twoNull) {
				return false;
			} else if (oneNull && twoNull) {
				return true;
			}
			return one.Actions == two.Actions && one.Angle == two.Angle;
		}
		public static bool operator !=(InputRecord one, InputRecord two) {
			bool oneNull = (object)one == null;
			bool twoNull = (object)two == null;
			if (oneNull != twoNull) {
				return true;
			} else if (oneNull && twoNull) {
				return false;
			}
			return one.Actions != two.Actions || one.Angle != two.Angle;
		}
		public int ActionPosition() {
			return Frames == 0 ? -1 : Math.Max(4, Frames.ToString().Length);
		}
	}
}