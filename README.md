# EagleIsland
TAS and Practice Tools for the game Eagle Island

## Installation

### Patch

Using the patch is the easiest way to get started. Instructions can be found [here.](https://github.com/rjr5838/EagleIslandAddons/blob/master/Game/Components/Instructions.md)

## Practice

### Actions Available
 - F7  = Save 
 - F8 / Press Left Stick  = Load
 - F9  = Previous Slot
 - F10 = Next Slot
 - 6   = Set Health/Mana to full

 - These can be rebound in (Main Eagle Island Directory)\TASsettings.xml
  - Note that you will have to reload the game for the changes to take affect.

## TAS (Work in Progress, not all below will work properly)

### Input File
Input file is called EagleIsland.tas and needs to be in the main EagleIsland directory (usually C:\Program Files (x86)\Steam\steamapps\common\Eagle Island\EagleIsland.tas)

Format for the input file is (Frames),(Actions)

e.g. 123,R,J (For 123 frames, hold Right and Jump)

### Actions Available
- R = Right
- L = Left
- U = Up
- D = Down
- X = Jump
- C = Attack
- Z = Feather Ring
- Q = Toggle Feather Left
- E = Toggle Feather Right 
- S = Menu

### Playback of Input File
#### Keyboard
While in game
- Start/Stop Playback: 1
- Fast Forward / Frame Advance Continuously: 3
- Pause / Frame Advance: 0
- Unpause: 2
- These can be rebound in (Main Eagle Island Directory)\TASsettings.xml
  - Note that you will have to reload the game for the changes to take affect.
  
#### Controller
While in game

- Start/Stop Playback: Right Stick
- Fast Forward / Frame Advance Continuously: Right Stick X+

### Special Input
#### Breakpoints
- You can create a breakpoint in the input file by typing *** by itself on a single line
- The program when played back from the start will fast forward until it reaches that line and then go into frame stepping mode
- You can specify the speed with ***X, where X is the speedup factor. e.g. ***10 will go at 10x speed
- ***! will force the TAS to pause even if there are breakpoints afterward in the file

#### Read
- Read,File Name,Starting Line,(Optional Ending Line)
- Will read inputs from the specified file.
- Currently requires files to be in the main Eagle Island directory.
- e.g. "Read,1A - Glades.tas,6" will read all inputs after line 6 from the '1A - Glades.tas' file
- This will also work if you shorten the file name, i.e. "Read,1A,6" will do the same 
- It's recommended to use labels instead of line numbers, so "Read,1A,lvl_1" would be the preferred format for this example.

#### Labels
- Prefixing a line with # will comment out the line
- A line beginning with # can be also be used as the starting point or ending point of a Read instruction.
- You can comment highlighted text in Eagle Island Studio by hitting Ctrl+K
  
### Eagle Island Studio
Can be used instead of notepad or similar for easier editing of the TAS file. Is located in [Releases](https://github.com/rjr5838/EagleIslandAddons/releases) as well.

If EagleIsland.exe is running it will automatically open EagleIsland.tas if it exists. You can hit Ctrl+O to open a different file, which will automatically save it to EagleIsland.tas as well. Ctrl+Shift+S will open a Save As dialog as well.
