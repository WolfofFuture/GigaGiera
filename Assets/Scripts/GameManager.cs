using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;

        }
        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
        
    }

    // Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // References
    public Player player;
    // public weapon itd...
    public FloatingTextManager floatingTextManager;

    //logic
    public int cash;
    public int experience;

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 mention, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, mention, duration);
    }

    // Save State
    /*
     * INT preferedSkin
     * INT cash
     * INT experience
     * INT weaponLevel
     */
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += cash.ToString() + "|";
        s += experience.ToString() + "|";
        s += "0";

        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        //0|10|15|2

        // Change player skin
        cash = int.Parse(data[1]);
        experience = int.Parse(data[2]);
        // Change weapon level


        Debug.Log("LoadState");
    }
}
