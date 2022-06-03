using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    private int sumDiamond;
    private bool infoPanel = false;
    public void SetDiamond(int _amount)
    {
        sumDiamond -= _amount;
        OrderSumDiamond.sumDiamond = sumDiamond;
    }
    public void SetEnergy(int _amount)
    {
        playerEnergy += _amount;
    }
    [SerializeField] private int enerjiPuanı, enerji;
    public int GetDiamond()
    {
        sumDiamond = OrderSumDiamond.sumDiamond;
        return sumDiamond;
    }
    [SerializeField] private Text sumDiamondText;
    public static GridManager grid;
    [SerializeField] private int row, column;
    [SerializeField] private Transform infoPanelPrefab;
    [SerializeField] private Transform[] cellPrefab;
    [SerializeField] public Transform cellInsidePrefab;
    [SerializeField] public CellObjectData database;    
    [SerializeField] private Text energyText,reamingText;
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
    private GameObject _cellGO;
    int playerEnergy = 100;
    float countdownTo = 90f;
    private void Awake()
    {
        if (grid == null)
            grid = this;

        SpawnCell();

        energyText.text = "";
        reamingText.text = "";
        infoPanel = false;
    }
    private void FixedUpdate()
    {
        reamingText.text = string.Format("Kalan Enerji: {0}", playerEnergy.ToString());
        if (playerEnergy < 100)
            EnergyText();
        sumDiamondText.text = GetDiamond().ToString();
    }
    void EnergyText()
    {
        if (countdownTo > 0)
            countdownTo -= Time.deltaTime;
        if (countdownTo <= 0)
        { 
            playerEnergy++;
            countdownTo = 90f;
            energyText.text = "";
            return;
        }
        float minutes = Mathf.FloorToInt(countdownTo / 60);
        float seconds = Mathf.FloorToInt(countdownTo % 60);
        energyText.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
    }
    public void SpawnCell()
    {
        for (int i = 1; i < row + 1; i++)
        {
            for (int j = 1; j < column + 1; j++)
            {
                if (i % 2 == 0)
                {
                    if (j % 2 == 1)
                        _cellGO = Instantiate(cellPrefab[0], transform).gameObject;

                    else
                        _cellGO = Instantiate(cellPrefab[1], transform).gameObject;
                }
                else
                {
                    if (j % 2 == 1)
                        _cellGO = Instantiate(cellPrefab[1], transform).gameObject;

                    else
                        _cellGO = Instantiate(cellPrefab[0], transform).gameObject;
                }
                _cellGO.name = (i) + "x" + (j);
            }
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject _cell = transform.GetChild(i).gameObject;
            AddEvent(_cell, EventTriggerType.PointerEnter, delegate { EmptySlotEnter(_cell); });
            AddEvent(_cell, EventTriggerType.PointerExit, delegate { EmptySlotExit(_cell); });
        }

        Inventory newContainer = inventory.Load();
        if (newContainer == null)
        {
            Debug.Log("Kayıtlı bir oyun yok!");
            inventory.Clear();
        }
        else
        {
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                inventory.GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].level);
                if (inventory.GetSlots[i].GetItemObject() != null)
                {
                    SlotSetting(inventory.GetSlots[i].GetItemObject(), newContainer.Slots[i].level, i);
                }
            }
            if (PlayerPrefs.GetInt("Diamond") != OrderSumDiamond.sumDiamond)
                OrderSumDiamond.sumDiamond = PlayerPrefs.GetInt("Diamond");
        }
    }
    public void EmptySlotEnter(GameObject obj)
    {
        MouseData.emptySlotHoveredOver = obj;
    }
    public void EmptySlotExit(GameObject obj)
    {
        MouseData.emptySlotHoveredOver = null;
    }
    public void StartBtn()
    {
        if (playerEnergy > 0 && inventory.EmptySlotCount > 0)
        {
            int sayac = UnityEngine.Random.Range(0, row * column);
            var slot = inventory.EmptySlot(sayac);
            if (!slot)
            {
                playerEnergy--;
                CreateSlots(sayac);
            }
            else
            {
                StartBtn();
            }
        }
    }
    public void CreateSlots(int i)
    {
        var random = UnityEngine.Random.Range(0, database.CellObjects.Length);
        var level = UnityEngine.Random.value < 0.95f ? 0 : 1;
        var item = database.CellObjects[random];
        SlotSetting(item, level, i);
    }
    public void SlotSetting(CellObject item, int level, int i)
    {
        var obj = Instantiate(cellInsidePrefab, transform.GetChild(i));
        obj.localPosition = Vector3.zero;
        AddEvent(obj.gameObject, EventTriggerType.PointerEnter, delegate { OnEnter(obj.gameObject); });
        AddEvent(obj.gameObject, EventTriggerType.PointerExit, delegate { OnExit(obj.gameObject); });
        AddEvent(obj.gameObject, EventTriggerType.BeginDrag, delegate { OnDragStart(obj.gameObject); });
        AddEvent(obj.gameObject, EventTriggerType.EndDrag, delegate { OnDragEnd(obj.gameObject); });
        AddEvent(obj.gameObject, EventTriggerType.Drag, delegate { OnDrag(obj.gameObject); });
        CellInfo info = obj.GetComponent<CellInfo>();
        info.item = item;
        inventory.AddItem(new ObjectItem(info.item), level, i);
        inventory.GetSlots[i].slotDisplay = obj.gameObject;
        slotsOnInterface.Add(obj.gameObject, inventory.GetSlots[i]);
        info.Setting();
    }
    public void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (!trigger) { Debug.LogWarning("No EventTrigger component found!"); return; }
        var eventTrigger = new EventTrigger.Entry { eventID = type };
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
        
        if (MouseData.emptySlotHoveredOver && !MouseData.slotHoveredOver)
        {
            inventory.SwapEmpty(slotsOnInterface[obj], MouseData.emptySlotHoveredOver);
        }
    }
    public void OnDrag(GameObject obj)
    {
        MouseData.slotMouseDragged = obj;
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    private GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].item.objectID >= 0)
        {
            tempItem = new GameObject();
            tempItem.transform.SetParent(transform.parent);
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(75, 75);
            rt.localScale = new Vector3(1, 1, 1);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].GetItemObject().Sprite[slotsOnInterface[obj].level];
            img.raycastTarget = false;
        }
        return tempItem;
    }
    public int GetChildPosition(Transform _obj)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == _obj)
                return i;
        }
        return -1;
    }
    public void AddEnergy()
    {
        if (!infoPanel)
        {
            Transform _new = Instantiate(infoPanelPrefab, transform.parent);
            _new.GetComponent<InfoPanelSetting>().Setting(_new, enerjiPuanı, enerji);
        }
    }
}

public static class MouseData
{
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
    public static GameObject slotMouseDragged;
    public static GameObject emptySlotHoveredOver;
}

