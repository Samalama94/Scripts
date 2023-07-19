void Main()
{
    // Find the AI Basic Task Block
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(blocks, (block) => block.CustomName.Contains("Follower"));
    
    // Check if we found the block
    if (blocks.Count == 0)
    {
        Echo("AI Basic Task Block not found");
        return;
    }
    
    // Get the first found AI Basic Task Block
    IMyFunctionalBlock taskBlock = blocks[0] as IMyFunctionalBlock;

    // Enable the block
    if (!taskBlock.Enabled)
    {
        taskBlock.GetActionWithName("OnOff_On").Apply(taskBlock);
        Echo("AI Basic Task Block is now enabled");
    }
    else
    {
        Echo("AI Basic Task Block was already enabled");
    }
}