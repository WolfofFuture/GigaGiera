using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject portal;       // Portal, który bêdzie wy³¹czony/w³¹czony
    public GameObject portalBlockedGraphic;
    public Enemy[] enemies;         // Przeciwnicy w pokoju
    public Collider2D roomTrigger;  // Trigger wejœcia gracza do pokoju

    private bool playerInside = false;

    private void Start()
    {
        if (portal != null)
        {
            portal.SetActive(false); // Portal wy³¹czony na start
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerInside) return;

        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            ActivateEnemies();

            if (portalBlockedGraphic != null)
                portalBlockedGraphic.SetActive(true);
        }
    }


    void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.PlayerEnteredRoom();
                enemy.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        bool allDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            OpenPortal();
            playerInside = false;

            if (portalBlockedGraphic != null)
                portalBlockedGraphic.SetActive(false);
        }
    }

    void OpenPortal()
    {
        if (portal != null)
        {
            portal.SetActive(true); // W³¹cz portal po pokonaniu wszystkich wrogów
        }
    }
}
