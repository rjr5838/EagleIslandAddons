### Manually

- Currently only works/tested for the Windows version.
  
- Go to [Releases](https://github.com/rjr5838/EagleIslandTAS/releases)
- Download EagleIsland-Addons.dll
- You will need the modified EagleIsland.exe as well. I wont host it here.
  - Make a backup of your EagleIsland.exe before starting!
  - You can modify it yourself with dnSpy or similar
  - Load EagleIsland.exe in dnSpy and EagleIsland-Addons.dll as well

![dnSpy](https://raw.githubusercontent.com/rjr5838/EagleIslandTAS/master/Images/dnSpy01.png)

  - Change EagleIsland.exe according to the document here [Modified](https://github.com/rjr5838/EagleIsland/blob/master/Game/WhatsModified.txt)
  - First step is to modify the IL instructions of the method and add a call into TAS.Manager.UpdateInputs()
  - Find the EagleIsland.Update(GameTime) method in dnSpy

![dnSpy](https://raw.githubusercontent.com/rjr5838/EagleIsland/master/Images/dnSpy02.png)

  - Right click in the right hand window and select Edit IL Instructions...
  - In this window right click and select Add New Instruction...

![dnSpy](https://raw.githubusercontent.com/rjr5838/EagleIsland/master/Images/dnSpy03.png)

  - Change the OpCode to be 'call' and then click the 'null' in the operand and select 'Method' then browse to EagleIsland-Addons.TAS.Manager.UpdateInputs
  - This adds a reference to EagleIsland-Addons.dll
  - Click OK in the Edit Method Body window
  - Then go to File -> Save Module and save this modified exe
  - Then go to File -> Reload All Assemblies to load the modified exe
  - Go to the Input class
  - Right click the class name > Edit Class (C#)
  - Add in the new TasGamepadState property from the txt file linked above
  - Replace the Update method that is here with the 2 Update methods from the txt file linked above
  - Compile
  - Then go to File -> Save Module and save this modified exe
  - Then go to File -> Reload All Assemblies to load the modified exe
  - Go back tot he EagleIsland.Update method
  - This time right click in the right window and select 'Edit Method (C#)'
  - Replace the body of the method with the body in the txt file linked above
  - Go to Interface.DrawGame method
  - Right click and select 'Edit Class'
  - Change Explosions from public to internal
  - Add TAS.InterfaceManager.DrawGame(spriteBatch) right before spriteBatch.end() (check the txt file)
  - For each of the methods/fields/properties listed at the bottom of the txt file, do the following:
    - Navigate to it in dnSpy
    - Press Alt+Enter to edit it
    - Change the access type to Public.
  - Save the modified version and you should be good to go
- Place EagleIsland-Addons.dll in your Eagle Island game directory (usually C:\Program Files (x86)\Steam\steamapps\common\Eagle Island\)
