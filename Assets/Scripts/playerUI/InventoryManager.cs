using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Transform inventoryParent; //parent
    public List<Image> itemSlots = new List<Image>();

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

        foreach (Transform frame in inventoryParent)
        {
            // znajdü dok≥adnie jedno dziecko typu Image
            foreach (Transform child in frame)
            {
                Image icon = child.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = null;
                    icon.color = new Color(1, 1, 1, 0);
                    itemSlots.Add(icon);
                }
            }
        }
    }


    public void AddItem(Sprite icon)
    {
        foreach (Image slot in itemSlots)
        {
            if (slot.sprite == null)
            {
                slot.sprite = icon;
                slot.color = Color.white;
                return;
            }
        }
    }

    public void ResetInventory()
    {
        foreach (Image slot in itemSlots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0);
        }
    }

}
