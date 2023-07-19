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
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            // Find all gas generators in the group "HydroGen"
            var generators = new List<IMyGasGenerator>();
            var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
            group?.GetBlocksOfType(generators);
            foreach (var generator in generators)
            {
                generator.Enabled = true; // turn on all generators
            }
        }

        public void Save()
        { }

        public void Main(string argument, UpdateType updateSource)
        {
            IMyTextPanel lcdPanel = GridTerminalSystem.GetBlockWithName("Gen LCD") as IMyTextPanel;
            List<IMyGasTank> tanks = new List<IMyGasTank>();
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, t => t.IsSameConstructAs(Me));
            lcdPanel.FontSize = 7;
            lcdPanel.Alignment = TextAlignment.CENTER;
            var textSurface = Me.GetSurface(0);
            textSurface.FontSize = 3;
            textSurface.Alignment = TextAlignment.CENTER;
            decimal totalFillRatio = 0;
            foreach (var tank in tanks)
            {
                totalFillRatio += (decimal)tank.FilledRatio;
            }
            totalFillRatio /= tanks.Count;

            if (totalFillRatio > 0.95M) // tanks are completely full
            {
                // Find all gas generators in the group "HydroGen"
                var generators = new List<IMyGasGenerator>();
                var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
                group?.GetBlocksOfType(generators);
                foreach (var generator in generators)
                {
                    generator.Enabled = false; // turn off all generators
                }
            }
            else if (totalFillRatio < 0.5M) // tanks are below 50% full
            {
                // Find all gas generators in the group "HydroGen"
                var generators = new List<IMyGasGenerator>();
                var group = GridTerminalSystem.GetBlockGroupWithName("HydroGen");
                group?.GetBlocksOfType(generators);
                foreach (var generator in generators)
                {
                    generator.Enabled = true; // turn on all generators
                }
            }
            else
            {
                // if tanks have between 50% and 100% hydrogen, do nothing
            }
            decimal Ratio = Decimal.Round(totalFillRatio, 2);
            Echo($"Total Fill Ratio: {totalFillRatio * 100}%");
            lcdPanel.WriteText($"Fill: {Ratio * 100}%");
            textSurface.WriteText($"02/H2 Gen\nControl");
        }

    }
}
