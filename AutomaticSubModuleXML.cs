using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ASMXML;

public class AutomaticSubModuleXML : Task
{
    private readonly StringBuilder Message = new();
    private readonly StringBuilder Output = new();

    [Required] public string OutputPath { get; set; }
    [Required] public string Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Version { get; set; }
    [Required] public string DefaultModule { get; set; }
    [Required] public string ModuleCategory { get; set; }
    [Required] public string ModuleType { get; set; }
    public string[] DependedModules { get; set; }
    public string[] ModulesToLoadAfterThis { get; set; }
    public string[] IncompatibleModules { get; set; }
    public string[] SubModules { get; set; }
    public string[] Xmls { get; set; }

    public override bool Execute()
    {
        string output = Path.GetFullPath(OutputPath + @"\..\..\SubModule.xml");
        _ = Message.AppendLine($"SubModule -> {output}");
        _ = Output.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        _ = Output.AppendLine("<Module>");
        string id = Id;
        _ = Output.AppendLine($"\t<Id value=\"{id}\" />");
        _ = Message.AppendLine($"\tId = {id}");
        string name = Name;
        _ = Output.AppendLine($"\t<Name value=\"{name}\" />");
        _ = Message.AppendLine($"\tName = {name}");
        string version = Version;
        _ = Output.AppendLine($"\t<Version value=\"{version}\" />");
        _ = Message.AppendLine($"\tVersion = {version}");
        string defaultModule = DefaultModule;
        _ = Output.AppendLine($"\t<DefaultModule value=\"{defaultModule}\" />");
        _ = Message.AppendLine($"\tDefaultModule = {defaultModule}");
        string moduleCategory = ModuleCategory;
        _ = Output.AppendLine($"\t<ModuleCategory value=\"{moduleCategory}\" />");
        _ = Message.AppendLine($"\tModuleCategory = {moduleCategory}");
        string moduleType = ModuleType;
        _ = Output.AppendLine($"\t<ModuleType value=\"{moduleType}\" />");
        _ = Message.AppendLine($"\tModuleType = {moduleType}");
        string[] dependencies = DependedModules;
        if (dependencies?.Length > 0)
        {
            _ = Output.AppendLine("\t<DependedModules>");
            _ = Message.AppendLine("\tDependedModules:");
            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependencyId = dependencies[i];
                _ = Output.Append($"\t\t<DependedModule Id=\"{dependencyId}\"");
                _ = Message.Append($"\t\t{dependencyId}");
                if (i + 1 <= dependencies.Length && dependencies[i + 1] is { } dependencyVersion
                                                 && System.Version.TryParse(Regex.Replace(dependencyVersion, "[^0-9.]", ""), out _))
                {
                    _ = Output.Append($" DependentVersion=\"{dependencyVersion}\"");
                    _ = Message.Append($" >= {dependencyVersion}");
                    i++;
                }
                if (i + 1 <= dependencies.Length && bool.TryParse(dependencies[i + 1], out bool dependencyOptional)
                 || i + 2 <= dependencies.Length && bool.TryParse(dependencies[i + 2], out dependencyOptional))
                    i++;
                else
                    dependencyOptional = false;
                _ = Output.Append($" Optional=\"{dependencyOptional.ToString().ToLower()}\"");
                if (dependencyOptional)
                    _ = Message.Append(" (optional)");
                _ = Output.AppendLine(" />");
                _ = Message.AppendLine();
            }
            _ = Output.AppendLine("\t</DependedModules>");
        }
        string[] modulesToLoadAfterThis = ModulesToLoadAfterThis;
        if (modulesToLoadAfterThis?.Length > 0)
        {
            _ = Output.AppendLine("\t<ModulesToLoadAfterThis>");
            _ = Message.AppendLine("\tModulesToLoadAfterThis:");
            foreach (string moduleId in modulesToLoadAfterThis)
            {
                _ = Output.AppendLine($"\t\t<Module Id=\"{moduleId}\" />");
                _ = Message.AppendLine($"\t\t{moduleId}");
            }
            _ = Output.AppendLine("\t</ModulesToLoadAfterThis>");
        }
        string[] incompatibilities = IncompatibleModules;
        if (incompatibilities?.Length > 0)
        {
            _ = Output.AppendLine("\t<IncompatibleModules>");
            _ = Message.AppendLine("\tIncompatibleModules:");
            foreach (string moduleId in incompatibilities)
            {
                _ = Output.AppendLine($"\t\t<Module Id=\"{moduleId}\" />");
                _ = Message.AppendLine($"\t\t{moduleId}");
            }
            _ = Output.AppendLine("\t</IncompatibleModules>");
        }
        string[] subModules = SubModules;
        if (subModules?.Length > 0)
        {
            _ = Output.AppendLine("\t<SubModules>");
            _ = Message.AppendLine("\tSubModules:");
            for (int i = 0; i < subModules.Length; i++)
            {
                string subModuleName = subModules[i];
                _ = Output.AppendLine("\t\t<SubModule>");
                _ = Message.AppendLine($"\t\t{subModuleName}:");
                _ = Output.AppendLine($"\t\t\t<Name value=\"{subModuleName}\" />");
                if (++i > subModules.Length)
                {
                    Log.LogError($"Missing DLLName for SubModule \"{subModuleName}\"");
                    break;
                }
                string subModuleDllName = subModules[i];
                _ = Output.AppendLine($"\t\t\t<DLLName value=\"{subModuleDllName}\" />");
                _ = Message.AppendLine($"\t\t\tDLLName = {subModuleDllName}");
                if (++i > subModules.Length)
                {
                    Log.LogError($"Missing SubModuleClassType for SubModule \"{subModuleName}\" with DLLName \"{subModuleDllName}\"");
                    break;
                }
                string subModuleClassType = subModules[i];
                _ = Output.AppendLine($"\t\t\t<SubModuleClassType value=\"{subModuleClassType}\" />");
                _ = Message.AppendLine($"\t\t\tSubModuleClassType = {subModuleClassType}");
                // NEED TO SUPPORT TAGS
                /*if (subModule.Tags?.Length > 0)
                {
                    _ = Output.AppendLine("\t\t\t<Tags>");
                    _ = Message.AppendLine("\t\t\tTags:");
                    for (int j = 0; j < subModule.Tags.Length; j++)
                    {
                        string key = subModule.Tags[j];
                        if (++j > subModule.Tags.Length)
                        {
                            Log.LogError($"Invalid number of tag parameters for SubModule \"{subModuleName}\"");
                            break;
                        }
                        string value = subModule.Tags[j];
                        _ = Output.AppendLine($"\t\t\t\t<Tag key=\"{key}\" value=\"{value}\" />");
                        _ = Message.AppendLine($"\t\t\t\t{key} = {value}");
                    }
                    _ = Output.AppendLine("\t\t\t</Tags>");
                }*/
                _ = Output.AppendLine("\t\t</SubModule>");
            }
            _ = Output.AppendLine("\t</SubModules>");
        }
        string[] xmls = Xmls;
        if (xmls?.Length > 0)
        {
            _ = Output.AppendLine("\t<Xmls>");
            _ = Message.AppendLine("\tXmls:");
            for (int i = 0; i < xmls.Length; i++)
            {
                string xmlId = xmls[i];
                _ = Output.AppendLine("\t\t<XmlNode>");
                _ = Message.AppendLine($"\t\t{xmlId}:");
                if (++i > xmls.Length)
                {
                    Log.LogError($"Missing path for XmlNode \"{xmlId}\"");
                    break;
                }
                string xmlPath = xmls[i];
                _ = Message.AppendLine($"\t\t\tpath = {xmlPath}");
                _ = Output.AppendLine($"\t\t\t<XmlName id=\"{xmlId}\" path=\"{xmlPath}\" />");
                // NEED TO SUPPORT INCLUDEDGAMETYPES
                /*if (xml.IncludedGameTypes?.Length > 0)
                {
                    _ = Output.AppendLine("\t\t\t<IncludedGameTypes>");
                    _ = Message.AppendLine("\t\t\tIncludedGameTypes:");
                    foreach (string gameType in xml.IncludedGameTypes)
                    {
                        _ = Output.AppendLine($"\t\t\t\t<GameType value=\"{gameType}\" />");
                        _ = Message.AppendLine($"\t\t\t\t{gameType}");
                    }
                    _ = Output.AppendLine("\t\t\t</IncludedGameTypes>");
                }*/
                _ = Output.AppendLine("\t\t</XmlNode>");
            }
            _ = Output.AppendLine("\t</Xmls>");
        }
        _ = Output.Append("</Module>");
        if (Log.HasLoggedErrors)
            return false;
        File.WriteAllText(output, Output.ToString().Trim());
        Log.LogMessage(MessageImportance.High, Message.ToString().Trim());
        return true;
    }
}