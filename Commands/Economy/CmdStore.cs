﻿/*
    Copyright 2011 MCForge
    
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
using System;
using System.Globalization;
using System.Threading;
using MCGalaxy.Eco;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    /// <summary> Economy Beta v1.0 QuantumHive  </summary>
    public sealed class CmdStore : Command {
        public override string name { get { return "store"; } }
        public override string shortcut { get { return "shop"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("item") }; }
        }        

        public override void Use(Player p, string message) {
            if (message == "") {
                foreach (Item item in Economy.Items) {
                    if (!item.Enabled) continue;
                    item.OnStoreOverview(p); 
                }
        		Player.SendMessage(p, "%HUse %T/store [item] %Hto see more information about that item.");
                return;
            }
            
            foreach (Item item in Economy.Items)
                foreach (string alias in item.Aliases)
            {
                if (message.CaselessEq(alias)) {
                    if (!item.Enabled) { 
                        Player.SendMessage(p, "%cThe " + item.Name + " item is not currently buyable."); return; 
                    }
                    item.OnStoreCommand(p); 
                    return;
                }
            }
            Help(p);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/store [item]");
            Player.SendMessage(p, "%HViews information about the specific item, such as its cost.");
            Player.SendMessage(p, "%T/store");
            Player.SendMessage(p, "%HViews information about all enabled items.");
            Player.SendMessage(p, "%H   Available items: %f" + Economy.GetItemNames(", "));
        }
    }
}
