using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class UIControler : MonoBehaviour {
    [SerializeField] private bool vrUI;
    [SerializeField] private GameObject targetComponent; 

    void Start() {
        UpdateComponentState();
    }

    private void UpdateComponentState() {
        bool isVR = isPresent();

        if (vrUI && isVR) {
            targetComponent.SetActive(true);
        }
        else if (vrUI && !isVR) {
            targetComponent.SetActive(false);
        }
        else if (!vrUI && isVR) {
            targetComponent.SetActive(false);
        }
        else if (!vrUI && !isVR) {
            targetComponent.SetActive(true);
        }
    }

    public static bool isPresent() {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems) {
            if (xrDisplay.running) {
                Debug.Log("has vr");
                return true;
            }
        }
        Debug.Log("Does not have vr");
        return false;
    }
}
