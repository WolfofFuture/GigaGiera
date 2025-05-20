using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public string itemID;
    public Sprite icon;

    private void Start()
    {
        if (CollectibleManager.Instance != null && CollectibleManager.Instance.IsCollected(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(itemID))
            {
                CollectibleManager.Instance.Collect(itemID);
            }

            InventoryManager.Instance.AddItem(icon);
            Destroy(gameObject);
        }
    }
}
