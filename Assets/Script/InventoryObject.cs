using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "New Inventory", menuName = "ScriptableObjects/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public CellObjectData database;
    [SerializeField]
    private Inventory Container = new Inventory();
    public InventorySlot[] GetSlots => Container.Slots;
    public bool AddItem(ObjectItem item, int amount, int position)
    {
        if (EmptySlotCount <= 0)
            return false;
        if (GetEmptySlot(position) != null)
        {
            GetEmptySlot(position).UpdateSlot(item, amount);
            return true;
        }
        else
            return false;
    }
    public InventorySlot GetEmptySlot(int i)
    {
        if (GetSlots[i].item.objectID <= -1)
        {
            return GetSlots[i];
        }
        else
            return null;
    }
    public bool EmptySlot(int i)
    {
        if (GetSlots[i].item.objectID < 0)
            return false;
        else
            return true;
    }
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.objectID <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item1 == item2)
            return;

        if (item1.level == item2.level && item1.GetItemObject().type == item2.GetItemObject().type)
        {
            item2.UpdateSlot(item1.item, item1.level + 1);
            item1.RemoveItem();
            Destroy(item1.slotDisplay);
        }
        else 
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.level);
            item1.slotDisplay.GetComponent<CellInfo>().SettingCell(temp.item);
            item2.slotDisplay.GetComponent<CellInfo>().SettingCell(item1.item);
            item2.UpdateSlot(item1.item, item1.level);
            item1.UpdateSlot(temp.item, temp.level);
        }
    }
    public void SwapEmpty(InventorySlot _slot, GameObject _obj)
    {
        int i = GridManager.grid.GetChildPosition(_obj.transform);
        var _item = GridManager.grid.database.CellObjects[_slot.item.objectID];
        var level = _slot.level;
        GridManager.grid.slotsOnInterface.Remove(_slot.slotDisplay);
        _slot.RemoveItem();
        GridManager.grid.SlotSetting(_item, level, i);
        Destroy(_slot.slotDisplay);
        GameObject _cell = MouseData.slotMouseDragged.transform.parent.gameObject;
        GridManager.grid.AddEvent(_cell, EventTriggerType.PointerEnter, delegate { GridManager.grid.EmptySlotEnter(_cell); });
        GridManager.grid.AddEvent(_cell, EventTriggerType.PointerExit, delegate { GridManager.grid.EmptySlotExit(_cell); });
    }

    [ContextMenu("Save")]
    public void Save()
    {
        #region Optional Save
        //string saveData = JsonUtility.ToJson(Container, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();
        #endregion

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public Inventory Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            #region Optional Load
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
            //file.Close();
            #endregion

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            stream.Close();
            return newContainer;
        }
        else
        {
            return null;
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[50];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].item = new ObjectItem();
            Slots[i].level = 0;
        }
    }
    public bool ContainsItem(CellObject itemObject)
    {
        return Array.Find(Slots, i => i.item.objectID == itemObject.itemData.objectID) != null;
    }
    public bool ContainsItem(int id)
    {
        return Slots.FirstOrDefault(i => i.item.objectID == id) != null;
    }
}