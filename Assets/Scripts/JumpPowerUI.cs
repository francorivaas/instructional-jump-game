using UnityEngine;
using UnityEngine.UI;

public class JumpPowerUI : MonoBehaviour
{
    public Slider slider;

    void Awake()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        SetVisible(false);
    }

    public void SetValue(float value)
    {
        slider.value = value;
        SetVisible(true);
    }

    public void SetVisible(bool visible)
    {
        slider.gameObject.SetActive(visible);
    }
}
