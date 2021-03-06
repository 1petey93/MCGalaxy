﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
/*
This generator was developed by Neko_baron.

Ideas, concepts, and code were used from the following two sources:
1) Isaac McGarvey's 'perlin noise generator' code
2) http://www.lighthouse3d.com/opengl/terrain/index.php3?introduction

 */
using System;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {
    
    public class TreeDrawOp : DrawOp {
        
        public override string Name { get { return "Tree"; } }
        
        public Random random;
        public bool overwrite = false;
        public int Type;        
        public const int T_Tree = 0, T_NotchTree = 1, T_NotchSwamp = 2, T_Cactus = 3;
        static Brush defBrush = new SolidBrush(Block.leaf, 0);
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) { return -1; }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
        	if (brush == null) brush = defBrush;
            Vec3U16 P = marks[0];
            if (Type == T_Tree) AddTree(p, lvl, P.X, P.Y, P.Z, brush);
            if (Type == T_NotchTree) AddNotchTree(p, lvl, P.X, P.Y, P.Z, brush);
            if (Type == T_NotchSwamp) AddNotchSwampTree(p, lvl, P.X, P.Y, P.Z, brush);
            if (Type == T_Cactus) AddCactus(p, lvl, P.X, P.Y, P.Z);
        }
        
        void AddTree(Player p, Level lvl, ushort x, ushort y, ushort z, Brush brush) {
            byte height = (byte)random.Next(5, 8);
            short top = (short)(height - random.Next(2, 4));
            for (ushort dy = 0; dy < top + height - 1; dy++) {
                ushort yy = (ushort)(y + dy);
                if (overwrite || lvl.GetTile(x, yy, z) == Block.air || (yy == y && lvl.GetTile(x, yy, z) == Block.shrub))
                    PlaceBlock(p, lvl, x, yy, z, Block.trunk, 0);
            }
            
            for (short dy = (short)-top; dy <= top; ++dy)
                for (short dz = (short)-top; dz <= top; ++dz)
                    for (short dx = (short)-top; dx <= top; ++dx)
            {
                short dist = (short)(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                if ((dist < top + 1) && random.Next(dist) < 2) {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy + height), zz = (ushort)(z + dz);

                    if ((xx != x || zz != z || dy >= top - 1) && (overwrite || lvl.GetTile(xx, yy, zz) == Block.air))
                    	PlaceBlock(p, lvl, xx, yy, zz, brush);
                }
            }
        }

        void AddNotchTree(Player p, Level lvl, ushort x, ushort y, ushort z, Brush brush) {
            byte height = (byte)random.Next(3, 7);
            byte top = (byte)(height - 2);
            for (int dy = 0; dy <= height; dy++) {
                ushort yy = (ushort)(y + dy);
                byte tile = lvl.GetTile(x, yy, z);
                if (overwrite || tile == Block.air || (yy == y && tile == Block.shrub))
                    PlaceBlock(p, lvl, x, yy, z, Block.trunk, 0);
            }

            for (int dy = top; dy <= height + 1; dy++) {
                int dist = dy > height - 1 ? 1 : 2;
                for (int dz = -dist; dz <= dist; dz++)
                    for (int dx = -dist; dx <= dist; dx++)
                {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy), zz = (ushort)(z + dz);
                    byte tile = lvl.GetTile(xx, yy, zz);
                    if ((xx == x && zz == z && dy <= height) || (!overwrite && tile != Block.air))
                        continue;

                    if (Math.Abs(dx) == dist && Math.Abs(dz) == dist) {
                        if (dy > height) continue;

                        if (random.Next(2) == 0)
                            PlaceBlock(p, lvl, xx, yy, zz, brush);
                    } else {
                        PlaceBlock(p, lvl, xx, yy, zz, brush);
                    }
                }
            }
        }

        void AddNotchSwampTree(Player p, Level lvl, ushort x, ushort y, ushort z, Brush brush) {
            byte height = (byte)random.Next(4, 8);
            byte top = (byte)(height - 2);
            for (int dy = 0; dy <= height; dy++) {
                ushort yy = (ushort)(y + dy);
                byte tile = lvl.GetTile(x, yy, z);
                if (overwrite || tile == Block.air || (yy == y && tile == Block.shrub))
                    PlaceBlock(p, lvl, x, yy, z, Block.trunk, 0);
            }

            for (int dy = top; dy <= height + 1; dy++) {
                int dist = dy > height - 1 ? 2 : 3;
                for (int dz = (short)-dist; dz <= dist; dz++)
                    for (int dx = (short)-dist; dx <= dist; dx++)
                {
                    ushort xx = (ushort)(x + dx), yy = (ushort)(y + dy), zz = (ushort)(z + dz);
                    byte tile = lvl.GetTile(xx, yy, zz);
                    if ((xx == x && zz == z && dy <= height) || (!overwrite && tile != Block.air))
                        continue;

                    if (Math.Abs(dz) == dist && Math.Abs(dx) == dist) {
                        if (dy > height) continue;

                        if (random.Next(2) == 0)
                            PlaceBlock(p, lvl, xx, yy, zz, brush);
                    } else {
                    	PlaceBlock(p, lvl, xx, yy, zz, brush);
                    }
                }
            }
        }

        void AddCactus(Player p, Level lvl, ushort x, ushort y, ushort z) {
            byte height = (byte)random.Next(3, 6);
            for (ushort dy = 0; dy <= height; dy++) {
                if (overwrite || lvl.GetTile(z, (ushort)(y + dy), z) == Block.air)
                    PlaceBlock(p, lvl, x, (ushort)(y + dy), z, Block.green, 0);
            }

            int inX = 0, inZ = 0;
            switch (random.Next(1, 3)) {
                    case 1: inX = -1; break;
                case 2:
                    default: inZ = -1; break;
            }

            for (ushort dy = height; dy <= random.Next(height + 2, height + 5); dy++) {
                if (overwrite || lvl.GetTile((ushort)(x + inX), (ushort)(y + dy), (ushort)(z + inZ)) == Block.air)
                    PlaceBlock(p, lvl, (ushort)(x + inX), (ushort)(y + dy), (ushort)(z + inZ), Block.green, 0);
            }
            for (ushort dy = height; dy <= random.Next(height + 2, height + 5); dy++) {
                if (overwrite || lvl.GetTile((ushort)(x - inX), (ushort)(y + dy), (ushort)(z - inZ)) == Block.air)
                    PlaceBlock(p, lvl, (ushort)(x - inX), (ushort)(y + dy), (ushort)(z - inZ), Block.green, 0);
            }
        }

        public static bool TreeCheck(Level lvl, ushort x, ushort z, ushort y, short dist) { //return true if tree is near
            for (short dy = (short)-dist; dy <= +dist; ++dy)
                for (short dz = (short)-dist; dz <= +dist; ++dz)
                    for (short dx = (short)-dist; dx <= +dist; ++dx)
            {
                byte tile = lvl.GetTile((ushort)(x + dx), (ushort)(z + dz), (ushort)(y + dy));
                if (tile == Block.trunk || tile == Block.green) return true;
            }
            return false;
        }
    }
}
