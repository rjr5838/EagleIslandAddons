using Eagle_Island;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace TAS {
	public class InputController {
		private List<InputRecord> inputs = new List<InputRecord>();
		private int inputIndex, frameToNext;
		private string filePath;
		private List<InputRecord> fastForwards = new List<InputRecord>();
		public InputController(string filePath) {
			this.filePath = filePath;
		}

		public bool CanPlayback { get { return inputIndex < inputs.Count; } }
		public bool HasFastForward { get { return fastForwards.Count > 0; } }
		public int FastForwardSpeed { get { return fastForwards.Count == 0 ? 1 : fastForwards[0].Frames == 0 ? 400 : fastForwards[0].Frames; } }
        public int CurrentFrame { get; private set; }
        public int CurrentInputFrame { get { return CurrentFrame - frameToNext + Current.Frames; } }
		public InputRecord Current { get; set; }
		public InputRecord Previous {
			get {
				if (frameToNext != 0 && inputIndex - 1 >= 0 && inputs.Count > 0) {
					return inputs[inputIndex - 1];
				}
				return null;
			}
		}
		public InputRecord Next {
			get {
				if (frameToNext != 0 && inputIndex + 1 < inputs.Count) {
					return inputs[inputIndex + 1];
				}
				return null;
			}
		}
		public bool HasInput(Actions action) {
			InputRecord input = Current;
			return input.HasActions(action);
		}
		public bool HasInputPressed(Actions action) {
			InputRecord input = Current;

			return input.HasActions(action) && CurrentInputFrame == 1;
		}
		public bool HasInputReleased(Actions action) {
			InputRecord current = Current;
			InputRecord previous = Previous;

			return !current.HasActions(action) && previous != null && previous.HasActions(action) && CurrentInputFrame == 1;
		}
		public override string ToString() {
			if (frameToNext == 0 && Current != null) {
				return Current.ToString() + "(" + CurrentFrame.ToString() + ")";
			} else if (inputIndex < inputs.Count && Current != null) {
				int inputFrames = Current.Frames;
				int startFrame = frameToNext - inputFrames;
				return Current.ToString() + "(" + (CurrentFrame - startFrame).ToString() + " / " + inputFrames + " : " + CurrentFrame + ")";
			}
			return string.Empty;
		}
		public string NextInput() {
			if (frameToNext != 0 && inputIndex + 1 < inputs.Count) {
				return inputs[inputIndex + 1].ToString();
			}
			return string.Empty;
		}
		public void InitializePlayback() {
			int trycount = 5;
			while (!ReadFile() && trycount >= 0) {
				System.Threading.Thread.Sleep(50);
				trycount--;
			}

			CurrentFrame = 0;
			inputIndex = 0;
			if (inputs.Count > 0) {
				Current = inputs[0];
				frameToNext = Current.Frames;
			} else {
				Current = new InputRecord();
				frameToNext = 1;
			}
		}
		public void ReloadPlayback() {
			int playedBackFrames = CurrentFrame;
			InitializePlayback();
			CurrentFrame = playedBackFrames;

			while (CurrentFrame >= frameToNext) {
				if (inputIndex + 1 >= inputs.Count) {
					inputIndex++;
					return;
				}
				if (Current.FastForward) {
					fastForwards.RemoveAt(0);
				}
				Current = inputs[++inputIndex];
				frameToNext += Current.Frames;
			}
		}
		public void InitializeRecording() {
			CurrentFrame = 0;
			inputIndex = 0;
			Current = new InputRecord();
			frameToNext = 0;
			inputs.Clear();
			fastForwards.Clear();
		}
		public void PlaybackPlayer() {
			if (inputIndex < inputs.Count && !Manager.IsLoading()) {
				if (CurrentFrame >= frameToNext) {
					if (inputIndex + 1 >= inputs.Count) {
						inputIndex++;
						return;
					}
					if (Current.FastForward) {
						fastForwards.RemoveAt(0);
					}
					Current = inputs[++inputIndex];
					frameToNext += Current.Frames;
				}

				CurrentFrame++;
			}
			Manager.SetInputs(Current);
		}
		public void RecordPlayer() {
			InputRecord input = new InputRecord() { Line = inputIndex + 1, Frames = CurrentFrame };
			GetCurrentInputs(input);

			if (CurrentFrame == 0 && input == Current) {
				return;
			} else if (input != Current && !Manager.IsLoading()) {
				Current.Frames = CurrentFrame - Current.Frames;
				inputIndex++;
				if (Current.Frames != 0) {
					inputs.Add(Current);
				}
				Current = input;
			}
			CurrentFrame++;
		}
		private static void GetCurrentInputs(InputRecord record) {
			if (Input.Held(Input.Actions.Up)) { record.Actions |= Actions.Up; }
			if (Input.Held(Input.Actions.Down)) { record.Actions |= Actions.Down; }
			if (Input.Held(Input.Actions.Left)) { record.Actions |= Actions.Left; }
			if (Input.Held(Input.Actions.Right)) { record.Actions |= Actions.Right; }
			if (Input.Held(Input.Actions.Jump)) { record.Actions |= Actions.Jump; }
			if (Input.Held(Input.Actions.Attack)) { record.Actions |= Actions.Attack; }
			if (Input.Held(Input.Actions.Feathers)) { record.Actions |= Actions.Feathers; }
			if (Input.Held(Input.Actions.ToggleLeft)) { record.Actions |= Actions.ToggleLeft; }
			if (Input.Held(Input.Actions.ToggleRight)) { record.Actions |= Actions.ToggleRight; }
			if (Input.Held(Input.Actions.Menu)) { record.Actions |= Actions.Menu; }
			if (Input.Held(Input.Actions.Map)) { record.Actions |= Actions.Map; }
		}
		public void WriteInputs() {
			using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) {
				for (int i = 0; i < inputs.Count; i++) {
					InputRecord record = inputs[i];
					byte[] data = Encoding.ASCII.GetBytes(record.ToString() + "\r\n");
					fs.Write(data, 0, data.Length);
				}
				fs.Close();
			}
		}
		private bool ReadFile() {
			try {
				inputs.Clear();
				fastForwards.Clear();
				if (!File.Exists(filePath)) { return false; }

				int lines = 0;
				using (StreamReader sr = new StreamReader(filePath)) {
					while (!sr.EndOfStream) {
						string line = sr.ReadLine();

						if (line.IndexOf("Read", System.StringComparison.OrdinalIgnoreCase) == 0 && line.Length > 5) {
							lines++;
							ReadFile(line.Substring(5), lines);
							lines--;
						}

						InputRecord input = new InputRecord(++lines, line);
						if (input.FastForward) {
							fastForwards.Add(input);

							if (inputs.Count > 0) {
								inputs[inputs.Count - 1].ForceBreak = input.ForceBreak;
								inputs[inputs.Count - 1].FastForward = true;
							}
						} else if (input.Frames != 0) {
							inputs.Add(input);
						}
					}
				}
				return true;
			} catch {
				return false;
			}
		}
		private void ReadFile(string extraFile, int lines) {
			int index = extraFile.IndexOf(',');
			string filePath = index > 0 ? extraFile.Substring(0, index) : extraFile;
			if (!File.Exists(filePath)) {
				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{filePath}*.tas");
				filePath = (files.GetValue(0)).ToString();
				if (!File.Exists(filePath)) { return; }
			}
			int skipLines = 0;
			int lineLen = int.MaxValue;
			if (index > 0) {
				int indexLen = extraFile.IndexOf(',', index + 1);
				if (indexLen > 0) {
					string startLine = extraFile.Substring(index + 1, indexLen - index - 1);
					string endLine = extraFile.Substring(indexLen + 1);
					GetLine(startLine, filePath, out skipLines);
					GetLine(endLine, filePath, out lineLen);
				} else {
					string startLine = extraFile.Substring(index + 1);
					GetLine(startLine, filePath, out skipLines);
				}
			}

			int subLine = 0;
			using (StreamReader sr = new StreamReader(filePath)) {
				while (!sr.EndOfStream) {
					string line = sr.ReadLine();

					subLine++;
					if (subLine <= skipLines) { continue; }
					if (subLine > lineLen) { break; }

					if (line.IndexOf("Read", System.StringComparison.OrdinalIgnoreCase) == 0 && line.Length > 5) {
						ReadFile(line.Substring(5), lines);
					}

					InputRecord input = new InputRecord(lines, line);
					if (input.FastForward) {
						fastForwards.Add(input);

						if (inputs.Count > 0) {
							inputs[inputs.Count - 1].ForceBreak = input.ForceBreak;
							inputs[inputs.Count - 1].FastForward = true;
						}
					} else if (input.Frames != 0) {
						inputs.Add(input);
					}
				}
			}
		}
		private void GetLine(string labelOrLineNumber, string path, out int lineNumber) {
                	if (!int.TryParse(labelOrLineNumber, out lineNumber)) {
				int curLine = 0;
				using (StreamReader sr = new StreamReader(path)) {
					while (!sr.EndOfStream) {
						curLine++;
						string line = sr.ReadLine();
						if (line == ("#" + labelOrLineNumber)) {
							lineNumber = curLine;
							return;
						}
					}
					lineNumber = int.MaxValue;
				}
			}
		}
	}
}
