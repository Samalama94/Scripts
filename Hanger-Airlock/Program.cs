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

        private IMyDoor lastDoorOpened;

        bool innerDoorsClosing = false;
        bool outerDoorsClosing = false;
        int innerDoorsCloseDelay = 25;
        int outerDoorsCloseDelay = 25;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
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

            if (!innerDoorOpened && !outerDoorOpened && lastDoorOpened == null)
            {
                foreach (var door in innerDoors)
                    door.Enabled = true;

                foreach (var door in outerDoors)
                    door.Enabled = true;
            }
            else if (innerDoorOpened && lastDoorOpened == null)
            {
                foreach (var door in outerDoors)
                    door.Enabled = false;

                lastDoorOpened = innerDoors.First(door => door.Status == DoorStatus.Open);
                innerDoorsClosing = true;
                innerDoorsCloseDelay = 25;
            }
            else if (outerDoorOpened && lastDoorOpened == null)
            {
                foreach (var door in innerDoors)
                    door.Enabled = false;

                lastDoorOpened = outerDoors.First(door => door.Status == DoorStatus.Open);
                outerDoorsClosing = true;
                outerDoorsCloseDelay = 25;
            }
            else if (innerDoorsClosing && innerDoorsCloseDelay-- == 0)
            {
                innerDoorsClosing = false;
                foreach (var door in innerDoors)
                    door.CloseDoor();

                airVent.Depressurize = true;
            }
            else if (outerDoorsClosing && outerDoorsCloseDelay-- == 0)
            {
                outerDoorsClosing = false;
                foreach (var door in outerDoors)
                    door.CloseDoor();

                airVent.Depressurize = true;
            }
            else if (!innerDoorOpened && lastDoorOpened != null)
            {
                if (lastDoorOpened == outerDoors.First())
                {
                    lastDoorOpened = null;
                }
                else if (airVent.GetOxygenLevel() < 0.1f)
                {
                    foreach (var door in outerDoors)
                        door.Enabled = true;
                }
            }
            else if (!outerDoorOpened && lastDoorOpened != null)
            {
                if (lastDoorOpened == innerDoors.First())
                {
                    lastDoorOpened = null;
                }
                else if (airVent.GetOxygenLevel() > 0.75f)
                {
                    foreach (var door in innerDoors)
                        door.Enabled = true;
                }
            }

        }

    }
}
