void Main()
{
    // Dictionary to store item subtype and the total quantity of the item.
    var items = new Dictionary<string, int>();
    // Dictionary to store full type of item and its name to be displayed on the screen.
    var typeNameMappings = new Dictionary<string, string>()
    {
        {"MyObjectBuilder_Ore", "Ore"},
        {"MyObjectBuilder_Ingot", "Ingot"},
        {"MyObjectBuilder_Component", "Component"},
        {"MyObjectBuilder_PhysicalGunObject", "Gun" },
        // add more mappings here...
    };

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
                string subtype = item.Type.SubtypeId;

                if (items.ContainsKey(subtype))
                {
                    items[subtype] += (int)item.Amount;
                }
                else
                {
                    items.Add(subtype, (int)item.Amount);
                }

                string screenName;
                if (typeNameMappings.TryGetValue(type, out screenName))
                { // Map full type to screen name.
                    type = screenName;
                }

                List<IMyTerminalBlock> lcdBlocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.SearchBlocksOfName(type, lcdBlocks, block1 => block1 is IMyTextPanel);

                IMyTextPanel lcd = lcdBlocks.Count > 0 ? lcdBlocks[0] as IMyTextPanel : null;

                // Prevent duplicate items on screen by searching for items that have already been written.
                if (lcd != null)
                {
                    string itemText = $"{subtype}: {item.Amount}";
                    if (!lcd.GetText().Contains(itemText))
                    {
                        lcd.WriteText(itemText, true);
                        if (items.Count % 2 == 0) // if even count of items
                        {
                            lcd.WriteText("\n", true);
                        }
                        else
                        {
                            lcd.WriteText(", ", true);
                        }
                    }
                }
                else
                {
                    Echo($"Screen for {type} not found");
                }
            }
        }
    }
}
