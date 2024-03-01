/*
 * Demoder.AnarchyData
 *
 * Copyright (c) 2012-2016 Marie Helene Kvello-Aune 
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either 
 * version 3 of the License.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public 
 * License along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using AODb.Common.Attributes;
using Newtonsoft.Json.Converters;
using System.Data;

namespace AODb.Data
{
    [Serializable]
    public class Criterion
    {
        /// <summary>
        /// This is Stat enum for Operator.Stat*, but other things for other operators.
        /// </summary>
        [StreamData(100)]  
        //[JsonConverter(typeof(TryStringEnumConverter))]
        public int Value1 { get; set; }

        /// <summary>
        /// This is the amount of skill for Operator.Stat*, but other things for other operators.
        /// </summary>
        [StreamData(200)]
        public int Value2 { get; set; }

        [StreamData(300)]
        //[JsonConverter(typeof(StringEnumConverter))]
        public Operator Operator { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", this.Value1, this.Value2, this.Operator);
        }

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
                    if(property.PropertyType == typeof(Operator)) { property.SetValue(this, (Operator)reader.ReadInt32()); }
                    else { property.SetValue(this, reader.ReadInt32()); }        
                }
            }
        }

        public class TryStringEnumConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Criterion values are usually stat values, but not always 
                // try converting Stat and fail back to int if it's not a stat
                uint val = (uint)(int)value;
                if(Enum.IsDefined(typeof(Stat), val))
                {
                    Stat val1 = (Stat)val;
                    writer.WriteValue(val1.ToString());
                }
                else
                {
                    writer.WriteValue(value);
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                throw new NotImplementedException();
            }
        }
    }
}
