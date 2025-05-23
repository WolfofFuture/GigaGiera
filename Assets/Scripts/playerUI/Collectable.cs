using UnityEngine;

public class Collectable : Collidable
{
    public string itemID;
    protected bool collected;

    private CollectablePickupSound pickupSound;

    protected override void Start()
    {
        base.Start();

        pickupSound = GetComponent<CollectablePickupSound>();

        if (CollectibleManager.Instance != null && CollectibleManager.Instance.IsCollected(itemID))
        {
            gameObject.SetActive(false);
            collected = true;
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (!collected && coll.CompareTag("Player"))
        {
            OnCollect();
        }
    }

    protected virtual void OnCollect()
    {
        collected = true;

        // Odtwórz dŸwiêk przy podniesieniu
        if (pickupSound != null)
        {
            pickupSound.PlayPickupSound();
        }

        CollectibleManager.Instance.Collect(itemID);
        gameObject.SetActive(false);
    }
}
