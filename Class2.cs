public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    var airlockGroups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(airlockGroups, group => group.Name.Contains("Airlock"));

   
}

public void Main()
{
    foreach (IMyBlockGroup airlockGroup in airlockGroups)
    {
        IMyDoor innerDoor = null;
        IMyDoor outerDoor = null;
        IMyAirVent airVent = null;

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

        if (innerDoor != null && outerDoor != null && airVent != null)
        {
            // Check if inner door is closed and outer door is closed, lock the outer one as well
            if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed)
            {
                innerDoor.Enabled = true;
                outerDoor.Enabled = true;
            }
            // Check if inner door is open and outer door is closed
            else if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Closed)
            {
                airVent.Depressurize = false;
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
                        outerDoor.Enabled = true;  // enable outer door after it closes
                        airVent.Depressurize = true;
                        innerDoorClosing = false;
                        innerDoor.CloseDoor();
                    }
                }
            }
            // Check if inner door is closed and outer door is open
            else if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Open)
            {
                airVent.Depressurize = true;
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
                    innerDoor.Enabled = true; // enable inner door after it closes
                    outerDoorClosing = false;
                    outerDoor.CloseDoor();
                }
            }
            else if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Open)
            {
                // both doors are open, not supposed to happen
                innerDoor.Enabled = false;
                outerDoor.Enabled = false;
                airVent.Depressurize = true;
            }

            // Update the airlock status on each display panel
            List<IMyTextSurface> surfacesToUpdate = new List<IMyTextSurface>();
            GridTerminalSystem.GetBlocksOfType(surfacesToUpdate, display => display.CustomName.Contains("Display") && display.CubeGrid == Me.CubeGrid);
            foreach (IMyTextSurface surface in surfacesToUpdate)
            {
                if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed)
                {
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
            }
        }


