using System;

[Serializable]
public class Int
{
    public int value;
    public Int(int value)
    {
        this.value = value;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator int(Int i)
    {
        return i.value;
    }
}
[Serializable]
public class Bool
{
    public bool value;
    public Bool(bool value)
    {
        this.value = value;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator bool(Bool i)
    {
        return i.value;
    }
}
[Serializable]
public class Float
{
    public float value;
    public Float(float value)
    {
        this.value = value;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator float(Float i)
    {
        return i.value;
    }
}
[Serializable]
public class String
{
    public string value;
    public String(string value)
    {
        this.value = value;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator string(String i)
    {
        return i.value;
    }
}