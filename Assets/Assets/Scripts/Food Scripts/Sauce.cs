using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sauce : MonoBehaviour
{
    public ParticleSystem ketchupParticleSystem;

    public LayerMask dishLayer;

    private bool isDishTriggered = false;

    private XRBaseControllerInteractor controllerInteractor;

    private void Start()
    {
        if (ketchupParticleSystem == null)
        {
            Debug.LogError("Ketchup Particle System is not assigned!");
            enabled = false;
            return;
        }
    }
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        TriggerSplash();
    }

    private void TriggerSplash()
    {
        if (ketchupParticleSystem.isPlaying)
            return; // Prevent overlapping bursts

        ketchupParticleSystem.Play();

        isDishTriggered = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!isDishTriggered && other.TryGetComponent(out Dish dish))
        {
            // Call the DishFound function on the Dish object
            //dish.DishFound();

            // Ensure it triggers only once per burst
            isDishTriggered = true;
        }
    }
}
