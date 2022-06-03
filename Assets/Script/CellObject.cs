using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellObject", menuName = "ScriptableObjects/CellObject", order = 1)]
public class CellObject : ScriptableObject
{
    public ObjectItem itemData = new ObjectItem();
    public CellObjectType type;    
    public Sprite[] Sprite = new Sprite[7];
}

[System.Serializable]
public class ObjectItem
{
    public int objectID;
    public string objectName;    
    public ObjectItem()
    {
        objectName = "";
        objectID = -1;
    }
    public ObjectItem(CellObject cellItem)
    {
        objectName = cellItem.name;
        objectID = cellItem.itemData.objectID;
    }
}
public enum CellObjectType
{ 
    Çilek,
    Muz,
    Blackberry,
    Portakal,
    Kakao
}