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
        private bool innerDoorClosed;
        bool outerDoorClosed;
        private string lastDoorOpened = "";

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
            Me.GetSurface(0).WriteText("Airlock State:\n" + airlockState.ToString() + "\n" + $"{outerDoorsCloseDelay}\n{airVent.GetOxygenLevel() * 100}");

            innerDoorOpened = innerDoors.Any(door => door.Status == DoorStatus.Open);
            outerDoorOpened = outerDoors.Any(door => door.Status == DoorStatus.Open);
            innerDoorClosed = innerDoors.Any(door => door.Status == DoorStatus.Closed);
            outerDoorClosed = outerDoors.Any(door => door.Status == DoorStatus.Closed);
            if (argument != null)
            {
                lastDoorOpened = argument;
            }

            if (!innerDoorOpened && !outerDoorOpened)
            {

                foreach (var door in innerDoors)
                    door.Enabled = true;

                foreach (var door in outerDoors)
                    door.Enabled = true;

            }

            switch (airlockState)
            {
                case AirlockState.Ready:
                    ManageAirlock();
                    break;

                case AirlockState.InnerDoorOpening:
                    if (!innerDoorClosed)
                    {
                        if (innerDoorsCloseDelay > 0)
                        {
                            innerDoorsCloseDelay--;
                        }
                        else
                        {
                            CloseInnerDoors();
                            innerDoorsCloseDelay = 100;
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
                            outerDoorsOpenDelay = 120;
                            airlockState = AirlockState.Ready;

                        }
                    }

                    break;
                case AirlockState.OuterDoorOpening:
                    if (airVent.GetOxygenLevel() > .1f)
                    {
                        airVent.Depressurize = true;
                        return;
                    }
                    else
                    {
                        OpenOuterDoors();
                    }
                    if (!outerDoorClosed)
                    {


                        if (outerDoorsCloseDelay > 0)
                        {
                            outerDoorsCloseDelay--;
                        }
                        else
                        {
                            CloseOuterDoors();
                            outerDoorsCloseDelay = 100;
                            airlockState = AirlockState.Pressurizing;
                        }
                    }
                    else
                    {
                        if (innerDoorsOpenDelay > 0 && outerDoorClosed)
                        {
                            innerDoorsOpenDelay--;
                        }
                        else if (outerDoorClosed)
                        {
                            innerDoorsOpenDelay = 120;
                            airlockState = AirlockState.Ready;
                        }
                    }

                    break;
                case AirlockState.InnerDoorClosing:

                    if (innerDoorsCloseDelay > 0)
                    {
                        innerDoorsCloseDelay--;
                    }
                    else
                    {
                        CloseInnerDoors();
                        innerDoorsCloseDelay = 100;
                        airlockState = AirlockState.Ready;
                    }

                    break;
                case AirlockState.OuterDoorClosing:
                    if (outerDoorsCloseDelay > 0)
                    {
                        outerDoorsCloseDelay--;
                    }
                    else
                    {
                        CloseOuterDoors();
                        outerDoorsCloseDelay = 100;
                        airlockState = AirlockState.Ready;
                    }
                    break;
                case AirlockState.Depressurizing:

                    airVent.Depressurize = true;
                    if (airVent.GetOxygenLevel() <= .1f)
                    {
                        outerDoorsOpenDelay = 100;
                        OpenOuterDoors();
                        airlockState = AirlockState.OuterDoorClosing;
                    }
                    break;
                case AirlockState.Pressurizing:
                    airVent.Depressurize = false;
                    if (airVent.GetOxygenLevel() >= .9f)
                    {

                        OpenInnerDoors();
                        airlockState = AirlockState.InnerDoorClosing;
                        innerDoorsOpenDelay = 100;
                    }
                    break;
            }

        }

        public void ManageAirlock()
        {
            if (lastDoorOpened == "outer")
            {
                airlockState = AirlockState.OuterDoorOpening;
            }

            else if (lastDoorOpened == "inner")
            {
                OpenInnerDoors();
                airlockState = AirlockState.InnerDoorOpening;
            }
            else
            {
                airlockState = AirlockState.Ready;


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

            foreach (var door in outerDoors)
            {
                door.CloseDoor();
            }

        }

        public void OpenInnerDoors()
        {
            foreach (var door in innerDoors)
            {
                door.OpenDoor();
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
