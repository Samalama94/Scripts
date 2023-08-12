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
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        void Main()
        {
            DisplayFuel();
            DisplayOres();
            DisplayCargoFillLevel();

        }
        public void DisplayCargoFillLevel()
        {
            List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
            float fillAmount = 0;

            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, c => c.CustomName.Contains("Cargo Container"));

            foreach (var container in cargoContainers)
            {
                var inventory = (container as IMyInventoryOwner).GetInventory(0);

                fillAmount += (float)inventory.CurrentVolume / (float)inventory.MaxVolume;
            }

            fillAmount /= cargoContainers.Count;

            int numHashes = (int)(fillAmount * 10);
            int numEquals = 10 - numHashes;

            string invFillLevel = new string('#', numHashes) + new string('=', numEquals);


            IMyCockpit oCockpit = GridTerminalSystem.GetBlockWithName("Sam's Old Miner") as IMyCockpit;
            var screen = oCockpit.GetSurface(1);
            if (screen != null)
            {
                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.FontSize = 3f;
                screen.WriteText($"Fill Level:\n{fillAmount:P}\n\n{invFillLevel}", false);
            }
        }

        public void DisplayOres()
        {
            List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
            List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();

            IMyCockpit oCockpit = GridTerminalSystem.GetBlockWithName("Sam's Old Miner") as IMyCockpit;
            var screen = oCockpit.GetSurface(0);

            if (screen == null)
            {
                Echo("Error, no cockpit screen found");
                return;
            }

            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, c => c.CustomName.Contains("Cargo Container"));

            foreach (var container in cargoContainers)
            {
                var inventory = (container as IMyInventoryOwner).GetInventory(0);
                var items = new List<MyInventoryItem>();

                inventory.GetItems(items, (i) => i.Type.TypeId == "MyObjectBuilder_Ore");

                foreach (var item in items)
                {
                    MyItemType itemType = item.Type;

                    inventoryItems.Add(item);
                }
            }


            StringBuilder sb = new StringBuilder("Ores:\n");

            foreach (var item in inventoryItems) { sb.AppendLine($" {Math.Round((double)item.Amount)} {item.Type.SubtypeId}"); }



            screen.FontSize = 3f;
            screen.WriteText(sb.ToString(), false);
            
        }
        public void DisplayFuel()
        {
            List<IMyGasTank> gasTanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(gasTanks, c => c.CustomName.Contains("Hydrogen Tank"));



            double totalCapacity = 0;
            double totalFillLevel = 0;
            foreach (var tank in gasTanks)
            {
                double capacity = (double)tank.Capacity;
                double tankFill = (double)tank.FilledRatio;
                totalCapacity += capacity;
                totalFillLevel += tankFill * capacity;
            }


            double fillAmount = totalFillLevel / totalCapacity;

            int numHashes = (int)(fillAmount * 10);
            int numEquals = 10 - numHashes;

            string fillLevel = new string('#', numHashes) + new string('=', numEquals);


            IMyCockpit oCockpit = GridTerminalSystem.GetBlockWithName("Sam's Old Miner") as IMyCockpit;
            var screen = oCockpit.GetSurface(2);

            if (screen != null)
            {
                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.FontSize = 3f;
                screen.WriteText($"Tank Level:\n{fillAmount:P}\n\n{fillLevel}", false);
            }
        }


    }
}
