using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform LookAt;
    public float boundX = 0.15f;
    public float boundY = 0.05f;

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        // This is to check if we're inside the bounds on the X axis
        float deltaX = LookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < LookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }

        }

        // This is to check if we're inside the bounds on the Y axis
        float deltaY = LookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
            {
                if (transform.position.y < LookAt.position.y)
                {
                    delta.y = deltaY - boundY;
                }
                else
                {
                    delta.y = deltaY + boundY;
                }

            }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }
}
