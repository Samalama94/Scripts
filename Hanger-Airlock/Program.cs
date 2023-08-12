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


        int innerDoorsCloseDelay = 100;
        int outerDoorsCloseDelay = 100;
        int outerDoorsOpenDelay = 120;
        private int innerDoorsOpenDelay = 120;

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
                    LockDoors();
                    airVent.Depressurize = false;
                    if (airVent.GetOxygenLevel() >= .9f)
                    {
                        UnlockDoors();
                        OpenInnerDoors();
                        airlockState = AirlockState.Ready;
                    }
                    break;
                case AirlockState.InnerDoorOpening:
                    if (innerDoorOpened)
                    {
                        CloseInnerDoors();
                        airlockState = AirlockState.Depressurizing;
                        outerDoorsOpenDelay = 0; //set outerDoorOpenDelay to 0 
                        // so that the second time around it opens immediately
                        // also added a check to  open the door only when the inner 
                        // door is completely closed                   
                    }
                    else
                    {
                        outerDoorsOpenDelay = outerDoorsOpenDelay > 0 ? outerDoorsOpenDelay - 1 : outerDoorsOpenDelay; // handle delay
                        if (outerDoorsOpenDelay == 0)
                        {
                            OpenOuterDoors();
                        }

                    }

                    break;
                case AirlockState.Depressurizing:
                    LockDoors();
                    airVent.Depressurize = true;
                    if (airVent.GetOxygenLevel() <= .1f)
                    {
                        UnlockDoors();
                        OpenOuterDoors();
                        airlockState = AirlockState.Ready;
                    }

                    break;
            }

        }

        public void ManageAirlock()
        {
            if (outerDoorOpened)
            {
                CloseOuterDoors();
                airVent.Depressurize = false;

                if (airVent.GetOxygenLevel() >= .9f)
                {
                    airlockState = AirlockState.InnerDoorOpening;
                }
            }
            else if (innerDoorOpened)
            {
                CloseInnerDoors();
                airVent.Depressurize = true;

                if (airVent.GetOxygenLevel() <= .1f)
                {
                    airlockState = AirlockState.OuterDoorOpening;
                }
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
            bool innerClosed = innerDoors.All(door => door.Status == DoorStatus.Closed);
            if (innerClosed && outerDoorsOpenDelay == 0)
            {

                foreach (var door in outerDoors)
                {
                    door.OpenDoor();
                    break; // we only need to open one door
                }
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


                outerDoorsCloseDelay = 100;

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

                innerDoorsCloseDelay = 100;

            }
        }

    }
}
