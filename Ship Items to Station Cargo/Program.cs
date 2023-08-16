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
        public Program()
        {
            
        }

        public void Save()
        {
            
        }

        // Using IsSameConstructAs to verify that a container is a part of the same construct at 'Me' before transferring items.

        void Main(string argument)
        {
            // Get all connectors named "Dock 1"
            var dock1 = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(argument, dock1, x => x is IMyShipConnector);

            // Get all connectors to "Dock 1"
            var dockConnectors = new List<IMyShipConnector>();
            foreach (IMyShipConnector dock in dock1)
            {
                if (dock.Status == MyShipConnectorStatus.Connected)
                {
                    dockConnectors.Add(dock.OtherConnector);
                }
            }

            // Get all cargo containers attached to these connectors
            var cargoContainers = new List<IMyTerminalBlock>();
            foreach (var connector in dockConnectors)
            {
                GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, x => x.IsSameConstructAs(connector));
            }

            // Get "Station Cargo 1"
            var targetContainer = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("Station Cargo 1", targetContainer, x => x is IMyCargoContainer);
            var backupContainer = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("Station Cargo 2", backupContainer, x => x is IMyCargoContainer);

            if (targetContainer.Count == 0)
                return;
            IMyCargoContainer targetedCargoContainer = targetContainer[0] as IMyCargoContainer;
            if (targetedCargoContainer == null)
                return;
            if (targetedCargoContainer.GetInventory().IsFull)
            {
                if (backupContainer.Count == 0)
                    return;
                targetedCargoContainer = backupContainer[0] as IMyCargoContainer;
            }

            // Move items from these containers to "Station Cargo 1"
            foreach (var container in cargoContainers)
            {
                var inventory = ((IMyCargoContainer)container).GetInventory();
                for (var i = inventory.ItemCount - 1; i >= 0; i--)
                {
                    inventory.TransferItemTo(targetedCargoContainer.GetInventory(), i, null, true, null);
                }
            }

            Me.GetSurface(0).WriteText($"Last Task:\nSuccessfully moved items from {argument}\nto {targetedCargoContainer.CustomName}");
        }
    }
}
