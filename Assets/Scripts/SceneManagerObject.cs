using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerObject : MonoBehaviour
{
    private static SceneManagerObject instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            EnableCollectableItemsRecursive(root.transform);
        }
    }

    private void EnableCollectableItemsRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            var collectable = child.GetComponent<CollectableItem>();
            if (collectable != null && !child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                Debug.Log("W³¹czono collectable: " + child.name);
            }

            // Rekurencja dla dzieci
            if (child.childCount > 0)
            {
                EnableCollectableItemsRecursive(child);
            }
        }
    }
}
