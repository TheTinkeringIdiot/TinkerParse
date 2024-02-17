using System;

namespace AODb.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class StreamDataAttribute : Attribute
{
    // ReadType
    // Entries
    
    public uint Index { get; private set; }
    public uint Entries { get; set; }
    public Type ReadType { get; set; }
    
    public StreamDataAttribute(uint index)
    {
        Index = index;
    }
}