IMyDoor innerDoor;
IMyDoor outerDoor;
IMyAirVent airVent;

private IMyDoor lastDoorOpened;

bool innerDoorClosing = false;
bool outerDoorClosing = false;
int innerDoorCloseDelay = 25; // 2.5 seconds
int outerDoorCloseDelay = 25; // 2.5 seconds

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    var airlockGroup = GridTerminalSystem.GetBlockGroupWithName("Upper air lock 2(Station)");
    if (airlockGroup != null)
    {
        List<IMyTerminalBlock> airlockBlocks = new List<IMyTerminalBlock>();
        airlockGroup.GetBlocks(airlockBlocks);
        foreach (IMyTerminalBlock block in airlockBlocks)
        {
            if (block is IMyDoor)
            {
                if (block.CustomName.Contains("Inner"))
                {
                    innerDoor = block as IMyDoor;
                }
                else if (block.CustomName.Contains("Outer"))
                {
                    outerDoor = block as IMyDoor;
                }
            }
            else if (block is IMyAirVent)
            {
                airVent = block as IMyAirVent;
            }
        }
    }
}

public void Main(string argument, UpdateType updateSource)
{
    IMyTextSurface display = null;
    List<IMyTextSurfaceProvider> providers = new List<IMyTextSurfaceProvider>();
    GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(providers);
    foreach (IMyTextSurfaceProvider provider in providers)
    {
        for (int i = 0; i < provider.SurfaceCount; i++)
        {
            IMyTextSurface screen = provider.GetSurface(i);
            if (!screen.DisplayName.Equals("Main Airlock Display")) continue;
            display = screen;
            break;
        }
        if (display != null)
        {
            break;
        }
    }

    if (display != null)
    {
        if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed)
        {
            //innerDoor.Enabled = true;
            //outerDoor.Enabled = true;
            display.WriteText("Airlock Ready", false);
        }
        else if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Closed)
        {
            display.WriteText("Airlock Depressurizing", false);
        }
        else if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Open)
        {
            display.WriteText("Airlock Pressurizing", false);
        }
        else if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Open)
        {
            display.WriteText("Airlock Error", false);
        }

    }


    var surface = Me.GetSurface(0);
    surface.FontSize = 1.5f;
    if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed)
    {
        innerDoor.Enabled = true;
        outerDoor.Enabled = true;
        surface.WriteText("Airlock Ready", false);
    }
    else if (innerDoorClosing && innerDoorCloseDelay == 0)
    {
        surface.WriteText("Airlock Depressurizing", false);
    }
    else if (outerDoorClosing && outerDoorCloseDelay == 0)
    {
        surface.WriteText("Airlock Pressurizing", false);
    }
    else if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Open)
    {
        surface.WriteText("Airlock Error", false);
    }



    // Check if the doors and airvent are not null
    if (innerDoor != null && outerDoor != null && airVent != null)
    {
        // Check if inner door is open and outer door is closed
        if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Closed && lastDoorOpened != innerDoor)
        {
            lastDoorOpened = innerDoor;
            // lock outer door
            outerDoor.Enabled = false;
            // Check if airvent can pressurize
            if (airVent.CanPressurize)
            {
                if (!innerDoorClosing)
                {
                    innerDoorClosing = true;

                    innerDoorCloseDelay = 25; // 2.5 seconds
                }
                else if (innerDoorClosing && innerDoorCloseDelay > 0)
                {
                    innerDoorCloseDelay--;
                }
                else if (innerDoorClosing && innerDoorCloseDelay == 0)
                {

                    surface.WriteText("", false);

                    innerDoorClosing = false;
                    innerDoor.CloseDoor();
                    airVent.Depressurize = true;
                }

            }
        }
        // Check if inner door is closed and outer door is open
        else if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Open && lastDoorOpened != outerDoor)
        {
            lastDoorOpened = outerDoor;

            // lock inner door
            innerDoor.Enabled = false;
            if (!outerDoorClosing)
            {
                outerDoorClosing = true;
                outerDoorCloseDelay = 25; // 2.5 seconds
            }
            else if (outerDoorClosing && outerDoorCloseDelay > 0)
            {
                outerDoorCloseDelay--;
            }
            else if (outerDoorClosing && outerDoorCloseDelay == 0)
            {
                innerDoor.Enabled = true;  // enable inner door after outer door opens
                outerDoorClosing = false;

                outerDoor.CloseDoor();
                airVent.Depressurize = true;
            }
        }
        if (lastDoorOpened == innerDoor && innerDoor.Status == DoorStatus.Closed)
        {


            // check is airvent is pressurized
            if (airVent.GetOxygenLevel() > 0.75f)
            {
                // open outer door
                outerDoor.Enabled = true;

            }
            else
            {
                outerDoor.Enabled = false;
            }
        }
        else if (lastDoorOpened == outerDoor && outerDoor.Status == DoorStatus.Closed)
        {


            // check if airvent is depressurized
            if (airVent.GetOxygenLevel() < .1f)
            {
                // open inner door
                innerDoor.Enabled = false;

            }
            else
            {
                outerDoor.Enabled = false;
            }
        }
    }
}