using System;

namespace ASMXML;

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleId : Attribute
{
    public ModuleId(string id) => Value = id;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleName : Attribute
{
    public ModuleName(string name) => Value = name;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleVersion : Attribute
{
    public ModuleVersion(string version) => Value = version;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleDefault : Attribute
{
    public ModuleDefault(bool @default) => Value = @default;

    public bool Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleCategory : Attribute
{
    public ModuleCategory(string value) => Value = value;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleType : Attribute
{
    public ModuleType(string value) => Value = value;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleDependedModule : Attribute
{
    public ModuleDependedModule(string id, string version = null, bool optional = false)
    {
        Id = id;
        Optional = optional;
        Version = version;
    }

    public string Id { get; }
    public bool Optional { get; }
    public string Version { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleModulesToLoadAfterThis : Attribute
{
    public ModuleModulesToLoadAfterThis(string id) => Id = id;

    public string Id { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleIncompatibleModule : Attribute
{
    public ModuleIncompatibleModule(string id) => Id = id;

    public string Id { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleSubModule : Attribute
{
    public ModuleSubModule(string name, string dllName, string subModuleClassType, string[] tags = null)
    {
        Name = name;
        DLLName = dllName;
        SubModuleClassType = subModuleClassType;
        Tags = tags;
    }

    public string Name { get; }
    public string DLLName { get; }
    public string SubModuleClassType { get; }
    public string[] Tags { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleXml : Attribute
{
    public ModuleXml(string id, string path, string[] includedGameTypes = null)
    {
        Id = id;
        Path = path;
        IncludedGameTypes = includedGameTypes;
    }

    public string Id { get; }
    public string Path { get; }
    public string[] IncludedGameTypes { get; }
}