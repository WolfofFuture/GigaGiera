using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    public MenuMusicPlayer musicPlayer; // podpinamy w Inspectorze
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("MusicVolume", 1f); // domyœlna wartoœæ
        slider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateVolume(float value)
    {
        musicPlayer.SetVolume(value);
    }
}
