using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Transform inventoryParent;
    public List<Image> itemSlots = new List<Image>();
    public List<string> itemIDs = new List<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        itemSlots.Clear();
        itemIDs.Clear();

        foreach (Transform frame in inventoryParent)
        {
            foreach (Transform child in frame)
            {
                Image icon = child.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = null;
                    icon.color = new Color(1, 1, 1, 0);
                    itemSlots.Add(icon);
                    itemIDs.Add(null);
                }
            }
        }
    }

    public void AddItem(Sprite icon, string itemID)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].sprite == null)
            {
                itemSlots[i].sprite = icon;
                itemSlots[i].color = Color.white;
                itemIDs[i] = itemID;
                return;
            }
        }
    }

    public bool HasItem(string itemID)
    {
        return itemIDs.Contains(itemID);
    }

    public void RemoveItem(string itemID)
    {
        for (int i = 0; i < itemIDs.Count; i++)
        {
            if (itemIDs[i] == itemID)
            {
                itemSlots[i].sprite = null;
                itemSlots[i].color = new Color(1, 1, 1, 0);
                itemIDs[i] = null;
                return;
            }
        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].sprite = null;
            itemSlots[i].color = new Color(1, 1, 1, 0);
            itemIDs[i] = null;
        }
    }
}
