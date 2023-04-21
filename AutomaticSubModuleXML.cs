using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Bannerlord.AutomaticSubModuleXML;

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
        _ = Output.AppendLine(
            "<Module xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"https://raw.githubusercontent.com/BUTR/Bannerlord.XmlSchemas/master/SubModule.xsd\">");
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
        _ = Message.AppendLine($"\tDefault = {@default}");
        string category = LogGetAttribute<ModuleCategory>(assembly)?.Value.ToString();
        _ = Output.AppendLine($"\t<ModuleCategory value=\"{category}\" />");
        _ = Message.AppendLine($"\tCategory = {category}");
        string type = LogGetAttribute<ModuleType>(assembly)?.Value.ToString();
        _ = Output.AppendLine($"\t<ModuleType value=\"{type}\" />");
        _ = Message.AppendLine($"\tType = {type}");
        string url = LogGetAttribute<ModuleUrl>(assembly)?.Value;
        _ = Output.AppendLine($"\t<Url value=\"{url}\" />");
        _ = Message.AppendLine($"\tUrl = {url}");
        List<ModuleDependency> dependencies = assembly.GetCustomAttributes<ModuleDependency>().ToList();
        if (dependencies.Count == 0)
            _ = Output.AppendLine("\t<DependedModules />");
        else
        {
            _ = Output.AppendLine("\t<DependedModules>");
            _ = Message.AppendLine("\tDependencies:");
            foreach (ModuleDependency dependency in dependencies)
            {
                _ = Output.Append($"\t\t<DependedModule Id=\"{dependency.Id}\"");
                _ = Message.Append($"\t\t{dependency.Id}");
                if (dependency.Version is not null)
                {
                    _ = Output.Append($" DependentVersion=\"{dependency.Version}\"");
                    _ = Message.Append($" >= {dependency.Version}");
                }
                if (dependency.Optional)
                {
                    _ = Output.Append($" Optional=\"{dependency.Optional.ToString().ToLower()}\"");
                    _ = Message.Append(" (optional)");
                }
                _ = Output.AppendLine(" />");
                _ = Message.AppendLine();
            }
            _ = Output.AppendLine("\t</DependedModules>");
        }
        List<ModuleSubModule> subModules = assembly.GetCustomAttributes<ModuleSubModule>().ToList();
        if (subModules.Count == 0)
            _ = Output.AppendLine("\t<SubModules />");
        else
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
                if (subModule.Tags.Length == 0)
                    _ = Output.AppendLine("\t\t\t<Tags />");
                else
                {
                    _ = Output.AppendLine("\t\t\t<Tags>");
                    _ = Message.AppendLine("\t\t\tTags:");
                    for (int i = 0; i < subModule.Tags.Length; i++)
                    {
                        string key = subModule.Tags[i];
                        if (++i > subModule.Tags.Length)
                        {
                            Log.LogError($"Incorrect number of tags for SubModule \"{subModule.Name}\"");
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
        _ = Output.Append("</Module>");
        if (Log.HasLoggedErrors)
            return false;
        File.WriteAllText(output, Output.ToString());
        Log.LogMessage(MessageImportance.High, Message.ToString());
        return true;
    }
}