using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Bannerlord.AutomaticSubModuleXML;

public class AutomaticSubModuleXML : Task
{
    [Required] public string TargetPath { get; set; }

    [Required] public string TargetDir { get; set; }

    public override bool Execute()
    {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(TargetPath);
        string name = versionInfo.ProductName!;
        string url = versionInfo.FileDescription!;
        string version = new Version(versionInfo.FileVersion!).ToString(3);
        AssemblyName assemblyName = Assembly.Load(File.ReadAllBytes(TargetPath)).GetName();
        string id = assemblyName.Name!;
        string gameVersion = assemblyName.Version!.ToString(3);
        string subModule = Path.GetFullPath(TargetDir + @"..\..\") + @"SubModule.xml";
        StringBuilder logMessage = new();
        _ = logMessage.AppendLine($"SubModule -> {subModule}");
        StringBuilder xml = new();
        _ = xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        _ = xml.AppendLine(
            "<Module xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"https://raw.githubusercontent.com/BUTR/Bannerlord.XmlSchemas/master/SubModule.xsd\">");
        _ = xml.AppendLine($"\t<Id value=\"{id}\"/>");
        _ = logMessage.AppendLine($"\tId = {id}");
        _ = xml.AppendLine($"\t<Name value=\"{name}\"/>");
        _ = logMessage.AppendLine($"\tName = {name}");
        _ = xml.AppendLine($"\t<Version value=\"v{version}\"/>");
        _ = logMessage.AppendLine($"\tVersion = {version}");
        _ = xml.AppendLine("\t<DefaultModule value=\"true\"/>");
        _ = xml.AppendLine("\t<ModuleCategory value=\"Singleplayer\"/>");
        _ = xml.AppendLine("\t<ModuleType value=\"Community\"/>");
        _ = xml.AppendLine($"\t<Url value=\"{url}\"/>");
        _ = logMessage.AppendLine($"\tUrl = {url}");
        _ = xml.AppendLine("\t<DependedModules>");
        string harmony = TargetDir + "\\0Harmony.dll";
        if (File.Exists(harmony))
        {
            string harmonyVersion = new Version(FileVersionInfo.GetVersionInfo(harmony).FileVersion!).ToString(3);
            File.Delete(harmony);
            _ = xml.AppendLine($"\t\t<DependedModule Id=\"Bannerlord.Harmony\" DependentVersion=\"v{harmonyVersion}\" Optional=\"false\"/>");
            _ = logMessage.AppendLine($"\tHarmony version >= {harmonyVersion}");
        }
        string mcm = TargetDir + "\\MCMv5.dll";
        if (File.Exists(mcm))
        {
            string mcmVersion = new Version(FileVersionInfo.GetVersionInfo(mcm).FileVersion!).ToString(3);
            _ = xml.AppendLine($"\t\t<DependedModule Id=\"Bannerlord.MBOptionScreen\" DependentVersion=\"v{mcmVersion}\" Optional=\"true\"/>");
            _ = logMessage.AppendLine($"\tMCM version >= {mcmVersion}");
        }
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"Native\" DependentVersion=\"v{gameVersion}\" Optional=\"false\"/>");
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"SandBoxCore\" DependentVersion=\"v{gameVersion}\" Optional=\"false\"/>");
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"Sandbox\" DependentVersion=\"v{gameVersion}\" Optional=\"false\"/>");
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"StoryMode\" DependentVersion=\"v{gameVersion}\" Optional=\"false\"/>");
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"CustomBattle\" DependentVersion=\"v{gameVersion}\" Optional=\"true\"/>");
        _ = xml.AppendLine($"\t\t<DependedModule Id=\"BirthAndDeath\" DependentVersion=\"v{gameVersion}\" Optional=\"true\"/>");
        _ = logMessage.Append($"\tGame version >= {gameVersion}");
        _ = xml.AppendLine("\t</DependedModules>");
        _ = xml.AppendLine("\t<SubModules>");
        _ = xml.AppendLine("\t\t<SubModule>");
        _ = xml.AppendLine($"\t\t\t<Name value=\"{id}\"/>");
        _ = xml.AppendLine($"\t\t\t<DLLName value=\"{id}.dll\"/>");
        _ = xml.AppendLine($"\t\t\t<SubModuleClassType value=\"{id}.SubModule\"/>");
        _ = xml.AppendLine("\t\t\t<Tags>");
        _ = xml.AppendLine("\t\t\t\t<Tag key=\"DedicatedServerType\" value=\"none\"/>");
        _ = xml.AppendLine("\t\t\t\t<Tag key=\"IsNoRenderModeElement\" value=\"false\"/>");
        _ = xml.AppendLine("\t\t\t</Tags>");
        _ = xml.AppendLine("\t\t</SubModule>");
        _ = xml.AppendLine("\t</SubModules>");
        _ = xml.Append("</Module>");
        File.WriteAllText(subModule, xml.ToString());
        Log.LogMessage(MessageImportance.High, logMessage.ToString());
        return !Log.HasLoggedErrors;
    }
}