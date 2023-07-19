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

            string fillLevel = new string('#', numHashes) + new string('=', numEquals);

            IMyTextPanel cockpitScreen2 = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Cockpit Screen 2");
            if (cockpitScreen2 != null)
            {
                cockpitScreen2.ContentType = ContentType.TEXT_AND_IMAGE;
                cockpitScreen2.FontSize = 3f;
                cockpitScreen2.WriteText($"Fill Level: {fillAmount:P}\n\n{fillLevel}", false);
            }
        }

        public void DisplayOres()
        {
            List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
            List<MyInventoryItem> inventoryItems = new List<MyInventoryItem>();
            IMyTextPanel cockpitScreen = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Cockpit Screen 1");

            if (cockpitScreen == null)
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



            cockpitScreen.FontSize = 3f;
            cockpitScreen.WriteText(sb.ToString(), false);
        }