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
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AODb.Common.Attributes;
namespace AODb.Data
{
    public class ModelInfo
    {

        public int TypeId { get; set; }
        public Dictionary<int, string> Instances;
        public ModelInfo()
        {
            Instances = new Dictionary<int, string>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            this.TypeId = reader.ReadInt32();
            int instanceCount = reader.ReadInt32();
            for(int i = 0; i < instanceCount; i++)
            {
                int instanceId = reader.ReadInt32();
                int nameLen = reader.ReadInt32();
                byte[] nameBytes = reader.ReadBytes(nameLen);
                string name = Encoding.Default.GetString(nameBytes);

                this.Instances.Add(instanceId, name);
            }

        }
    }
}
