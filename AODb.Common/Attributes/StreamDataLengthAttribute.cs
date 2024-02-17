using System;

namespace AODb.Common.Attributes;

public class StreamDataLengthAttribute : Attribute
{
    public LengthType Type { get; private set; }

    public StreamDataLengthAttribute(LengthType type)
    {
        Type = type;
    }
}