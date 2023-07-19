// Using IsSameConstructAs to verify that a container is a part of the same construct at 'Me' before transferring items.
void Main(string argument)
{
    var cargoContainers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);
    cargoContainers = cargoContainers.OrderBy(x => x.CustomName).ToList();

    if (cargoContainers.Count <= 0)
        return;

    IMyCargoContainer targetedCargoContainer = GetTargetedCargoContainer(cargoContainers);

    if (targetedCargoContainer == null)
        return;

    foreach (var container in cargoContainers)
    {
        if (container == targetedCargoContainer || !targetedCargoContainer.IsSameConstructAs(container))
            continue;

        var inventory = ((IMyCargoContainer)container).GetInventory();

        for (var i = inventory.ItemCount - 1; i >= 0; i--)
        {
            var item = inventory.GetItemAt(i);
            inventory.TransferItemTo(targetedCargoContainer.GetInventory(), i, null, true, null);
        }
    }
}

IMyCargoContainer GetTargetedCargoContainer(List<IMyTerminalBlock> cargoContainers)
{
    IMyCargoContainer targetedCargoContainer = null;

    foreach (var container in cargoContainers)
    {
        if (container.CustomName == "Station Cargo 1")
        {
            targetedCargoContainer = (IMyCargoContainer)container;
            break;
        }
    }

    return targetedCargoContainer;
}