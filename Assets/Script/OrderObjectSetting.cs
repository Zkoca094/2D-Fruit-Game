using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderSumDiamond
{
    public static int sumDiamond;
}
public class OrderObjectSetting : MonoBehaviour
{
    [SerializeField] private Image ordererProfile;
    [SerializeField] private Image orderObjectProfile;
    [SerializeField] private Image checkMark;
    [SerializeField] private Text prizeText;
    [SerializeField] private Button serveBtn;
    public Transform _contain;
    public InventorySlot _slot;
    public void SetOrderObject(OrderObject _order)
    {
        ordererProfile.sprite = _order.orderImage;
        orderObjectProfile.sprite = _order.orderItem.Sprite[_order.ItemLevel];
        checkMark.gameObject.SetActive(false);
        serveBtn.gameObject.SetActive(false);

        switch (_order.ItemLevel)
        { 
            case 4:
                prizeText.text = string.Format("x{0}", 15);
                serveBtn.onClick.AddListener(() => ServeBtn(15));
                break;
            case 5:
                prizeText.text = string.Format("x{0}", 30);
                serveBtn.onClick.AddListener(() => ServeBtn(30));
                break;
            case 6:
                prizeText.text = string.Format("x{0}", 60);
                serveBtn.onClick.AddListener(() => ServeBtn(60));
                break;
        }
    }
    public void OrderFound(Transform _orderobj)
    {
        _orderobj.GetComponent<OrderObjectSetting>().checkMark.gameObject.SetActive(true);
        _orderobj.GetComponent<OrderObjectSetting>().serveBtn.gameObject.SetActive(true);
        _orderobj.GetComponent<OrderObjectSetting>().serveBtn.onClick.AddListener(() => OrderServe(_orderobj));
    }
    void SetDefaults(Transform _contain)
    {
        for (int i = 0; i < _contain.childCount; i++)
        {
            _contain.GetChild(i).Find("CheckMark").gameObject.SetActive(false);
            _contain.GetChild(i).Find("Serve_Btn").gameObject.SetActive(false);
        }
    }
    public void OrderServe(Transform _obj)
    {
        Destroy(_obj.gameObject);
        Destroy(_slot.slotDisplay.gameObject);
        _slot.UpdateSlot(new ObjectItem(), -1);
        SetDefaults(_contain);
    }
    void ServeBtn(int _diamond)
    {
        OrderSumDiamond.sumDiamond += _diamond;
    }
}
