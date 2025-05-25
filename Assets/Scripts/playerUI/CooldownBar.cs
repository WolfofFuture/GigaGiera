using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    public Slider slider;

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogError("CooldownBar: Brak komponentu Slider na tym obiekcie!");
            }
        }
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f; // start — pasek pe³ny (atak dostêpny)
    }

    // value od 0 do 1, gdzie 1 = cooldown zakoñczony (gotowy do ataku)
    public void SetCooldownProgress(float progress)
    {
        slider.value = Mathf.Clamp01(progress);
    }
}
