using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stove : MonoBehaviour
{
    [SerializeField] private Slider stoveSlider;
    [SerializeField] private ParticleSystem particleStove;
    [SerializeField] private TextMeshProUGUI displayStoveTemp;

    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule particleEmission;
    void Start()
    {
        if (particleStove != null)
        {
            particleMain = particleStove.main;
            particleEmission = particleStove.emission;
        }
    }

    void Update()
    {
        UpdateParticleProperties();
        StoveSliderValue();
    }

    private void UpdateParticleProperties()
    {
        if (particleStove != null)
        {
            if (stoveSlider.value == 0)
            {
                particleEmission.enabled = false;
            }
            else
            {
                particleEmission.enabled = true;
                particleMain.startSpeed = Mathf.Lerp(1f, 5f, stoveSlider.value / stoveSlider.maxValue);

                particleMain.startLifetime = Mathf.Lerp(0.1f, 0.15f, stoveSlider.value / stoveSlider.maxValue);

                particleMain.startColor = Color.Lerp(Color.yellow, Color.red, stoveSlider.value / stoveSlider.maxValue);

                particleEmission.rateOverTime = Mathf.Lerp(10f, 50f, stoveSlider.value / stoveSlider.maxValue);
            }
        }
    }
    private void StoveSliderValue()
    {
        displayStoveTemp.text = stoveSlider.value.ToString("F0") + " °C";
    }
}