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

            if (Vector3D.Distance(ship.GetPosition(), player.Position) > 50) {return;}


            foreach (IMyCargoContainer container in containers)
            {
                var inventory = container.GetInventory(0);
                maxCapacity += (double)inventory.MaxVolume;
                currentCapacity += (double)inventory.CurrentVolume;
            }

            double fillPercentage = currentCapacity / maxCapacity;




            if (fillPercentage < 0.9)
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