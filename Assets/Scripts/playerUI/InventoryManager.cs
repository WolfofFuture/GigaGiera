using System.Collections.Generic;
using System.Linq;
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

        List<KeyValuePair<Vector2, Image>> slots = new List<KeyValuePair<Vector2, Image>>();

        foreach (Transform frame in inventoryParent)
        {
            foreach (Transform child in frame)
            {
                Image icon = child.GetComponent<Image>();
                if (icon != null)
                    slots.Add(new KeyValuePair<Vector2, Image>(frame.localPosition, icon));
            }
        }

        foreach (KeyValuePair<Vector2, Image> slot in slots.OrderByDescending(s => Mathf.Round(s.Key.y / 50f)).ThenBy(s => s.Key.x))
        {
            Image icon = slot.Value;
            icon.rectTransform.anchoredPosition = Vector2.zero;
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
            itemSlots.Add(icon);
            itemIDs.Add(null);
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

    public bool HasFreeSlot()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].sprite == null)
                return true;
        }
        return false;
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
                for (int j = i; j < itemSlots.Count - 1; j++)
                {
                    itemSlots[j].sprite = itemSlots[j + 1].sprite;
                    itemSlots[j].color = itemSlots[j + 1].color;
                    itemIDs[j] = itemIDs[j + 1];
                }

                int last = itemSlots.Count - 1;
                itemSlots[last].sprite = null;
                itemSlots[last].color = new Color(1, 1, 1, 0);
                itemIDs[last] = null;
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
