using UnityEngine;
using UnityEngine.UI;

public class Stove : MonoBehaviour
{
    [SerializeField] private Slider stoveSlider;
    
    void Update()
    {
        PrintStoveSliderValue();
    }

    private void PrintStoveSliderValue()
    {
        //Debug.Log((int)stoveSlider.value + " °C");
    }
}
