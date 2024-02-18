
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
    public class DungeonTextureSmall
    {
        [StreamData(0)]
        public uint DBType { get; set; }

        [StreamData(1)]
        public uint AOID { get; set; }

        [StreamData(2)]
        public uint Version { get; set; }

        public byte[] JpegData { get; private set; }

        public DungeonTextureSmall() { }

        public void PopulateFromStream(BinaryReader reader)
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(StreamDataAttribute)))
                .OrderBy(p => p.GetCustomAttributes().OfType<StreamDataAttribute>().First().Index)
                .ToArray();
            
            foreach(PropertyInfo property in properties)
            {
                StreamDataAttribute attrib = (StreamDataAttribute)Attribute.GetCustomAttribute(property, typeof(StreamDataAttribute));

                if(attrib != null)
                {
                    property.SetValue(this, reader.ReadUInt32());
                }
            }
            long total = reader.BaseStream.Length;
            long pos = reader.BaseStream.Position;

            this.JpegData = reader.ReadBytes((int)(total - pos));
        }
    }
}