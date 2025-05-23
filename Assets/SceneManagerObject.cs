using UnityEngine;
using UnityEngine.SceneManagement;

public class PotionSpawner : MonoBehaviour
{
    public GameObject speedPotionPrefab; // przypisz prefab mikstury w inspektorze
    private GameObject currentPotionInstance;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room4") // wpisz nazwê swojego pokoju
        {
            if (currentPotionInstance == null)
            {
                currentPotionInstance = Instantiate(speedPotionPrefab, new Vector3(2, 0, 0), Quaternion.identity);
            }
            else
            {
                currentPotionInstance.SetActive(true);
            }
        }
        else
        {
            if (currentPotionInstance != null)
                currentPotionInstance.SetActive(false);
        }
    }
}
