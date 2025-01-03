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
using AODb.Data.Attributes;

namespace AODb.Data.Spells
{

    [SpellId(53030)]
    public class GfxEffectSpell : Spell
    {
        // TODO: Verify this is correct.
        [StreamData(0)]
        public uint Value { get; set; }

        [StreamData(1)]
        public int GfxLife { get; set; }
        
        [StreamData(2)]
        public int GfxSize { get; set; }
        
        [StreamData(3)]
        public int GfxRed { get; set; }

        [StreamData(4)]
        public int GfxBlue { get; set; }

        [StreamData(5)]
        public int GfxGreen { get; set; }

        [StreamData(6)]
        public int GfxFade { get; set; }
    }
}
