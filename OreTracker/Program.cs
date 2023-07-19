using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.





        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        { }

        void Main()
        {
            var items = new Dictionary<string, int>();

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(blocks);

            foreach (var block in blocks)
            {
                IMyInventory inventory = (block as IMyCargoContainer)?.GetInventory(0);

                if (inventory != null)
                {
                    List<MyInventoryItem> containerItems = new List<MyInventoryItem>();

                    inventory.GetItems(containerItems);

                    foreach (var item in containerItems)
                    {
                        string type = item.Type.TypeId;

                        if (type.Contains("MyObjectBuilder_Ore") || type.Contains("MyObjectBuilder_Ingot") || type.Contains("MyObjectBuilder_OxygenContainerObject"))
                        {
                            string subtype = item.Type.SubtypeId;

                            if (items.ContainsKey(subtype))
                            {
                                items[subtype] += (int)item.Amount;
                            }
                            else
                            {
                                items.Add(subtype, (int)item.Amount);
                            }
                        }
                    }
                }
            }
            
            IMyCockpit Cockpit = GridTerminalSystem.GetBlockWithName("[Flight 1]") as IMyCockpit;
            var screen = Cockpit.GetSurface(1);
            screen.FontSize = 0.7f;
            var StringList = new List<string>();
            StringList.AddRange(new[] { "ORES\n====\n\n" });
            int count = 0;

            foreach (var item in items)
            {
                if (count % 2 == 0)
                {
                    StringList.Add($"{item.Key}: {item.Value}, ");
                }
                else
                {
                    StringList[StringList.Count - 1] += $"{item.Key}: {item.Value}\n";
                }

                count++;
            }

            // If the last item was in an odd index, add its name only to the line
            if (count % 2 != 0 && count > 1)
            {
                StringList[StringList.Count - 1] += "\n";
            }

            screen.WriteText(String.Join("", StringList));
        }

        /* Version for an LCD panel
         * public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        { }

        void Main()
        {
            var items = new Dictionary<string, int>();

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(blocks);

            foreach (var block in blocks)
            {
                IMyInventory inventory = (block as IMyCargoContainer)?.GetInventory(0);

                if (inventory != null)
                {
                    List<MyInventoryItem> containerItems = new List<MyInventoryItem>();

                    inventory.GetItems(containerItems);

                    foreach (var item in containerItems)
                    {
                        string type = item.Type.TypeId;

                        if (type.Contains("MyObjectBuilder_Ore") || type.Contains("MyObjectBuilder_Ingot") || type.Contains("MyObjectBuilder_OxygenContainerObject"))
                        {
                            string subtype = item.Type.SubtypeId;

                            if (items.ContainsKey(subtype))
                            {
                                items[subtype] += (int)item.Amount;
                            }
                            else
                            {
                                items.Add(subtype, (int)item.Amount);
                            }
                        }
                    }
                }
            }

            //IMyCockpit Cockpit = GridTerminalSystem.GetBlockWithName("[Flight 1]") as IMyCockpit;
            //var screen = Cockpit.GetSurface(1);
            var screen = GridTerminalSystem.GetBlockWithName("DoDo") as IMyTextPanel;
            screen.FontSize = 0.7f;
            var StringList = new List<string>();
            StringList.AddRange(new[] { "ORES\n====\n\n" });
            int count = 0;

            foreach (var item in items)
            {
                if (count % 2 == 0)
                {
                    StringList.Add($"{item.Key}: {item.Value}, ");
                }
                else
                {
                    StringList[StringList.Count - 1] += $"{item.Key}: {item.Value}\n";
                }

                count++;
            }

            // If the last item was in an odd index, add its name only to the line
            if (count % 2 != 0 && count > 1)
            {
                StringList[StringList.Count - 1] += "\n";
            }

            screen.WriteText(String.Join("", StringList));
        }
        */
    }
}
