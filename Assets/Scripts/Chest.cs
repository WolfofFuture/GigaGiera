using JetBrains.Annotations;
using UnityEngine;

public class Chest : Collectable //Inheritence, The chest inherits from Collectable script which inherits the collidable and etc.
{
    public Sprite emptyChest;
    public int cashAmount = 5;

    protected override void OnCollect()
    {

        if(!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite=emptyChest;
            GameManager.instance.ShowText("+" + cashAmount + " CASH!",25,Color.green,transform.position,Vector3.up * 25, 1.5f);
        }


        //collected = true;
        //base.OnCollect();
        //Debug.Log("CASH CASH CASH");
    }
}
