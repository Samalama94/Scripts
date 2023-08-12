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

        List<IMyDoor> innerDoors;
        List<IMyDoor> outerDoors;
        IMyAirVent airVent;


        bool innerDoorsClosing = false;
        bool outerDoorsClosing = false;
      
        int innerDoorsCloseDelay = 50;
        int outerDoorsCloseDelay = 50;
        int outerDoorsOpenDuration = 5 * 60; // in seconds
        private int outerDoorsOpenDelay = 60;
        
        private string lastDoorOpened;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            innerDoors = new List<IMyDoor>();
            outerDoors = new List<IMyDoor>();

            airVent = GridTerminalSystem.GetBlockWithName("Hanger Airlock Airvent") as IMyAirVent;


            var innerGroup = GridTerminalSystem.GetBlockGroupWithName("Inner Hangar air lock door");
            var outerGroup = GridTerminalSystem.GetBlockGroupWithName("Outer Hangar airlock Door");

            if (innerGroup != null)
                innerGroup.GetBlocksOfType<IMyDoor>(innerDoors);

            if (outerGroup != null)
                outerGroup.GetBlocksOfType<IMyDoor>(outerDoors);
        }

        public void Main(string argument, UpdateType updateSource)
        {


            bool innerDoorOpened = innerDoors.Any(door => door.Status == DoorStatus.Open);
            bool outerDoorOpened = outerDoors.Any(door => door.Status == DoorStatus.Open);

            if (!innerDoorOpened && !outerDoorOpened)
            {
                Me.GetSurface(0).WriteText("Airlock Ready");
                foreach (var door in innerDoors)
                    door.Enabled = true;

                foreach (var door in outerDoors)
                    door.Enabled = true;

                innerDoorsClosing = false;
                outerDoorsClosing = false;
                
            }

            if (innerDoorOpened && !outerDoorOpened)
            {
                lastDoorOpened = "Inner";
                if (innerDoorsCloseDelay > 0)
                {
                    innerDoorsCloseDelay--;
                }
                else
                {
                    foreach (var door in innerDoors)
                    {
                        door.CloseDoor();
                    }
                    innerDoorsClosing = true;
                    innerDoorsCloseDelay = 50;
                    int i = 0;

                    
                }
            }

            if (lastDoorOpened == "Inner" && innerDoorsClosing == false  && !innerDoorOpened)
            {
                if (outerDoorsOpenDelay > 0)
                {
                    outerDoorsOpenDelay--;
                }
                else
                {
                    foreach (var door in outerDoors)
                    {
                        
                        door.OpenDoor();
                    }

                    if (outerDoorsCloseDelay > 0 && outerDoorsOpenDelay == 0)
                    {
                        outerDoorsCloseDelay--;
                    }
                    else
                    {
                        foreach (var door in outerDoors)
                        {
                            door.CloseDoor();
                        }
                        outerDoorsClosing = true;
                        outerDoorsCloseDelay = 50;
                        outerDoorsOpenDelay = 60;
                        lastDoorOpened = "";
                    }


                }


            }

        }



    }
}
