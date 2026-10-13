using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
            SetupDeathButton();
        }
    }

    private void SetupDeathButton()
    {
        Button button = deathPanel.GetComponentInChildren<Button>(true);
        if (button == null)
            return;

        string label = SaveSystem.Exists() ? "Load last save" : "Back to menu";

        Text legacyText = button.GetComponentInChildren<Text>(true);
        if (legacyText != null)
            legacyText.text = label;

        TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>(true);
        if (tmpText != null)
            tmpText.text = label;

        GameInput.Select(button.gameObject);
    }

    public void BackToMenu()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.RespawnAfterDeath();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

}
