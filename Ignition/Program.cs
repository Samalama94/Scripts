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
        
        string ignitionType = "";

        private bool ignition;
        public Program()
        {
            
        }

        public void Save()
        {
           
        }

        public void Main(string argument, UpdateType updateSource)
        {
            ignitionType = argument;
            if (argument == null) return;
            var attachedBlocks = new List<IMyTerminalBlock>();
            var thrusters = new List<IMyThrust>();
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            // Get all blocks on the grid
            GridTerminalSystem.GetBlocks(blocks);

            // Get all connectors on the grid
            List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);

            foreach (var block in blocks)
            {
                if (block.IsSameConstructAs(Me) && block is IMyThrust)
                {
                    thrusters.Add(block as IMyThrust);
                }
            }

            // At this point, all desired blocks should be stored in the `blocks` list.

            // get all thruster type blocks from the blocks list and store in a list called thrusters

            // print a list of all blocks to Me.GetSurface(0);
            var sb = new StringBuilder();
            foreach (var block in thrusters)
            {
                sb.AppendLine(block.CustomName);
            }
            //Me.GetSurface(0).WriteText(sb.ToString());

            ignition = thrusters.Any(thruster => thruster.Enabled == true);
            
            switch (ignition)
            {
                case false:
                    IgnitionOn(thrusters);
                    Me.GetSurface(0).WriteText("Ignition:\nOn");
                    break;
                case true:
                    IgnitionOff(thrusters);
                    Me.GetSurface(0).WriteText("Ignition:\nOff");
                    break;
                default:
                    Me.GetSurface(0).WriteText("Ignition is not set to On or Off");
                    break;
            }

        }

        private void IgnitionOff(List<IMyThrust> thrusters)
        {
            var blocks = thrusters;
            foreach (var block in blocks)
            {
                var thruster = block;
                thruster.Enabled = false;
            }
        }

        private void IgnitionOn(List<IMyThrust> thrusters)
        {
            var blocks = thrusters;
            foreach (var block in blocks)
            {
                var thruster = block;
                thruster.Enabled = true;
            }
        }
    }
}
