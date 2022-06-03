using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InventoryObject playerInventory;
    [SerializeField] private GridManager grid;
    public void SaveLevel()
    {
        playerInventory.Save();
        string path = Application.persistentDataPath + playerInventory.savePath;
        if (File.Exists(path))
            Debug.Log("Kayıt başarılı");

        PlayerPrefs.SetInt("Diamond", OrderSumDiamond.sumDiamond);
    }
    public void ResetLevel()
    {
        string path = Application.persistentDataPath + playerInventory.savePath;
        if (File.Exists(path))
            File.Delete(path);
        else
            Debug.LogError("Save file not found in " + path);

        playerInventory.Clear();
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        grid.SpawnCell();
        OrderList.instance.reset = true;

    }
    public void QuitLevel()
    {
        Application.Quit();
    }
}
