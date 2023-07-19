public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save()
{ }


void Main()
{
    double altitude = 0;
    IMyRemoteControl remoteControl = GridTerminalSystem.GetBlockWithName("Remote Control") as IMyRemoteControl;
    if (remoteControl != null)
    {
        altitude = remoteControl.GetShipSpeed();
        Me.GetSurface(0).WriteText("Altitude: " + altitude.ToString("N0") + " meters");
        Echo("Altitude: " + altitude.ToString("N0") + " meters");
    }
    else
    {
        Echo("Error: No remote control found!");
    }

    IMySoundBlock soundBlock = GridTerminalSystem.GetBlockWithName("Sound Block Name") as IMySoundBlock;

    if (soundBlock == null)
    {
        Echo("Sound block not found.");
        return;
    }

    if (altitude < 100)
    {
        soundBlock.GetActionWithName("PlaySound").Apply(soundBlock);
        return;
    }

}