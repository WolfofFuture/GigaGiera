using UnityEngine;

// Ikony przedmiotow potrzebne przy wczytywaniu zapisu (w zapisie trzymamy tylko ID przedmiotu).
public class ItemIcons : ScriptableObject
{
    public Sprite key;
    public Sprite bomb;
    public Sprite speedPotion;
    public Sprite healthPotion;

    private static ItemIcons cached;

    public static ItemIcons Load()
    {
        if (cached == null)
            cached = Resources.Load<ItemIcons>("ItemIcons");
        return cached;
    }

    public Sprite Get(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return null;

        if (itemID.StartsWith("key_")) return key;
        if (itemID.StartsWith("bomb_")) return bomb;
        if (itemID.StartsWith("speed_potion")) return speedPotion;
        if (itemID.StartsWith("health_potion")) return healthPotion;
        return null;
    }
}
