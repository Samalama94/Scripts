using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    internal partial class Program : MyGridProgram
    {
        private IMyCockpit Cockpit;
        private string CockpitName;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        { }

        private void Main(string argument)
        {
            if (argument == null) return;
            CockpitName = argument;

            Cockpit = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyCockpit;
            if (Cockpit == null) return;

            DisplayFuel(Cockpit.GetSurface(2));
            DisplayOres(Cockpit.GetSurface(0));
            DisplayCargoFillLevel(Cockpit.GetSurface(1));
        }

        private void DisplayCargoFillLevel(IMyTextSurface Screen)
        {
            var screen = Screen;
            if (screen != null)
            {
                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.FontSize = 3f;
            }

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

            screen.WriteText($"Fill Level:\n{fillAmount:P}\n\n{invFillLevel}", false);
        }

        private void DisplayOres(IMyTextSurface Screen)
        {
            var screen = Screen;
            if (screen != null)
            {
                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.FontSize = 3f;
            }

            List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
            List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();

            IMyCockpit oCockpit = GridTerminalSystem.GetBlockWithName("Sam's Old Miner") as IMyCockpit;

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

        private void DisplayFuel(IMyTextSurface Screen)
        {
            var screen = Screen;
            if (screen != null)
            {
                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.FontSize = 3f;
            }

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

            screen.WriteText($"Tank Level:\n{fillAmount:P}\n\n{fillLevel}", false);
        }
    }
}