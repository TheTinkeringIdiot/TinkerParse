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
using System.Reflection.Metadata;
using AODb.Data;

namespace AODb.Data.TemplateData
{
    public class ShopHashData : TemplateDataBase
    {
        public List<ShopHashEntry> Items { get; private set; }
        public ShopHashData(TemplateDataBase source)
            : base(source)
        {
            //this.Items = new ShopHashEntry[this.Entries];
        }

        public ShopHashData()
        {
            this.Items = new List<ShopHashEntry>();
        }

        public void PopulateFromStream(BinaryReader reader)
        {
            int confirm = reader.ReadInt32();
            if(confirm != 37) { throw new Exception("CONFIRMATION CHECK FAILED!"); }

            int numHashes = reader.ReadInt32();
            numHashes = (numHashes / 1009) - 1;

            for(int i = 0; i < numHashes; i++)
            {
                ShopHashEntry hash = new ShopHashEntry();
                hash.PopulateFromStream(reader);
                this.Items.Add(hash);
            }

            // Funcom finds counting difficult so
            // catch any extra hashes they missed
            // bool actuallyDone = false;
            // while(!actuallyDone && reader.BaseStream.Position < reader.BaseStream.Length)
            // {
            //     byte key = reader.ReadByte();
            //     if(key > 0x23) 
            //     {
            //         reader.BaseStream.Position -= 1;
            //         ShopHashEntry hash = new ShopHashEntry();
            //         hash.PopulateFromStream(reader);
            //         this.Items.Add(hash);
            //     }
            //     else{
            //         reader.BaseStream.Position -= 1;
            //         actuallyDone = true;
            //     }
            // }
        }
    }
}
