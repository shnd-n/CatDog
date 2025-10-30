using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;         // 슬롯 프리팹 (InventorySlot)
    public Transform gridParent;          // InventoryPanel (Grid Layout Group이 있는 곳)
    public int slotCount = 20;            // 슬롯 개수

    private List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        CreateSlots();

    }

    void CreateSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            slot.name = $"Slot_{i}";
            slots.Add(slot);
        }
    }

    public void AddItem(Sprite itemIcon)
    {
        foreach (var slot in slots)
        {
            Image icon = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (icon.sprite == null)
            {
                icon.sprite = itemIcon;
                icon.color = Color.white;
                break;
            }
        }
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        Image icon = slots[index].transform.Find("ItemIcon").GetComponent<Image>();
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0); // 투명 처리
    }
}
