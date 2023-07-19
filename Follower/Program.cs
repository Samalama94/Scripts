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

        MyDetectedEntityInfo player;
        IMySensorBlock sensor;
        string sensorOwner;


        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            sensor = GridTerminalSystem.GetBlockWithName("Sensor") as IMySensorBlock;

            if (sensor != null)
            {
                sensorOwner = sensor.OwnerId.ToString();
            }

        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
            Storage = player.EntityId.ToString();
        }
        
        void Main()
        {

            List<MyDetectedEntityInfo> detectedEntities = new List<MyDetectedEntityInfo>();
            sensor.DetectedEntities(detectedEntities);

            if (detectedEntities.Count > 0)
            {
                // Save the first detected entity.
                player = detectedEntities[0];
            }

            if (player.IsEmpty())
            {
                return;
            }

            var ship = GridTerminalSystem.GetBlockWithName("Follower") as IMyRemoteControl;
            
            List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("Follower 1 Cargo", containers,
                x => x is IMyCargoContainer && x.CubeGrid == ship.CubeGrid);
            double maxCapacity = 0;
            double currentCapacity = 0;



            foreach (IMyCargoContainer container in containers)
            {
                var inventory = container.GetInventory(0);
                maxCapacity += (double)inventory.MaxVolume;
                currentCapacity += (double)inventory.CurrentVolume;
            }

            double fillPercentage = currentCapacity / maxCapacity;

            if (Vector3D.Distance(ship.GetPosition(), player.Position) < 20)
            {
                ship.ClearWaypoints();
                return;
            }


            if (fillPercentage < 0.9 && (Vector3D.Distance(ship.GetPosition(), player.Position) > 20))
            {
                ship.ClearWaypoints();
                ship.AddWaypoint(player.Position, "player");
                ship.SetAutoPilotEnabled(true);
            }
            else
            {
                ship.ClearWaypoints();
                // Assuming home waypoint is created manually and is named "Home"
                var home = new MyWaypointInfo("home", new Vector3D(-41135.59, -36579.51, -33672.06));
                ship.AddWaypoint(home.Coords, "home");
                ship.SetAutoPilotEnabled(true);
            }

            Echo($"Cargo is {(fillPercentage * 100):F1}% full.");
            Me.GetSurface(0).WriteText($"Cargo is {(fillPercentage * 100):F1}% full.");
        }
    }
}
//note this is currently causing the ship to crash??