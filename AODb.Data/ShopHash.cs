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

using System.IO;
using System.Linq;
using System.Text;

namespace AODb.Data
{
    public class ShopHashEntry
    {
        /// <summary>
        /// Cluster to spawn
        /// </summary>
        public Hash Hash { get; set; }
        /// <summary>
        /// Minimum level of item
        /// </summary>
        public short MinLevel { get; set; }
        /// <summary>
        /// Maximum level of item
        /// </summary>
        public short MaxLevel { get; set; }
        public byte BaseAmount { get; set; }
        public byte RegenAmount { get; set; }
        public short RegenInterval { get; set; }

        public byte UnknownOne { get; set; }
        public short UnknownZero { get; set; }

        /// <summary>
        /// Price/Cost modifier (percent?)
        /// </summary>
        /// <remarks>
        /// Was tagged as cost modifier, but as of 2013-02-19, it's tagged as SpawnChance.
        /// This because shop booths have a custom stat for CostModifier, and most items have a value of 100 here.
        /// </remarks>
        public int SpawnChance { get; set; }

        public void PopulateFromStream(BinaryReader reader)
        {
            byte[] hash = reader.ReadBytes(4);
            this.Hash = new Hash(Encoding.Default.GetString(hash.Reverse().ToArray()));
            int minlevel = reader.ReadByte();
            if(minlevel == 0)
            {
                //Min/Max level don't fit in a byte (QL256+), so are shorts
                minlevel = reader.ReadByte();
                this.MinLevel = reader.ReadInt16();
                this.MaxLevel = reader.ReadInt16();
            }
            else
            {
                this.MinLevel = (short)minlevel;
                this.MaxLevel = (short)reader.ReadByte();
            }

            this.BaseAmount = reader.ReadByte();
            this.RegenAmount = reader.ReadByte();
            this.RegenInterval = reader.ReadInt16();
            this.UnknownOne = reader.ReadByte();
            this.UnknownZero = reader.ReadInt16();
            this.SpawnChance = reader.ReadInt32();

        }
    }
}
