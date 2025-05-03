using UnityEngine;

public abstract class Mover : Fighter
{
    // FIELDS
    protected BoxCollider2D boxCollider; //before it was private
    private Vector3 moveDelta;
    private RaycastHit2D hit;
    protected float ySpeed = 0.75f;
    protected float xSpeed = 1.0f;

    //Functions
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

    }

    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);

        // Swap sprite directrion
        if (moveDelta.x > 0)
            transform.localScale = Vector3.one;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // add push vector if any
        moveDelta += pushDirection;

        // reduce the push force every frame based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        // Make sure we can move in this direction by casting a box there first if the box returns null we're free to move
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {

            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // 
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        }
    }
}
