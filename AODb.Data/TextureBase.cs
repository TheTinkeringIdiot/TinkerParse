
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AODb.Common;
using AODb.Common.Attributes;
using AODb.Data.TemplateData;

namespace AODb.Data
{
    public abstract class TextureBase
    {
        [StreamData(0)]
        public uint DBType { get; set; }

        [StreamData(1)]
        public uint AOID { get; set; }

        [StreamData(2)]
        public uint Version { get; set; }

        public byte[] ImgData { get; set; }

        public string Name { get; set; }

        public abstract void PopulateFromStream(BinaryReader reader);
        
    }
}