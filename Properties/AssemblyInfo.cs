using System.Reflection;
using System.Runtime.InteropServices;
using Bannerlord.AutomaticSubModuleXML;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle("Bannerlord.AutomaticSubModuleXML")]
[assembly: AssemblyProduct("AutomaticSubModuleXML")]
[assembly: AssemblyCopyright("2023, pointfeev (https://github.com/pointfeev)")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// attribute examples
[assembly: ModuleId("Bannerlord.AutomaticSubModuleXML")]
[assembly: ModuleName("AutomaticSubModuleXML")]
[assembly: ModuleVersion("v1.0.0")]
[assembly: ModuleDefault(true)]
[assembly: ModuleCategory(ModuleCategoryValue.Singleplayer)]
[assembly: ModuleType(ModuleTypeValue.Community)]
[assembly: ModuleUrl("https://github.com/pointfeev/AutomaticSubModuleXML")]
[assembly: ModuleDependency("Bannerlord.Harmony", "v2.2.2")]
[assembly: ModuleDependency("Bannerlord.MBOptionScreen", "v5.7.1")]
[assembly: ModuleDependency("Native", "v1.1.3")]
[assembly: ModuleDependency("SandboxCore", "v1.1.3")]
[assembly: ModuleDependency("Sandbox", "v1.1.3")]
[assembly: ModuleDependency("StoryMode", "v1.1.3")]
[assembly: ModuleDependency("CustomBattle", "v1.1.3", true)]
[assembly: ModuleDependency("BirthAndDeath", "v1.1.3", true)]
[assembly:
    ModuleSubModule("AutomaticSubModuleXML", "Bannerlord.AutomaticSubModuleXML.dll", "Bannerlord.AutomaticSubModuleXML.SubModule",
        new[] { "DedicatedServerType", "none", "IsNoRenderModeElement", "false" })]