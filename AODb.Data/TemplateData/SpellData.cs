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
using AODb.Data.Attributes;

namespace AODb.Data.TemplateData
{
    public class SpellData : TemplateDataBase
    {
        public List<Spell> Items { get; set; }
        // public TemplateEvent Event { get { return (TemplateEvent)this.Identity.Instance; } }
        public TemplateEvent Event { get; set; }

        public SpellData(TemplateDataBase source)
            : base(source) { }

        public SpellData()
        {
            this.Items = new List<Spell>();
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} spells.", this.Event, this.Entries);
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            this.Event = (TemplateEvent)reader.ReadInt32();

            uint spellCount = reader.ReadUInt32();
            spellCount = (spellCount / 1009) - 1;

            for(int i = 0; i < spellCount; i++)
            {
                int spellKey = 0;
                while(spellKey == 0) { spellKey = reader.ReadInt32(); }

                Spell spell = CreateSpellWithKey(spellKey);
                if(spell != null)
                {
                    spell.PopulateFromStream(reader);
                    this.Items.Add(spell);
                }
            }

            // Funcom finds counting difficult so
            // catch any extra spells they missed
            // bool actuallyDone = false;

            // while(!actuallyDone && reader.BaseStream.Position < reader.BaseStream.Length)
            // {
            //     int key = reader.ReadInt32();
            //     if((key & 0xCF00) == 0xCF00) 
            //     {
            //         Spell spell = CreateSpellWithKey(key);
            //         if(spell != null)
            //         {
            //             spell.PopulateFromStream(reader);
            //             this.Items.Add(spell);
            //         }
            //     }
            //     else{
            //         reader.BaseStream.Position -= 4;
            //         actuallyDone = true;
            //     }
            // }
        }

        private Spell CreateSpellWithKey(int spellKey)
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            var targetType = types.FirstOrDefault(type =>
            {
                var attribute = (SpellIdAttribute)Attribute.GetCustomAttribute(type, typeof(SpellIdAttribute));
                return attribute != null && attribute.ID == spellKey;
            });

            if(targetType != null)
            {
                return (Spell)Activator.CreateInstance(targetType);
            }
            else { return null; }
        }
    }
}
