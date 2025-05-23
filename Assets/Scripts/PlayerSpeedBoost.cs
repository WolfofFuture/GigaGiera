using System.Collections;
using UnityEngine;

public class PlayerSpeedBoost : MonoBehaviour
{
    private Mover mover;

    private float originalXSpeed;
    private float originalYSpeed;

    private Coroutine speedBoostCoroutine;

    private void Awake()
    {
        mover = GetComponent<Mover>();
        if (mover == null)
        {
            Debug.LogError("PlayerSpeedBoost wymaga komponentu Mover na tym samym obiekcie.");
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
            ResetSpeed();
        }
        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        // zapamiêtaj oryginalne prêdkoœci
        originalXSpeed = mover.xSpeed;
        originalYSpeed = mover.ySpeed;

        // podwój prêdkoœci (lub wg mno¿nika)
        mover.xSpeed *= multiplier;
        mover.ySpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        ResetSpeed();

        speedBoostCoroutine = null;
    }

    private void ResetSpeed()
    {
        mover.xSpeed = originalXSpeed;
        mover.ySpeed = originalYSpeed;
    }
}
