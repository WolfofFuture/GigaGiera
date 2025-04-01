using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] private float movementSpeed = 3f;
    private BoxCollider2D boxCollider;
    //private Vector2 movementDirection;
    private Vector3 moveDelta;
    private RaycastHit2D hit;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    /*void Update()
    {
        //movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }*/
    void FixedUpdate()
    {
        //rb.linearVelocity = movementDirection.normalized * movementSpeed * Time.fixedDeltaTime; - póŸniej pewnie bêdzie mo¿na usun¹æ ten i inne komentarze
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDelta = new Vector3(x,y,0);
        if (moveDelta.x > 0)
            transform.localScale = Vector3.one;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask());//DODAÆ BLOKUJ¥CE MASKI W OBU
        if (hit.collider == null)
        {
        transform.Translate(0,moveDelta.y * Time.deltaTime,0);
        }
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.x), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask());
        if (hit.collider == null)
        {
            transform.Translate(moveDelta.x * Time.deltaTime,0, 0);
        }
    }
}
