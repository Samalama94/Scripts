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
            Pressurizing,
            Depressurizing

        }

        private AirlockState airlockState;


        int innerDoorsCloseDelay = 50;
        int outerDoorsCloseDelay = 50;
        int outerDoorsOpenDelay = 60;
        private int innerDoorsOpenDelay = 60;

        bool innerDoorOpened;
        bool outerDoorOpened;
        private bool innerDoorClosed;
        bool outerDoorClosed;
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
            Me.GetSurface(0).WriteText("Airlock State: " + airlockState.ToString() + "\n");

            innerDoorOpened = innerDoors.Any(door => door.Status == DoorStatus.Open);
            outerDoorOpened = outerDoors.Any(door => door.Status == DoorStatus.Open);
            innerDoorClosed = innerDoors.Any(door => door.Status == DoorStatus.Closed);
            outerDoorClosed = outerDoors.Any(door => door.Status == DoorStatus.Closed);


            if (!innerDoorOpened && !outerDoorOpened)
            {

                foreach (var door in innerDoors)
                    door.Enabled = true;

                foreach (var door in outerDoors)
                    door.Enabled = true;

            }

            switch (airlockState)
            {
                default:
                    ManageAirlock();
                    break;
                case AirlockState.Pressurizing:

                    airVent.Depressurize = false;
                    if (airVent.GetOxygenLevel() >= .9f)
                    {
                        OpenInnerDoors();
                        airlockState = AirlockState.Ready;
                    }
                    break;
                case AirlockState.InnerDoorOpening:
                    if (innerDoorOpened)
                    {
                        if (innerDoorsCloseDelay > 0)
                        {
                            innerDoorsCloseDelay--;
                        }
                        else
                        {
                            CloseInnerDoors();

                            innerDoorsCloseDelay = 50;
                            airlockState = AirlockState.Depressurizing;
                        }
                        

                    }
                    else
                    {
                        if (outerDoorsOpenDelay > 0 && innerDoorClosed)
                        {
                            outerDoorsOpenDelay--;
                        }
                        else if (innerDoorClosed)
                        {
                            OpenOuterDoors();
                            outerDoorsOpenDelay = 60;
                            airlockState = AirlockState.Ready;
                        }

                    }

                    break;
                case AirlockState.Depressurizing:

                    airVent.Depressurize = true;
                    if (airVent.GetOxygenLevel() <= .1f)
                    {
                        if (outerDoorsOpenDelay > 0)
                        {
                            outerDoorsOpenDelay--;
                        }
                        else
                        {
                           OpenOuterDoors();
                           airlockState = AirlockState.Ready;
                           outerDoorsOpenDelay = 50;
                           lastDoorOpened = "";

                        }
                        
                       
                    }

                    break;
            }

        }

        public void ManageAirlock()
        {
            if (outerDoorOpened)
            {
                CloseOuterDoors();
            }
            else if (innerDoorOpened)
            {
                lastDoorOpened = "Inner";
                airlockState = AirlockState.InnerDoorOpening;
            }
        }

        public void LockDoors()
        {
            foreach (var door in innerDoors)
            {
                door.Enabled = false;
            }

            foreach (var door in outerDoors)
            {
                door.Enabled = false;
            }
        }

        public void UnlockDoors()
        {
            foreach (var door in innerDoors)
            {
                door.Enabled = true;
            }

            foreach (var door in outerDoors)
            {
                door.Enabled = true;
            }
        }

        public void OpenOuterDoors()
        {
            foreach (var door in outerDoors)
            {
                door.OpenDoor();

            }


        }

        public void CloseOuterDoors()
        {
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


                outerDoorsCloseDelay = 50;

            }
        }

        public void OpenInnerDoors()
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

                    }
                }
            }
        }

        public void CloseInnerDoors()
        {

            foreach (var door in innerDoors)
            {
                door.CloseDoor();
            }


        }

    }
}
