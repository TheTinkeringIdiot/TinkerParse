using System;

namespace AODb.Common.Attributes;

public class StreamDataStringAttribute : Attribute
{
    public StringType Type { get; private set; }
    public StringEncoding Encoding { get; private set; }
    
    public StreamDataStringAttribute(StringType type, StringEncoding encoding)
    {
        Type = type;
        Encoding = encoding;
    }
}