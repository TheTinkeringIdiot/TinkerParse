
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
    public class Texture : TextureBase
    {
        public Texture() { }

        public override void PopulateFromStream(BinaryReader reader)
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

            byte peek = reader.ReadByte();
            reader.BaseStream.Position -= 1;
            if(peek != 0xff && peek != 0x89)
            {
                //this is a ground texture, it has a name
                string fullName = Encoding.Default.GetString(reader.ReadBytes(24));
                this.Name = fullName.Substring(0, fullName.IndexOf('\x00'));
            }

            long total = reader.BaseStream.Length;
            long pos = reader.BaseStream.Position;

            this.ImgData = reader.ReadBytes((int)(total - pos));
        }
    }
}