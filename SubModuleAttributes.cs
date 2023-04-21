using System;

namespace Bannerlord.AutomaticSubModuleXML;

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

public enum ModuleCategoryValue { Singleplayer, Multiplayer, Server }

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleCategory : Attribute
{
    public ModuleCategory(ModuleCategoryValue value) => Value = value;

    public ModuleCategoryValue Value { get; }
}

public enum ModuleTypeValue { Official, OfficialOptional, Community }

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleType : Attribute
{
    public ModuleType(ModuleTypeValue value) => Value = value;

    public ModuleTypeValue Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleUrl : Attribute
{
    public ModuleUrl(string url) => Value = url;

    public string Value { get; }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleDependency : Attribute
{
    public ModuleDependency(string id, string version = null, bool optional = false)
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