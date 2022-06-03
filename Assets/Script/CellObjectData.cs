using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellObject_Database", menuName = "ScriptableObjects/CellObject Database", order = 1)]
public class CellObjectData : ScriptableObject
{
    public CellObject[] CellObjects;

    public void OnValidate()
    {
        for (int i = 0; i < CellObjects.Length; i++)
        {
            CellObjects[i].itemData.objectID = i;
        }
    }
}

