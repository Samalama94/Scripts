public Program()
{
}

public void Save()
{
}
void Main()
{


    IMyCockpit oCockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
    var screen = oCockpit.GetSurface(0);
    screen.FontSize = 3f;
    screen.WriteText($"Y0000000000000o\noooooooooooooooo\nooooooooooooo\n");
}