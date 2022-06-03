using UnityEngine.UI;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    public CellObject item;
    public int level = -1;
    public Image image;
    public InventorySlot slot;
    public void SettingCell(ObjectItem _newItem)
    {
        item = GridManager.grid.database.CellObjects[_newItem.objectID];
        level = slot.level;
        image.sprite = item.Sprite[level];
    }
    private void FixedUpdate()
    {
        SettingCell(item.itemData);
    }
    public void Setting()
    {
        slot = GridManager.grid.slotsOnInterface[transform.gameObject];
    }
}
