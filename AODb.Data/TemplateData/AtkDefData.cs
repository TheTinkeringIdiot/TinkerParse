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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AODb.Data.TemplateData
{
    public class AtkDefData : TemplateDataBase
    {
        public List<StatValue> Attack { get; private set; }
        public List<StatValue> Defense { get; private set; }

        public AtkDefData(TemplateDataBase source)
            : base(source)
        {
            this.Attack = new List<StatValue>();
            this.Defense = new List<StatValue>();
        }

        public AtkDefData()
        {
            this.Attack = new List<StatValue>();
            this.Defense = new List<StatValue>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            int mapConfirm = reader.ReadInt32();
            if(!(mapConfirm == 0x04)) { throw new System.Exception("Confirmation Check Failed!"); }
            int numMaps = reader.ReadInt32();
            numMaps = (numMaps / 1009) - 1;

            for(int i = 0; i < numMaps; i++) {
                int mapType = reader.ReadInt32();
                int itemCount = reader.ReadInt32();
                itemCount = (itemCount / 1009) - 1;

                for(int j = 0; j < itemCount; j++) {
                    StatValue sv = new StatValue();
                    sv.PopulateFromStream(reader);
                    if(mapType == 0x0d) { this.Defense.Add(sv); }
                    else if(mapType == 0x0c) { this.Attack.Add(sv); }
                }
            }
        }
    }
}
