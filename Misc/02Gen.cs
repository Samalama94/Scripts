public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    // Find all gas generators in the group "HydroGen"
    var generators = new List<IMyGasGenerator>();
    var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
    group?.GetBlocksOfType(generators);
    foreach (var generator in generators)
    {
        generator.Enabled = true; // turn on all generators
    }
}
 
public void Save()
{ }

public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel lcdPanel = GridTerminalSystem.GetBlockWithName("Gen LCD") as IMyTextPanel;
    List<IMyGasTank> tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, t => t.IsSameConstructAs(Me));
    lcdPanel.FontSize = 7;
    lcdPanel.Alignment = TextAlignment.CENTER;
    var textSurface = Me.GetSurface(0);
    textSurface.FontSize = 3;
    textSurface.Alignment = TextAlignment.CENTER;
    decimal totalFillRatio = 0;
    foreach (var tank in tanks)
    {
        totalFillRatio += (decimal)tank.FilledRatio;
    }
    totalFillRatio /= tanks.Count;

    if (totalFillRatio > 0.95M) // tanks are completely full
    {
        // Find all gas generators in the group "HydroGen"
        var generators = new List<IMyGasGenerator>();
        var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
        group?.GetBlocksOfType(generators);
        foreach (var generator in generators)
        {
            generator.Enabled = false; // turn off all generators
        }
    }
    else if (totalFillRatio < 0.5M) // tanks are below 50% full
    {
        // Find all gas generators in the group "HydroGen"
        var generators = new List<IMyGasGenerator>();
        var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
        group?.GetBlocksOfType(generators);
        foreach (var generator in generators)
        {
            generator.Enabled = true; // turn on all generators
        }
    }
    else
    {
        // if tanks have between 50% and 100% hydrogen, do nothing
    }
    decimal Ratio = Decimal.Round(totalFillRatio, 2);
    Echo($"Total Fill Ratio: {totalFillRatio * 100}%");
    lcdPanel.WriteText($"Fill: {Ratio * 100}%");
    textSurface.WriteText($"02/H2 Gen\nControl");
}
