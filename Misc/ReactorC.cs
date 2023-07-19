
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; // run every 6 seconds
}

public void Save()
{ }

public void Main(string argument, UpdateType updateSource)
{

    List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();


    var textSurface = Me.GetSurface(0);
    textSurface.FontSize = 8;

    //IMyTextPanel lcdPanel = GridTerminalSystem.GetBlockWithName("Reactor LCD") as IMyTextPanel;
    //lcdPanel.FontSize = 3;

    IMyReactor reactor = GridTerminalSystem.GetBlockWithName("Reactor") as IMyReactor;
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, t => t.IsSameConstructAs(Me));

    if (reactor == null)
    {
        Echo("Error: no reactor found");
        return;
    }
    if (batteries.Count == 0)
    {
        Echo("Error: no batteries found");
        return;
    }
    float totalStoredPower = 0;
    float maxStoredPower = 0;
    for (int i = 0; i < batteries.Count; i++)
    {
        IMyBatteryBlock battery = batteries[i] as IMyBatteryBlock;
        totalStoredPower += battery.CurrentStoredPower;
        maxStoredPower += battery.MaxStoredPower;
    }

    float avgBatteryChargeInPercent = (totalStoredPower / maxStoredPower) * 100;

    if (avgBatteryChargeInPercent > 90f) // tanks are completely full
    {
        reactor.Enabled = false; // turn off reactor
    }
    else if (avgBatteryChargeInPercent < 35f) // tanks are below 50% full
    {
        reactor.Enabled = true; // turn on reactor
    }
    else
    {
        // if tanks have between 50% and 100% hydrogen, do nothing
    }
    decimal level = Decimal.Round((decimal)avgBatteryChargeInPercent, 2);
    Echo($"Power Level: {avgBatteryChargeInPercent}%");
    Echo($"Reactor Status: {reactor.Enabled}");
    //lcdPanel.WriteText($"Power Level:\n{level}%");
    Me.GetSurface(0).WriteText($"{level}");

}
