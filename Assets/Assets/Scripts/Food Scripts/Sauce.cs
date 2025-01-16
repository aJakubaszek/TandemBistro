using System.Collections;
using UnityEngine;

public class Sauce : MonoBehaviour
{
    // Reference to the Particle System
    public ParticleSystem ketchupParticleSystem;

    // Timer interval for triggering the effect
    public float splashInterval = 1.0f;

    // Flag to control the particle effect (for future XR input use)
    private bool isTriggered = false;

    private void Start()
    {
        // Check if the Particle System is assigned
        if (ketchupParticleSystem == null)
        {
            Debug.LogError("Ketchup Particle System is not assigned!");
            enabled = false;
            return;
        }

        // Start the repeating splash effect
        InvokeRepeating(nameof(TriggerSplash), 0f, splashInterval);
    }

    private void Update()
    {
        // Placeholder for XR input to toggle isTriggered
        // Example:
        // isTriggered = XRInputManager.IsActivated("YourXRInputAction");
    }

    private void TriggerSplash()
    {
        if (isTriggered || Application.isEditor) // Ensure it's triggered every second for now
        {
            ketchupParticleSystem.Play();
        }
    }

    // Optional: Method to enable or disable the effect
    public void SetTriggered(bool value)
    {
        isTriggered = value;
    }
}
