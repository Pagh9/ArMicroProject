using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class ScreenTapped : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private XRIDefaultInputActions input;
    private List<ARRaycastHit> rayHits = new List<ARRaycastHit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>(); 
        arPlaneManager = GetComponent<ARPlaneManager>();
        input = new XRIDefaultInputActions();

    }

    private void OnEnable()
    {
        input.TouchscreenGestures.Enable();
        input.TouchscreenGestures.TapStartPosition.performed += OnTapStartPosition;

    }
    private void OnDisable()
    {
        input.TouchscreenGestures.Disable();
        input.TouchscreenGestures.TapStartPosition.performed -= OnTapStartPosition;

    }

    private void OnTapStartPosition(InputAction.CallbackContext context)
    {
        if (arRaycastManager.Raycast(context.ReadValue<Vector2>(), rayHits, TrackableType.PlaneWithinPolygon))
        {
            Vector3 hitPosition = rayHits[0].pose.position;
            Quaternion hitRotation = rayHits[0].pose.rotation;

            Instantiate(objectPrefab, hitPosition, hitRotation);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
