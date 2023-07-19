using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.





        IMyDoor innerDoor;
        IMyDoor outerDoor;
        IMyAirVent airVent;

        bool innerDoorClosing = false;
        bool outerDoorClosing = false;
        int innerDoorCloseDelay = 25; // 2.5 seconds
        int outerDoorCloseDelay = 25; // 2.5 seconds

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            var airlockGroup = GridTerminalSystem.GetBlockGroupWithName("Station Airlock");
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
                    innerDoor.Enabled = true;
                    outerDoor.Enabled = true;
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
                if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Closed)
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
                            surface.WriteText("", false);
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
                        airVent.Depressurize = false;
                        innerDoor.Enabled = true;  // enable inner door after outer door opens
                        outerDoorClosing = false;
                        outerDoor.CloseDoor();
                    }
                }
            }
        }
    }
    
}

