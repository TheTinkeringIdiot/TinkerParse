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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AODb.Common.Attributes;

namespace AODb.Data
{
    /// <summary>
    /// Type is generally refered to as spell ID in other RDB parsers.
    /// Instance seems to always be 0
    /// </summary>
    [Serializable]
    public class Spell : Base
    {
        #region Header info
        /*
         * Observation:
         * Up to and including 18.4.15, spells have all changed versions at the same time.
         * Here's a list of the first client version that a spell version was observed:
         * 1: 
         * 2: 
         * 3: 
         * 4: 17.01.01
         */
        /// <summary>
        /// Spell version
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Metadata stored from spell definitions.
        /// </summary>
        public Version Patch { get; set; }
        #endregion

        /// <summary>
        /// Requirements for the spell (if any)
        /// </summary>
        public List<Criterion> Criteria { get; set; }

        #region Activation info
        /// <summary>
        /// The target in which this function is applied to. TODO: Convert to enum
        /// </summary>
        public Target Target { get; set; }

        /// <summary>
        /// How many times this function occurs on the target
        /// </summary>
        public int TickCount { get; set; }

        /// <summary>
        /// Interval between ticks in milliseconds
        /// </summary>
        public int TickInterval { get; set; }
        #endregion

        /*
         * 18.04.15
         * Is 9 for everything except FearSpell and StunSpell.
         * These two spells have varying numbers here.
         * 
         */
        public int Unknown2 { get; set; }

        public Spell()
        {
            this.Criteria = new List<Criterion>();
        }

        public override string ToString()
        {
            return this.Identity.ToString();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            int confirm = reader.ReadInt32();
            if(confirm != 0) { throw new Exception("CONFIRMATION CHECK FAILED!"); }
            confirm = reader.ReadInt32();
            if(confirm != 4) { throw new Exception("CONFIRMATION CHECK FAILED!"); }

            int criteriaCount = reader.ReadInt32();
            if(criteriaCount > 0)
            {
                for(int i = 0; i < criteriaCount; i++)
                {
                    Criterion crit = new Criterion();
                    crit.PopulateFromStream(reader);
                    this.Criteria.Add(crit);
                }
            }

            this.TickCount = reader.ReadInt32();
            this.TickInterval = reader.ReadInt32();
            this.Target = (Target)reader.ReadInt32();

            confirm = reader.ReadInt32();
            if(confirm != 9) { throw new Exception("CONFIRMATION CHECK FAILED!"); }

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
                    if(property.PropertyType == typeof(UInt32)) { property.SetValue(this, reader.ReadUInt32()); }
                    else if (property.PropertyType == typeof(Int32)) { property.SetValue(this, reader.ReadInt32()); } 
                    else if (property.PropertyType == typeof(Stat)) { property.SetValue(this, (Stat)reader.ReadInt32()); } 
                    else if (property.PropertyType == typeof(TextureLocation)) { property.SetValue(this, (TextureLocation)reader.ReadInt32()); } 
                    else if (property.PropertyType == typeof(string)) 
                    { 
                        int strLen = reader.ReadInt32();
                        property.SetValue(this, Encoding.Default.GetString(reader.ReadBytes(strLen))); 
                    }
                    else if (property.PropertyType == typeof(Hash)) 
                    { 
                        property.SetValue(this, new Hash(Encoding.Default.GetString(reader.ReadBytes(4)))); 
                    }
                    else if (property.PropertyType == typeof(ActionFlag)) { property.SetValue(this, (ActionFlag)reader.ReadUInt32()); } 
                    else if (property.PropertyType == typeof(MonsterShape)) { property.SetValue(this, (MonsterShape)reader.ReadUInt32()); }
                    else if (property.PropertyType == typeof(Target)) { property.SetValue(this, (Target)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(EndFightModifier)) { property.SetValue(this, (EndFightModifier)reader.ReadUInt32()); }
                    else if (property.PropertyType == typeof(NanoSchool)) { property.SetValue(this, (NanoSchool)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(BitFlag)) { property.SetValue(this, (BitFlag)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(NpcAction)) { property.SetValue(this, (NpcAction)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(AoEntity)) { property.SetValue(this, (AoEntity)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(Breed)) { property.SetValue(this, (Breed)reader.ReadInt32()); }
                    else if (property.PropertyType == typeof(Gender)) { property.SetValue(this, (Gender)reader.ReadInt32()); }
                    else { throw new Exception($"Unhandled property type: {property.PropertyType}"); }
                }
            }
        }
    }
}

