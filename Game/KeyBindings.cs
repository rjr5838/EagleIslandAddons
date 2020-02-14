using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace TAS {
	public struct KeyBindings {
		public List<Keys> keyStart;
		public List<Keys> keyFastForward;
		public List<Keys> keyFrameAdvance;
		public List<Keys> keyPause;
		public List<Keys> keyRecord;
		public List<Keys> keySave;
		public List<Keys> keyLoad;
	}
}
