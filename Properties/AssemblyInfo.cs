using System.Reflection;
using System.Runtime.InteropServices;
using ASMXML;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle("Bannerlord.AutomaticSubModuleXML")]
[assembly: AssemblyProduct("Automatic Sub Module XML")]
[assembly: AssemblyCopyright("2023, pointfeev (https://github.com/pointfeev)")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// attribute examples
[assembly: ModuleId("Bannerlord.AutomaticSubModuleXML")]
[assembly: ModuleName("Automatic Sub Module XML")]
[assembly: ModuleVersion("v1.0.0")]
[assembly: ModuleDefault(true)]
[assembly: ModuleCategory("Singleplayer")]
[assembly: ModuleType("Community")]
[assembly: ModuleDependedModule("Bannerlord.Harmony", "v2.3.0")]
[assembly: ModuleDependedModule("Bannerlord.MBOptionScreen", "v5.7.1", true)]
[assembly: ModuleDependedModule("Native", "v1.1.3")]
[assembly: ModuleDependedModule("SandboxCore", "v1.1.3")]
[assembly: ModuleDependedModule("Sandbox", "v1.1.3")]
[assembly: ModuleDependedModule("StoryMode", "v1.1.3")]
[assembly: ModuleDependedModule("CustomBattle", "v1.1.3", true)]
[assembly: ModuleDependedModule("BirthAndDeath", "v1.1.3", true)]
[assembly: ModuleModulesToLoadAfterThis("Bannerlord.Harmony")]
[assembly: ModuleModulesToLoadAfterThis("Bannerlord.MBOptionScreen")]
[assembly: ModuleIncompatibleModule("Bannerlord.Harmony")]
[assembly: ModuleIncompatibleModule("Bannerlord.MBOptionScreen")]
[assembly:
    ModuleSubModule("AutomaticSubModuleXML", "Bannerlord.AutomaticSubModuleXML.dll", "Bannerlord.AutomaticSubModuleXML.SubModule",
        new[] { "DedicatedServerType", "none", "IsNoRenderModeElement", "false" })]
[assembly: ModuleXml("GameText", "strings")]
[assembly: ModuleXml("CraftingPieces", "crafting_pieces", new[] { "Campaign", "CampaignStoryMode", "CustomGame", "EditorGame" })]
[assembly: ModuleXml("CraftingPieces", "mp_crafting_pieces", new[] { "MultiplayerGame" })]