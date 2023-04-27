using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ASMXML;

public class AutomaticSubModuleXML : Task
{
    private readonly StringBuilder Message = new();
    private readonly StringBuilder Output = new();

    [Required] public string Target { get; set; }

    private T LogGetAttribute<T>(Assembly assembly) where T : Attribute
    {
        T attribute = assembly.GetCustomAttribute<T>();
        if (attribute is not null)
            return attribute;
        Log.LogError($"Missing assembly attribute \"{typeof(T).Name}\"");
        return null;
    }

    public override bool Execute()
    {
        Assembly assembly = Assembly.Load(File.ReadAllBytes(Target));
        string output = Path.GetFullPath(Path.GetDirectoryName(Target) + @"\..\..\") + @"SubModule.xml";
        _ = Message.AppendLine($"SubModule -> {output}");
        _ = Output.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        _ = Output.AppendLine("<Module>");
        string id = LogGetAttribute<ModuleId>(assembly)?.Value;
        _ = Output.AppendLine($"\t<Id value=\"{id}\" />");
        _ = Message.AppendLine($"\tId = {id}");
        string name = LogGetAttribute<ModuleName>(assembly)?.Value;
        _ = Output.AppendLine($"\t<Name value=\"{name}\" />");
        _ = Message.AppendLine($"\tName = {name}");
        string version = LogGetAttribute<ModuleVersion>(assembly)?.Value;
        _ = Output.AppendLine($"\t<Version value=\"{version}\" />");
        _ = Message.AppendLine($"\tVersion = {version}");
        string @default = (LogGetAttribute<ModuleDefault>(assembly)?.Value ?? false).ToString().ToLower();
        _ = Output.AppendLine($"\t<DefaultModule value=\"{@default}\" />");
        _ = Message.AppendLine($"\tDefaultModule = {@default}");
        string category = LogGetAttribute<ModuleCategory>(assembly)?.Value;
        _ = Output.AppendLine($"\t<ModuleCategory value=\"{category}\" />");
        _ = Message.AppendLine($"\tModuleCategory = {category}");
        string type = LogGetAttribute<ModuleType>(assembly)?.Value;
        _ = Output.AppendLine($"\t<ModuleType value=\"{type}\" />");
        _ = Message.AppendLine($"\tModuleType = {type}");
        List<ModuleDependedModule> dependencies = assembly.GetCustomAttributes<ModuleDependedModule>().ToList();
        if (dependencies.Count > 0)
        {
            _ = Output.AppendLine("\t<DependedModules>");
            _ = Message.AppendLine("\tDependedModules:");
            foreach (ModuleDependedModule dependency in dependencies)
            {
                _ = Output.Append($"\t\t<DependedModule Id=\"{dependency.Id}\"");
                _ = Message.Append($"\t\t{dependency.Id}");
                if (dependency.Version is not null)
                {
                    _ = Output.Append($" DependentVersion=\"{dependency.Version}\"");
                    _ = Message.Append($" >= {dependency.Version}");
                }
                _ = Output.Append($" Optional=\"{dependency.Optional.ToString().ToLower()}\"");
                if (dependency.Optional)
                    _ = Message.Append(" (optional)");
                _ = Output.AppendLine(" />");
                _ = Message.AppendLine();
            }
            _ = Output.AppendLine("\t</DependedModules>");
        }
        List<ModuleModulesToLoadAfterThis> modulesToLoadAfterThis = assembly.GetCustomAttributes<ModuleModulesToLoadAfterThis>().ToList();
        if (modulesToLoadAfterThis.Count > 0)
        {
            _ = Output.AppendLine("\t<ModulesToLoadAfterThis>");
            _ = Message.AppendLine("\tModulesToLoadAfterThis:");
            foreach (ModuleModulesToLoadAfterThis moduleToLoadAfterThis in modulesToLoadAfterThis)
            {
                _ = Output.AppendLine($"\t\t<Module Id=\"{moduleToLoadAfterThis.Id}\" />");
                _ = Message.AppendLine($"\t\t{moduleToLoadAfterThis.Id}");
            }
            _ = Output.AppendLine("\t</ModulesToLoadAfterThis>");
        }
        List<ModuleIncompatibleModule> incompatibilities = assembly.GetCustomAttributes<ModuleIncompatibleModule>().ToList();
        if (incompatibilities.Count > 0)
        {
            _ = Output.AppendLine("\t<IncompatibleModules>");
            _ = Message.AppendLine("\tIncompatibleModules:");
            foreach (ModuleIncompatibleModule incompatibility in incompatibilities)
            {
                _ = Output.AppendLine($"\t\t<Module Id=\"{incompatibility.Id}\" />");
                _ = Message.AppendLine($"\t\t{incompatibility.Id}");
            }
            _ = Output.AppendLine("\t</IncompatibleModules>");
        }
        List<ModuleSubModule> subModules = assembly.GetCustomAttributes<ModuleSubModule>().ToList();
        if (subModules.Count > 0)
        {
            _ = Output.AppendLine("\t<SubModules>");
            _ = Message.AppendLine("\tSubModules:");
            foreach (ModuleSubModule subModule in subModules)
            {
                _ = Output.AppendLine("\t\t<SubModule>");
                _ = Message.AppendLine($"\t\t{subModule.Name}:");
                _ = Output.AppendLine($"\t\t\t<Name value=\"{subModule.Name}\" />");
                _ = Output.AppendLine($"\t\t\t<DLLName value=\"{subModule.DLLName}\" />");
                _ = Message.AppendLine($"\t\t\tDLLName = {subModule.DLLName}");
                _ = Output.AppendLine($"\t\t\t<SubModuleClassType value=\"{subModule.SubModuleClassType}\" />");
                _ = Message.AppendLine($"\t\t\tSubModuleClassType = {subModule.SubModuleClassType}");
                if (subModule.Tags?.Length > 0)
                {
                    _ = Output.AppendLine("\t\t\t<Tags>");
                    _ = Message.AppendLine("\t\t\tTags:");
                    for (int i = 0; i < subModule.Tags.Length; i++)
                    {
                        string key = subModule.Tags[i];
                        if (++i > subModule.Tags.Length)
                        {
                            Log.LogError($"Invalid number of tag parameters for SubModule \"{subModule.Name}\"");
                            break;
                        }
                        string value = subModule.Tags[i];
                        _ = Output.AppendLine($"\t\t\t\t<Tag key=\"{key}\" value=\"{value}\" />");
                        _ = Message.AppendLine($"\t\t\t\t{key} = {value}");
                    }
                    _ = Output.AppendLine("\t\t\t</Tags>");
                }
                _ = Output.AppendLine("\t\t</SubModule>");
            }
            _ = Output.AppendLine("\t</SubModules>");
        }
        List<ModuleXml> xmls = assembly.GetCustomAttributes<ModuleXml>().ToList();
        if (xmls.Count > 0)
        {
            _ = Output.AppendLine("\t<Xmls>");
            _ = Message.AppendLine("\tXmls:");
            foreach (ModuleXml xml in xmls)
            {
                _ = Output.AppendLine("\t\t<XmlNode>");
                _ = Message.AppendLine($"\t\t{xml.Id}:");
                _ = Message.AppendLine($"\t\t\tPath = {xml.Path}");
                _ = Output.AppendLine($"\t\t\t<XmlName id=\"{xml.Id}\" path=\"{xml.Path}\" />");
                if (xml.IncludedGameTypes?.Length > 0)
                {
                    _ = Output.AppendLine("\t\t\t<IncludedGameTypes>");
                    _ = Message.AppendLine("\t\t\tIncludedGameTypes:");
                    foreach (string gameType in xml.IncludedGameTypes)
                    {
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