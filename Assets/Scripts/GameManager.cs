using NUnit.Framework;
using System.Collections.Generic;
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
        //PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
    }
    //Zasoby
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    //public List<int> xpTables;

    //Referencje
    public Player player;

    public int coins;
    //public int exp;

    //Zapis
    /*
     * INT playerSkin
     * INT coins
     * Int exp
     * INT weapon
     */
    public void SaveState()
    {
        string s = "";
        s += "0"+"|";
        s += coins.ToString() + "|";
        //s += exp.ToString() + "|";
        //s += "0";

        PlayerPrefs.SetString("SaveState", s);
        //Debug.Log("SaveState");
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        //Zmiana wygladu gracza pod koniec gry ale to pozniej
        coins = int.Parse(data[1]);
        //exp = int.Parse(data[-]);

        //Debug.Log("LoadState");
    }
}
