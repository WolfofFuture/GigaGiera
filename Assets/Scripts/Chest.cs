using UnityEngine;

public class Chest : Collectable
{
    public Sprite emptyChest;
    public int cashAmount = 5;

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;

            // Poprawiona referencja do singletona GameManager
            GameManager.Instance.ShowText("+" + cashAmount + " CASH!", 25, Color.green, transform.position, Vector3.up * 25, 1.5f);
        }
    }
}
