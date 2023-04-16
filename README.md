## Bannerlord.AutomaticSubModuleXML
For personal use within my Bannerlord mods

### If you wish to use this yourself:
- Edit the exact output for your needs [within this method](https://github.com/pointfeev/AutomaticSubModuleXML/blob/main/AutomaticSubModuleXML.cs#L17).
- If you use [Harmony](https://github.com/pardeike/Harmony), make sure you set its DLL to be output during builds ([example](https://github.com/pointfeev/SortedIncome/blob/main/SortedIncome.csproj#L42)); this project will utilize that DLL to [populate the dependency](https://github.com/pointfeev/AutomaticSubModuleXML/blob/main/AutomaticSubModuleXML.cs#L45) and then [delete it automatically](https://github.com/pointfeev/AutomaticSubModuleXML/blob/main/AutomaticSubModuleXML.cs#L49) so it doesn't cause issues.
- If you use [Mod Configuration Menu](https://github.com/Aragas/Bannerlord.MBOptionScreen), make sure you [set it up as a soft dependency](https://mcm.bannerlord.aragas.org/articles/MCMv5/mcmv5-soft-dependency.html) OR add two lines to delete the DLL and its PDB.
- If you have any other mod dependencies, make sure they get output and add lines to deal with them as necessary just like is done with Harmony and Mod Configuration Menu above.
- Build the project and move the output DLL to the desired directory.
- Edit your AssemblyInfo.cs parameters to match [these values](https://github.com/pointfeev/SortedIncome/blob/main/Properties/AssemblyInfo.cs#L6) (see [here](https://github.com/pointfeev/SortedIncome/blob/main/SubModule.cs#L19) for what those variables are referencing).
- Add the UsingTask and Target tags to your .csproj and edit the AssemblyFile attribute to the path of your output DLL ([example](https://github.com/pointfeev/SortedIncome/blob/main/SortedIncome.csproj#L50)).
- Make sure the assembly name and root namespace are the same ([example](https://github.com/pointfeev/SortedIncome/blob/main/SortedIncome.csproj#L12)).
- Build your project and enjoy an automatically updated SubModule.xml!
