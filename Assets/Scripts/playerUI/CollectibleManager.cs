//using System.Collections.Generic;
//using UnityEngine;

//public class CollectibleManager : MonoBehaviour
//{
//    public static CollectibleManager Instance;

//    private HashSet<string> collectedItems = new HashSet<string>();

//    private void Awake()
//    {
//        if (Instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        LoadCollectedItems();
//    }

//    public bool IsCollected(string itemID)
//    {
//        return collectedItems.Contains(itemID);
//    }

//    public void Collect(string itemID)
//    {
//        collectedItems.Add(itemID);
//        SaveCollectedItems();
//    }

//    public IEnumerable<string> GetCollectedItems()
//    {
//        return collectedItems;
//    }

//    private void SaveCollectedItems()
//    {
//        PlayerPrefs.SetString("CollectedItems", string.Join(",", collectedItems));
//    }

//    private void LoadCollectedItems()
//    {
//        collectedItems.Clear();
//        string saved = PlayerPrefs.GetString("CollectedItems", "");
//        if (!string.IsNullOrEmpty(saved))
//        {
//            string[] ids = saved.Split(',');
//            foreach (string id in ids)
//            {
//                collectedItems.Add(id);
//            }
//        }
//    }

//    public void ResetCollected()
//    {
//        collectedItems.Clear();
//        PlayerPrefs.DeleteKey("CollectedItems");
//    }

//    public void UseItem(string itemID)
//    {
//        if (collectedItems.Contains(itemID))
//        {
//            collectedItems.Remove(itemID);
//            SaveCollectedItems();
//        }
//    }

//}
