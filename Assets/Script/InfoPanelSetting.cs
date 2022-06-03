using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class InfoPanelSetting : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private Text errorText;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    private Transform _destroyObj;
    private int puan, enerji;

    public void Setting(Transform _new, int _puan, int _enerji)
    {
        infoText.text = string.Format("{0} puana {1} enerji almak istediğinize emin misiniz?", _puan, _enerji);
        yesBtn.GetComponentInChildren<Text>().text = "Evet";
        noBtn.GetComponentInChildren<Text>().text = "Hayır";
        puan = _puan;
        enerji = _enerji;
        _destroyObj = _new;
        yesBtn.onClick.AddListener(YesBtn);
        noBtn.onClick.AddListener(NoBtn);
    }
    private void YesBtn()
    {
        int diamond = GridManager.grid.GetDiamond();
        if (diamond >= puan)
        {
            GridManager.grid.SetDiamond(puan);
            GridManager.grid.SetEnergy(enerji);
            Destroy(_destroyObj.gameObject);
        }
        else 
        {
            errorText.gameObject.SetActive(true);
            errorText.text = string.Format("Enerji almak için {0} puanınız olmalıdır.", puan);
        }
    }
    private void NoBtn()
    {
        Destroy(_destroyObj.gameObject);
    }
}
