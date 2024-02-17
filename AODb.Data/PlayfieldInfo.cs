﻿/*
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

using AODb.Common.Attributes;

namespace AODb.Data
{
    public class PlayfieldInfo : Base
    {
        public int UnknownA { get; set; }
        public int Resource { get; set; }
        public string Name { get; set; }
        public byte[] UnknownB { get; set; }
    }

    public class PlayfieldDistrict
    {
        [StreamData(0)]
        public float X { get; set; }
        
        [StreamData(1)]
        public float Y { get; set; }
        
        [StreamData(2)]
        public float Z { get; set; }

        // UTF-16 characters I am assuming?
        [StreamData(3)]
        [StreamDataString(StringType.Normal, StringEncoding.ASCII)]
        [StreamDataLength(LengthType.UInt16)]
        public string Name { get; set; }

        [StreamData(4, Entries = 9)]
        public ushort[] MusicId { get; set; }

        /// <summary>
        /// Used by autogenerated missions?
        /// </summary>
        [StreamData(10)]
        public ushort NpcMinLevel { get; set; }

        /// <summary>
        /// Used by autogenerated missions?
        /// </summary>
        [StreamData(11)]
        public ushort NpcMaxLevel { get; set; }

        [StreamData(12)]
        public ushort LcaMinLevel { get; set; }
        
        [StreamData(13)]
        public ushort LcaMaxLevel { get; set; }

        [StreamData(14)]
        public byte RespawnChance { get; set; }

        [StreamData(15)]
        public int RespawnInterval { get; set; }

        [StreamData(16)]
        public byte SuppressionGas { get; set; }

        [StreamData(20)]
        public byte RandomSpawns { get; set; }

        [StreamData(21)]
        public byte WildlifeSpawns { get; set; }
        
        [StreamData(22)]
        public byte RadiusSpawns { get; set; }

        [StreamData(23)]
        public byte StaticSpawns { get; set; }
    }
}