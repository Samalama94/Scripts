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

    Me.GetSurface(0).WriteText($"Last Task:\nSuccesfully moved items from {argument}\nto {targetedCargoContainer.CustomName}");
}