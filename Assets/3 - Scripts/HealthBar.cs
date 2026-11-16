using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject barVisual;
    [SerializeField] private Slider slider;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        barVisual.SetActive(false);
    }

    public void Initialize(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        barVisual.SetActive(false);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        barVisual.SetActive(true);
    }

    public void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
