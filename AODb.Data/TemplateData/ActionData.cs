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

namespace AODb.Data.TemplateData
{
    public class ActionData
    {
        public List<TemplateActionData> Actions { get; set; }

        // public ActionData(TemplateDataBase source)
        //     : base(source)
        // {
        //     this.Actions = new List<TemplateActionData>();
        // }

        public ActionData()
        {
            this.Actions = new List<TemplateActionData>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            int blockConfirm = reader.ReadInt32();
            if(blockConfirm != 0x24) { throw new System.Exception("Confirmation Check Failed!"); }

            int actionDataCount = reader.ReadInt32();
            actionDataCount = (actionDataCount / 1009) - 1;

            for(int i = 0; i < actionDataCount; i++)
            {
                TemplateActionData actionData = new TemplateActionData();
                actionData.PopulateFromStream(reader);
                Actions.Add(actionData);
            }
        }
    }
}
