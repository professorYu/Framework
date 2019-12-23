using System;

[AttributeUsage(AttributeTargets.Class)]
public class MonoSingletonPath : Attribute
{
    public string PathInHierarchy { get; }

    public MonoSingletonPath(string pathInHierarchy)
    {
        PathInHierarchy = pathInHierarchy;
    }
}
