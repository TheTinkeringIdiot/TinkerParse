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
using AODb.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace AODb.Data
{
    [Serializable]
    public class ModelData
    {

        public int DbType { get; set; }
        public int DbInstance { get; set; }
        public int DbVersion { get; set; }
        public List<ModelInfo> Instances;

        public ModelData()
        {
            Instances = new List<ModelInfo>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            this.DbType = reader.ReadInt32();
            this.DbInstance = reader.ReadInt32();
            this.DbVersion = reader.ReadInt32();

            int typeCount = reader.ReadInt32();
            for(int i = 0; i < typeCount; i++)
            {
                ModelInfo mi = new ModelInfo();
                mi.PopulateFromStream(reader);
                this.Instances.Add(mi);
            }
        }
    }
}
