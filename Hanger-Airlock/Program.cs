using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
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
        List<IMyAirVent> airVents;
        List<IMyTextPanel> lcds;
        IMyAirVent airVent;


        IMyAirVent airlockVent;
        IMyDoor airlockOuterDoor;
        IMyDoor airlockInnerDoor;

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

        int innerAirlockInnerDoorCloseDelay = 25;

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
            airVents = new List<IMyAirVent>();
            lcds = new List<IMyTextPanel>();
            airVent = GridTerminalSystem.GetBlockWithName("Hanger Airlock Airvent") as IMyAirVent;


            airlockVent = GridTerminalSystem.GetBlockWithName("Hanger Tiny Airlock Airvent") as IMyAirVent;
            airlockOuterDoor = GridTerminalSystem.GetBlockWithName("Hanger Tiny Airlock Outer Door") as IMyDoor;
            airlockInnerDoor = GridTerminalSystem.GetBlockWithName("Hanger Tiny Airlock Inner Door") as IMyDoor;

            var innerGroup = GridTerminalSystem.GetBlockGroupWithName("Inner Hangar air lock door");
            var outerGroup = GridTerminalSystem.GetBlockGroupWithName("Outer Hangar airlock Door");
            var airVentGroup = GridTerminalSystem.GetBlockGroupWithName("Hanger Airlock Airvents");
            var lcdGroup = GridTerminalSystem.GetBlockGroupWithName("Hangar LCD Screen");

            if (innerGroup != null)
                innerGroup.GetBlocksOfType<IMyDoor>(innerDoors);

            if (outerGroup != null)
                outerGroup.GetBlocksOfType<IMyDoor>(outerDoors);

            if (airVentGroup != null)
                airVentGroup.GetBlocksOfType<IMyAirVent>(airVents);

            if (lcdGroup != null)
            {
                lcdGroup.GetBlocksOfType<IMyTextPanel>(lcds);
            }

            foreach (var lcd in lcds)
            {
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.FontSize = 3f;
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (airlockInnerDoor.Status == DoorStatus.Open)
            {
                if (innerAirlockInnerDoorCloseDelay > 0)
                {
                    innerAirlockInnerDoorCloseDelay--;
                }
                else
                {
                    airlockInnerDoor.CloseDoor();
                    innerAirlockInnerDoorCloseDelay = 25;
                }
            }


            if (airlockOuterDoor != null && airlockVent != null)
            {
                if (airlockVent.GetOxygenLevel() < 0.01f)
                {
                    airlockOuterDoor.Enabled = true;
                    if (airlockOuterDoor.Status == DoorStatus.Open)
                    {
                        airlockInnerDoor.Enabled = false;
                    }
                    else
                    {
                        airlockInnerDoor.Enabled = true;
                    }
                }
                else
                {
                    airlockOuterDoor.Enabled = false;
                }
            }


            Me.GetSurface(0).WriteText("Airlock State:\n" + airlockState.ToString() + "\n" + $"{airVent.GetOxygenLevel() * 100}");
            foreach (var lcd in lcds)
            {
                lcd.WriteText("Airlock State:\n" + airlockState.ToString() + "\n" + $"Air Pressure\nInside:\n{Math.Round(airVent.GetOxygenLevel() * 100)}%");
            }
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
                    if (airVent.GetOxygenLevel() > .01f)
                    {
                        Depressurize();
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

                    Depressurize();
                    if (airVent.GetOxygenLevel() <= .1f)
                    {
                        outerDoorsOpenDelay = 100;
                        OpenOuterDoors();
                        airlockState = AirlockState.OuterDoorClosing;
                    }
                    break;

                case AirlockState.Pressurizing:
                    Pressurize();
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

        public void Depressurize()
        {
            foreach (var airVent in airVents)
            {
                airVent.Depressurize = true;
            }
        }

        public void Pressurize()
        {
            foreach (var airVent in airVents)
            {
                airVent.Depressurize = false;
            }
        }

        public void DisplayAirLevelOnLcd()
        {
            foreach (var lcd in lcds)
            {
                lcd.WriteText("Airlock State:\n" + airlockState.ToString() + "\n" + $"{airVent.GetOxygenLevel() * 100}");
            }
        }

    }
}
