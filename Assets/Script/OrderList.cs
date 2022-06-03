using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrdererList
{
    Default = 0,
    John = 1,
    Robert = 2,
    David = 3,
    Charles = 4,
    Linda = 5,
    Mary = 6,
    Patricia = 7,
    Sandra = 8
}

[System.Serializable]
public class OrderObject
{
    public CellObject orderItem;
    public int ItemLevel;
    public OrdererList orderer;
    public Sprite orderImage;
    public OrderObject()
    {
        orderItem = null;
        ItemLevel = -1;
        orderer = OrdererList.Default;
        orderImage = null;
    }
    public OrderObject(CellObject cellItem,int level,OrdererList _orderer,Sprite _image)
    {
        orderItem = cellItem;
        ItemLevel = level;
        orderer = _orderer;
        orderImage = _image;
    }
}
public class OrderList : MonoBehaviour
{
    public static OrderList instance;
    public CellObjectData database;  
    public Sprite[] menImage = new Sprite[4];
    public Sprite[] womenImage = new Sprite[4];
    private OrderObject orderObject;
    private OrdererList[] _newOrderer;
    private Sprite _newImage;
    [SerializeField] private Transform orderPrefab;
    [SerializeField] private Transform orderContain;
    public Dictionary<GameObject, OrderObject> SlotsOnOrderList = new Dictionary<GameObject, OrderObject>();
    public InventoryObject playerInventory;
    public bool reset = false;

    private void Start()
    {
        instance = this;
        _newOrderer = (OrdererList[])System.Enum.GetValues(typeof(OrdererList));
    }    
    public OrderObject GetOrderObject(GameObject _obj)
    {
        return SlotsOnOrderList[_obj];
    }
    private void FixedUpdate()
    {
        if (playerInventory.GetSlots.Length > 0 || reset)
        {
            Controller();
            reset = false;
        }

        if (orderContain.childCount < 4)
        {
            SetOrder();
            CreateOrder(orderObject);
        }
    }

    public void Controller()
    {
        for (int i = 0; i < orderContain.childCount; i++)
        {
            OrderObject _new = GetOrderObject(orderContain.GetChild(i).gameObject);
            Transform _newTr = orderContain.GetChild(i);
            for (int j = 0; j < playerInventory.GetSlots.Length; j++)
            {
                if (playerInventory.GetSlots[j].item.objectID == _new.orderItem.itemData.objectID)
                {
                    if (playerInventory.GetSlots[j].level == _new.ItemLevel)
                    {
                        _newTr.GetComponent<OrderObjectSetting>().OrderFound(_newTr);
                        _newTr.GetComponent<OrderObjectSetting>()._contain = orderContain;
                        _newTr.GetComponent<OrderObjectSetting>()._slot = playerInventory.GetSlots[j];
                    }
                }
            }
        }
    }
    void SetOrder()
    {
        var level = UnityEngine.Random.value < 0.75f ? 5 : 4;
        level = UnityEngine.Random.value < 0.95f ? 4 : 6;
        var random1 = UnityEngine.Random.Range(0, database.CellObjects.Length);
        var random2 = UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(OrdererList)).Length);
        if (random2 < 5)
        {
            var random3 = UnityEngine.Random.Range(0, menImage.Length);
            _newImage = menImage[random3];
        }
        else
        {
            var random3 = UnityEngine.Random.Range(0, womenImage.Length);
            _newImage = womenImage[random3];
        }

        orderObject = new OrderObject(database.CellObjects[random1], level, _newOrderer[random2],_newImage);
    }
    void CreateOrder(OrderObject _order)
    {
        Transform _newobject = Instantiate(orderPrefab, orderContain);
        _newobject.localPosition = Vector3.zero;
        _newobject.GetComponent<OrderObjectSetting>().SetOrderObject(_order);
        SlotsOnOrderList.Add(_newobject.gameObject, _order);
    }
}
