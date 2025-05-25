using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform LookAt;
    public float boundX = 0.15f;
    public float boundY = 0.05f;
    private static CameraMotor instance; //so camera doesnt dissapear

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        if (LookAt == null) return; // 🛡️ zapobiegamy błędowi MissingReferenceException

        Vector3 delta = Vector3.zero;

        float deltaX = LookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            delta.x = transform.position.x < LookAt.position.x ? deltaX - boundX : deltaX + boundX;
        }

        float deltaY = LookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            delta.y = transform.position.y < LookAt.position.y ? deltaY - boundY : deltaY + boundY;
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }

    // 🧭 Metoda do ustawienia celu śledzenia (np. z GameManagera)
    public void SetTarget(Transform newTarget)
    {
        LookAt = newTarget;
    }
}
