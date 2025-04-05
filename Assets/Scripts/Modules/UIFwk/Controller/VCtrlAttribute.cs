using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class VCtrlAttribute : Attribute
{
    public string TYPE { get; }

    public VCtrlAttribute(string type)
    {
        TYPE = type;
    }
}