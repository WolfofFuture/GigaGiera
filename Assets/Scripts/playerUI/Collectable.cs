using UnityEngine;

public class Collectable : Collidable
{
    public string itemID;
    protected bool collected;

    protected override void Start()
    {
        base.Start();

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
        CollectibleManager.Instance.Collect(itemID);
        gameObject.SetActive(false);
    }
}
