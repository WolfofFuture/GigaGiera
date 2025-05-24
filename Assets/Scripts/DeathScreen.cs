using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathWatcher : MonoBehaviour
{
    public HealthBar healthBar;      // pod³¹cz w Inspectorze
    public GameObject deathPanel;    // pod³¹cz panel œmierci
    public int maxHealth = 100;      // ustaw max HP

    [HideInInspector]
    public bool isDead = false;      // flaga do blokady ruchu

    private bool deathShown = false;

    void Start()
    {
        deathPanel.SetActive(false);
        isDead = false;
        deathShown = false;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
    }

    void Update()
    {
        if (!deathShown && healthBar.slider.value <= 0)
        {
            deathShown = true;
            isDead = true;
            deathPanel.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

}
