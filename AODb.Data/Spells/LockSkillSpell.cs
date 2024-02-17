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
    [SpellId(53033)]
    [SpellFormat("Lock skill {Skill} for {Duration}.")]
    public class LockSkillSpell : Spell
    {
        [StreamData(0)]
        public int A { get; set; }

        [StreamData(1)]
        public Stat Skill { get; set; }

        [StreamData(2)]
        [Interpolate]
        [DurationParameter]
        public int Duration { get; set; }
    }
}