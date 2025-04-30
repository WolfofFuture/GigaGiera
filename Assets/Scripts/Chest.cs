using JetBrains.Annotations;
using UnityEngine;

public class Chest : Collectable //Inheritence, The chest inherits from Collectable script which inherits the collidable and etc.
{
    public Sprite emptyChest;
    public int Cash = 5;

    protected override void OnCollect()
    {

        if(!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite=emptyChest;
            Debug.Log("Grant " + Cash + " Cash!");
        }


        //collected = true;
        //base.OnCollect();
        //Debug.Log("CASH CASH CASH");
    }
}
