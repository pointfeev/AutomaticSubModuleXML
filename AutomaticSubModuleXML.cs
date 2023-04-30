using System.IO;
using System.Text;
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
        string[] dependedModules = DependedModules;
        if (dependedModules?.Length > 0)
        {
            _ = Output.AppendLine("\t<DependedModules>");
            _ = Message.AppendLine("\tDependedModules:");
            for (int dependedModuleIndex = 0; dependedModuleIndex < dependedModules.Length; dependedModuleIndex++)
            {
                if (string.IsNullOrWhiteSpace(dependedModules[dependedModuleIndex].Trim().Trim(','))
                 || dependedModules[dependedModuleIndex].Split(',') is not { Length: > 0 } dependedModule)
                {
                    Log.LogError($"Empty DependedModule at index {dependedModuleIndex}");
                    continue;
                }
                string dependencyId = dependedModule[0].Trim();
                if (string.IsNullOrWhiteSpace(dependencyId))
                {
                    Log.LogError($"Empty id for DependedModule at index {dependedModuleIndex}");
                    continue;
                }
                _ = Output.Append($"\t\t<DependedModule Id=\"{dependencyId}\"");
                _ = Message.Append($"\t\t{dependencyId}");
                if (dependedModule.Length > 1)
                {
                    string dependencyVersion = dependedModule[1].Trim();
                    if (string.IsNullOrWhiteSpace(dependencyVersion))
                    {
                        Log.LogError($"Empty DependentVersion for DependedModule \"{dependencyId}\" at index {dependedModuleIndex}");
                        continue;
                    }
                    _ = Output.Append($" DependentVersion=\"{dependencyVersion}\"");
                    _ = Message.Append($" >= {dependencyVersion}");
                }
                bool dependencyOptional = false;
                if (dependedModule.Length > 2)
                {
                    string dependencyOptionalString = dependedModule[2].Trim();
                    if (!bool.TryParse(dependencyOptionalString, out dependencyOptional))
                    {
                        Log.LogError(
                            $"Unable to parse Optional bool from \'{dependencyOptionalString}\' for DependedModule \"{dependencyId}\" at index {dependedModuleIndex}");
                        continue;
                    }
                }
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
            for (int subModuleIndex = 0; subModuleIndex < subModules.Length; subModuleIndex++)
            {
                if (string.IsNullOrWhiteSpace(subModules[subModuleIndex].Trim().Trim(','))
                 || subModules[subModuleIndex].Split(',') is not { Length: > 0 } subModule)
                {
                    Log.LogError($"Empty SubModule at index {subModuleIndex}");
                    continue;
                }
                string subModuleName = subModule[0].Trim();
                if (string.IsNullOrWhiteSpace(subModuleName))
                {
                    Log.LogError($"Empty name for SubModule at index {subModuleIndex}");
                    continue;
                }
                _ = Output.AppendLine("\t\t<SubModule>");
                _ = Message.AppendLine($"\t\t{subModuleName}:");
                _ = Output.AppendLine($"\t\t\t<Name value=\"{subModuleName}\" />");
                if (subModule.Length < 2)
                {
                    Log.LogError($"Missing DLLName for SubModule \"{subModuleName}\"");
                    continue;
                }
                string subModuleDllName = subModule[1].Trim();
                if (string.IsNullOrWhiteSpace(subModuleDllName))
                {
                    Log.LogError($"Empty DLLName for SubModule \"{subModuleName}\"");
                    continue;
                }
                _ = Output.AppendLine($"\t\t\t<DLLName value=\"{subModuleDllName}\" />");
                _ = Message.AppendLine($"\t\t\tDLLName = {subModuleDllName}");
                if (subModule.Length < 3)
                {
                    Log.LogError($"Missing SubModuleClassType for SubModule \"{subModuleName}\"");
                    continue;
                }
                string subModuleClassType = subModule[2].Trim();
                if (string.IsNullOrWhiteSpace(subModuleClassType))
                {
                    Log.LogError($"Empty SubModuleClassType for SubModule \"{subModuleName}\"");
                    continue;
                }
                _ = Output.AppendLine($"\t\t\t<SubModuleClassType value=\"{subModuleClassType}\" />");
                _ = Message.AppendLine($"\t\t\tSubModuleClassType = {subModuleClassType}");
                if (subModule.Length > 3)
                {
                    _ = Output.AppendLine("\t\t\t<Tags>");
                    _ = Message.AppendLine("\t\t\tTags:");
                    for (int tagIndex = 3; tagIndex < subModule.Length; tagIndex++)
                    {
                        if (string.IsNullOrWhiteSpace(subModule[tagIndex].Trim().Trim(':')) || subModule[tagIndex].Split(':') is not { Length: > 0 } tag)
                        {
                            Log.LogError($"Empty tag at index {tagIndex} for SubModule \"{subModuleName}\"");
                            continue;
                        }
                        string tagKey = tag[0].Trim();
                        if (string.IsNullOrWhiteSpace(tagKey))
                        {
                            Log.LogError($"Empty key for tag at index {tagIndex} for SubModule \"{subModuleName}\"");
                            continue;
                        }
                        if (tag.Length < 2)
                        {
                            Log.LogError($"Missing value for tag \"{tagKey}\" for SubModule \"{subModuleName}\"");
                            break;
                        }
                        string tagValue = tag[1].Trim();
                        if (string.IsNullOrWhiteSpace(tagValue))
                        {
                            Log.LogError($"Empty value for tag \"{tagKey}\" for SubModule \"{subModuleName}\"");
                            continue;
                        }
                        _ = Output.AppendLine($"\t\t\t\t<Tag key=\"{tagKey}\" value=\"{tagValue}\" />");
                        _ = Message.AppendLine($"\t\t\t\t{tagKey} = {tagValue}");
                    }
                    _ = Output.AppendLine("\t\t\t</Tags>");
                }
                _ = Output.AppendLine("\t\t</SubModule>");
            }
            _ = Output.AppendLine("\t</SubModules>");
        }
        string[] xmls = Xmls;
        if (xmls?.Length > 0)
        {
            _ = Output.AppendLine("\t<Xmls>");
            _ = Message.AppendLine("\tXmls:");
            for (int xmlIndex = 0; xmlIndex < xmls.Length; xmlIndex++)
            {
                if (string.IsNullOrWhiteSpace(xmls[xmlIndex].Trim().Trim(',')) || xmls[xmlIndex].Split(',') is not { Length: > 0 } xmlNode)
                {
                    Log.LogError($"Empty XmlNode at index {xmlIndex}");
                    continue;
                }
                string xmlNodeId = xmlNode[0].Trim();
                if (string.IsNullOrWhiteSpace(xmlNodeId))
                {
                    Log.LogError($"Empty id for XmlNode at index {xmlIndex}");
                    continue;
                }
                _ = Output.AppendLine("\t\t<XmlNode>");
                _ = Message.AppendLine($"\t\t{xmlNodeId}:");
                if (xmlNode.Length < 2)
                {
                    Log.LogError($"Missing path for XmlNode \"{xmlNodeId}\"");
                    continue;
                }
                string xmlNodePath = xmlNode[1].Trim();
                if (string.IsNullOrWhiteSpace(xmlNodePath))
                {
                    Log.LogError($"Empty path for XmlNode \"{xmlNodeId}\"");
                    continue;
                }
                _ = Message.AppendLine($"\t\t\tpath = {xmlNodePath}");
                _ = Output.AppendLine($"\t\t\t<XmlName id=\"{xmlNodeId}\" path=\"{xmlNodePath}\" />");
                if (xmlNode.Length > 2)
                {
                    _ = Output.AppendLine("\t\t\t<IncludedGameTypes>");
                    _ = Message.AppendLine("\t\t\tIncludedGameTypes:");
                    for (int gameTypeIndex = 2; gameTypeIndex < xmlNode.Length; gameTypeIndex++)
                    {
                        string gameType = xmlNode[gameTypeIndex].Trim();
                        if (string.IsNullOrWhiteSpace(gameType))
                        {
                            Log.LogError($"Empty GameType at index {gameTypeIndex} for XmlNode \"{xmlNodeId}\"");
                            continue;
                        }
                        _ = Output.AppendLine($"\t\t\t\t<GameType value=\"{gameType}\" />");
                        _ = Message.AppendLine($"\t\t\t\t{gameType}");
                    }
                    _ = Output.AppendLine("\t\t\t</IncludedGameTypes>");
                }
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