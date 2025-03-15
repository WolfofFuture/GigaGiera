using UnityEngine;

public class AnotherPlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float smoothTime = 0.1f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private Vector2 velocity, currentVelocity;

    // Update is called once per frame

    private void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector2 targetVelocity = input * movementSpeed;

        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref currentVelocity, smoothTime);

        rb.linearVelocity = velocity;
    }
}
