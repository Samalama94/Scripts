using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.Entities.Blocks;
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


        IMyAirVent airlockVent;
        IMyDoor airlockOuterDoor;
        IMyDoor airlockInnerDoor;

        private int innerAirlockInnerDoorCloseDelay = 50;

        public Program()
        {
            // Run every 100 ticks (60 ticks is 1 second)
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            var airlockGroup = GridTerminalSystem.GetBlockGroupWithName("Station Main Airlock");
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
                            airlockInnerDoor = block as IMyDoor;
                        }
                        else if (block.CustomName.Contains("Outer"))
                        {
                            airlockOuterDoor = block as IMyDoor;
                        }
                    }
                    else if (block is IMyAirVent)
                    {
                        airlockVent = block as IMyAirVent;
                    }
                }

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
                    airlockOuterDoor.Enabled = true;
                }
            }

        }


    }


}



