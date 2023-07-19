public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save()
{ }

public void Main(string argument, UpdateType updateSource)
{
    var tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, t => t.IsSameConstructAs(Me));

    var lcdPanel = GridTerminalSystem.GetBlockWithName("Tank Display") as IMyTextPanel;
    lcdPanel.ContentType = ContentType.TEXT_AND_IMAGE;
    lcdPanel.Alignment = TextAlignment.LEFT;
    lcdPanel.FontSize = 5f;
    lcdPanel.WriteText("Hydrogen Tanks:");

    double totalLevel = 0;
    foreach (var tank in tanks)
    {
        totalLevel += tank.FilledRatio;
    }

    int barLength = 25;
    int filledBars = (int)Math.Round(totalLevel / tanks.Count * barLength);
    string bar = new string('#', filledBars) + new string('-', barLength - filledBars);
    lcdPanel.WriteText($"Total Level: {bar}");
    Me.GetSurface(0).FontSize = 3f;
    Me.GetSurface(0).WriteText("Tank Level");
}
