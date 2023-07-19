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
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // run every 6 seconds
        }

        public void Save()
        { }

        public void Main(string argument, UpdateType updateSource)
        {

            List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();


            var textSurface = Me.GetSurface(0);
            textSurface.FontSize = 8;

            //IMyTextPanel lcdPanel = GridTerminalSystem.GetBlockWithName("Reactor LCD") as IMyTextPanel;
            //lcdPanel.FontSize = 3;

            IMyReactor reactor = GridTerminalSystem.GetBlockWithName("Reactor") as IMyReactor;
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, t => t.IsSameConstructAs(Me));

            if (reactor == null)
            {
                Echo("Error: no reactor found");
                return;
            }
            if (batteries.Count == 0)
            {
                Echo("Error: no batteries found");
                return;
            }
            float totalStoredPower = 0;
            float maxStoredPower = 0;
            for (int i = 0; i < batteries.Count; i++)
            {
                IMyBatteryBlock battery = batteries[i] as IMyBatteryBlock;
                totalStoredPower += battery.CurrentStoredPower;
                maxStoredPower += battery.MaxStoredPower;
            }

            float avgBatteryChargeInPercent = (totalStoredPower / maxStoredPower) * 100;

            if (avgBatteryChargeInPercent > 90f) // tanks are completely full
            {
                reactor.Enabled = false; // turn off reactor
            }
            else if (avgBatteryChargeInPercent < 35f) // tanks are below 50% full
            {
                reactor.Enabled = true; // turn on reactor
            }
            else
            {
                // if tanks have between 50% and 100% hydrogen, do nothing
            }
            decimal level = Decimal.Round((decimal)avgBatteryChargeInPercent, 2);
            Echo($"Power Level: {avgBatteryChargeInPercent}%");
            Echo($"Reactor Status: {reactor.Enabled}");
            //lcdPanel.WriteText($"Power Level:\n{level}%");
            Me.GetSurface(0).WriteText($"{level}");

        }

    }
}
