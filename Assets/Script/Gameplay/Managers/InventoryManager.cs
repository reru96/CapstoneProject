using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager: Injectable<InventoryManager>
{
    public RunTimeInventory runInventory { get; private set; }
    public PermanentInventory permanentInventory { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (runInventory == null)
            runInventory = new RunTimeInventory();

        if (permanentInventory == null)
            permanentInventory = new PermanentInventory();

    }

    public void ResetRunInventory() => runInventory.ResetInventory();

    public void SavePassiveInventory() =>
        SaveSystem.SavePermanentInventory(permanentInventory);

    public void LoadPassiveInventory() =>
        SaveSystem.LoadPermanentInventory(permanentInventory);
}
