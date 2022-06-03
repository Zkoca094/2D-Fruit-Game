using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public Action<InventorySlot> onAfterUpdated;
    [System.NonSerialized]
    public Action<InventorySlot> onBeforeUpdated;
    public ObjectItem item;
    public int level;

    public CellObject GetItemObject()
    {
        return item.objectID >= 0 ? GridManager.grid.database.CellObjects[item.objectID] : null;
    }
    public InventorySlot() => UpdateSlot(new ObjectItem(), 0);
    public InventorySlot(ObjectItem item, int amount) => UpdateSlot(item, amount);
    public void RemoveItem() => UpdateSlot(new ObjectItem(), 0);
    public void UpdateSlot(ObjectItem itemValue, int amountValue)
    {
        onBeforeUpdated?.Invoke(this);
        item = itemValue;
        level = amountValue;
        onAfterUpdated?.Invoke(this);
    }
}