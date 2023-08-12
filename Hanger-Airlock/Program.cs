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

        enum AirlockState
        {
            Ready,
            InnerDoorOpening,
            InnerDoorClosing,
            InnerDoorClosed,
            InnerDoorOpen,
            OuterDoorOpening,
            OuterDoorOpen,
            OuterDoorClosing,
            OuterDoorClosed
        }

        private AirlockState airlockState;

        bool innerDoorsClosing = false;
        bool outerDoorsClosing = false;

        int innerDoorsCloseDelay = 50;
        int outerDoorsCloseDelay = 50;
        int outerDoorsOpenDelay = 60;

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
                airlockState = AirlockState.InnerDoorOpening;
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

                    airlockState = AirlockState.InnerDoorClosing;
                    innerDoorsClosing = true;
                    innerDoorsCloseDelay = 50;
                    int i = 0;


                }
            }

            if (lastDoorOpened == "Inner" && airlockState == AirlockState.InnerDoorClosing && !innerDoorOpened)
            {
                if (outerDoorsOpenDelay > 0)
                {
                    outerDoorsOpenDelay--;
                }
                else
                {
                    if (outerDoorsOpenDelay == 0)
                    {

                        foreach (var door in outerDoors)
                        {
                            door.OpenDoor();
                            outerDoorsOpenDelay--;
                            airlockState = AirlockState.OuterDoorOpening;
                        }
                    }
                }
            }

            if (outerDoorsCloseDelay > 0 && outerDoorOpened && airlockState == AirlockState.OuterDoorOpening)
            {
                outerDoorsCloseDelay--;
            }
            else if (outerDoorsCloseDelay == 0 && outerDoorOpened)
            {
                airlockState = AirlockState.OuterDoorClosing;
                foreach (var door in outerDoors)
                {
                    door.CloseDoor();
                }
                outerDoorsClosing = true;
                outerDoorsCloseDelay = 50;
                outerDoorsOpenDelay = 60;
                airlockState = AirlockState.Ready;
            }







        }



    }
}
