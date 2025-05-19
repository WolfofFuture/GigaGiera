using UnityEngine;

public class TankAI : MonoBehaviour
{
    public AudioClip TankS1;
    private AudioSource audioSource;

    private Transform player;
    public float speed = 1.5f;

    void Start()
    {  
        Player playerScript = FindObjectOfType<Player>();
        if (playerScript != null)
        {
            player = playerScript.transform;
        }
        else
        {
            Debug.LogWarning("COAACH! NIE MA TUTAJ GRACZA!! COAAACH!");
        }

        audioSource = GetComponent<AudioSource>();
        Invoke("Roar", 0.5f);


    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    void Roar()
    {
        if (TankS1 != null)
            audioSource.PlayOneShot(TankS1);
    }

}
