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
using System.Runtime.InteropServices.Marshalling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AODb.Data
{
    public class TemplateActionData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateAction Action { get; set; }
        public List<Criterion> Criteria { get; set; }

        public TemplateActionData()
        {
            this.Criteria = new List<Criterion>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            this.Action = (TemplateAction)reader.ReadInt32();
            
            int numCriteria = reader.ReadInt32();
            numCriteria = (numCriteria / 1009) -1;

            for(int i = 0; i < numCriteria; i++)
            {
                Criterion crit = new Criterion();
                crit.PopulateFromStream(reader);
                this.Criteria.Add(crit);
            }
        }
    }
}
