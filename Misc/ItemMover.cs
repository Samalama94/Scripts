void Main(string argument)
{
    var cargoContainers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);
    cargoContainers = cargoContainers.OrderBy(x => x.CustomName).ToList();

    if (cargoContainers.Count <= 0)
        return;

    IMyCargoContainer largestCargoContainer = GetLargestCargoContainer(cargoContainers);

    foreach (var container in cargoContainers)
    {
        if (container == largestCargoContainer || !targetedCargoContainer.IsSameConstructAs(container))
            continue;

        var inventory = ((IMyCargoContainer)container).GetInventory();

        for (var i = inventory.ItemCount - 1; i >= 0; i--)
        {
            var item = inventory.GetItemAt(i);
            inventory.TransferItemTo(largestCargoContainer.GetInventory(), i, null, true, null);
        }
    }
}

IMyCargoContainer GetLargestCargoContainer(List<IMyTerminalBlock> cargoContainers)
{
    IMyCargoContainer largestCargoContainer = (IMyCargoContainer)cargoContainers[0];
    var maxSize = largestCargoContainer.GetInventory().MaxVolume;

    foreach (var container in cargoContainers)
    {
        var curCargo = (IMyCargoContainer)container;
        var curSize = curCargo.GetInventory().MaxVolume;

        if (curSize > maxSize)
        {
            maxSize = curSize;
            largestCargoContainer = curCargo;
        }
    }

    return largestCargoContainer;
}

