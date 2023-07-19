using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{ }

// split given list into chunks
public static List<List<T>> SplitList<T>(List<T> items, int chunkSize)
{
    List<List<T>> chunks = new List<List<T>>();
    for (int i = 0; i < items.Count; i += chunkSize)
    {
        chunks.Add(items.GetRange(i, Math.Min(chunkSize, items.Count - i)));
    }
    return chunks;
}


public void Main(string argument)
{
    // Get all LCD blocks in a group called "ItemViewer LCD" from the grid.
    List<IMyTerminalBlock> lcdBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlockGroupWithName("ItemViewer LCD").GetBlocksOfType<IMyTextPanel>(lcdBlocks);


    float fontSize;
    int chunkSize;
    var textSurface = Me.GetSurface(0);
    textSurface.FontSize = 3;
    

    switch (lcdBlocks.Count)
    {
        case 1:
            fontSize = 1.0f;
            chunkSize = 15;
            break;
        case 2:
            fontSize = 1.25f;
            chunkSize = 13;
            break;
        default:
            fontSize = 1.5f;
            chunkSize = 10;
            break;
    }

    // Loop over all found lcds and modify them
    foreach (IMyTextPanel lcd in lcdBlocks)
    {
        //lcd.Alignment = TextAlignment.CENTER;
        lcd.FontSize = fontSize;

        if (lcd == null)
        {
            Echo("LCD not found.");
            return;
        }

        // Clear the LCD.
        lcd.WriteText("");
    }

    // Create a List to hold all cargo containers that we will collect.
    List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);

    // Make sure we found at least one cargo container.
    if (cargoContainers.Count == 0)
    {
        Echo("No cargo containers found.");
        return;
    }

    // Get the list of items in all loaded containers.
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    foreach (var container in cargoContainers)
    {
        IMyInventory inventory = ((IMyEntity)container).GetInventory(0);
        inventory.GetItems(items);
    }

    // Split items into chunks of 10.
    List<List<MyInventoryItem>> chunks = SplitList(items, chunkSize);

    for (int i = 0; i < chunks.Count; i++)
    {
        for (int j = 0; j < chunks[i].Count; j++)
        {
            if (lcdBlocks.Count != 0)
            {
                IMyTextPanel lcd = (IMyTextPanel)lcdBlocks[(i % lcdBlocks.Count)];
                lcd.WriteText(chunks[i][j].Type.SubtypeId + ": " + chunks[i][j].Amount + "\n", true);
                //lcd.WriteText(chunks[i][j].Type.TypeId + " - " + chunks[i][j].Type.SubtypeId + ": " + chunks[i][j].Amount + "\n", true);
            }
        }
    }
    textSurface.WriteText("Item Viewer\n");
    Echo("Task complete.");
}
