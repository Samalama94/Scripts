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
            OuterDoorOpening,
            OuterDoorClosing,
       
        }

        private AirlockState airlockState;


        int innerDoorsCloseDelay = 50;
        int outerDoorsCloseDelay = 50;
        int outerDoorsOpenDelay = 60;
        private int innerDoorsOpenDelay = 60;

        bool innerDoorOpened;
        bool outerDoorOpened;
        string lastDoorOpened;

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


            innerDoorOpened = innerDoors.Any(door => door.Status == DoorStatus.Open);
            outerDoorOpened = outerDoors.Any(door => door.Status == DoorStatus.Open);

            if (!innerDoorOpened && !outerDoorOpened)
            {
                Me.GetSurface(0).WriteText("Airlock Ready");
                foreach (var door in innerDoors)
                    door.Enabled = true;

                foreach (var door in outerDoors)
                    door.Enabled = true;

            }

            
            ManageInner();
            ManageOuter();





        }

        public void ManageInner()
        {
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
                outerDoorsCloseDelay = 50;
                outerDoorsOpenDelay = 60;
                airlockState = AirlockState.Ready;
            }
        }



        public void ManageOuter()
        {
            if (outerDoorOpened && !innerDoorOpened)
            {
                lastDoorOpened = "Outer";
                airlockState = AirlockState.OuterDoorOpening;
                if (outerDoorsCloseDelay > 0)
                {
                    outerDoorsCloseDelay--;
                }
                else
                {
                    foreach (var door in outerDoors)
                    {
                        door.CloseDoor();
                    }

                    airlockState = AirlockState.OuterDoorClosing;
                    outerDoorsCloseDelay = 50;

                }
            }

            if (lastDoorOpened == "Outer" && airlockState == AirlockState.OuterDoorClosing && !outerDoorOpened)
            {
                if (innerDoorsOpenDelay > 0)
                {
                    innerDoorsOpenDelay--;
                }
                else
                {
                    if (innerDoorsOpenDelay == 0)
                    {

                        foreach (var door in innerDoors)
                        {
                            door.OpenDoor();
                            innerDoorsOpenDelay--;
                            airlockState = AirlockState.InnerDoorOpening;
                        }
                    }
                }
            }

            if (innerDoorsCloseDelay > 0 && innerDoorOpened && airlockState == AirlockState.InnerDoorOpening)
            {
                innerDoorsCloseDelay--;
            }
            else if (innerDoorsCloseDelay == 0 && innerDoorOpened)
            {
                airlockState = AirlockState.InnerDoorClosing;
                foreach (var door in innerDoors)
                {
                    door.CloseDoor();
                }
                innerDoorsCloseDelay = 50;
                innerDoorsOpenDelay = 60;
                airlockState = AirlockState.Ready;
            }
        }



    }
}
