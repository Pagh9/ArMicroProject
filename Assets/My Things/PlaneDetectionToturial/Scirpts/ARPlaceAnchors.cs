using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class ARPlaceAnchors : MonoBehaviour
{
    [Header("Prefab to place")]
    [SerializeField] private GameObject objectPrefab;

    [Header("Raycast")]
    [SerializeField] private TrackableType raycastMask = TrackableType.PlaneWithinPolygon;

    private ARRaycastManager raycastManager;
    private ARAnchorManager anchorManager;

    private static readonly List<ARRaycastHit> hits = new();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];
        if (touch.phase != TouchPhase.Began)
            return;
        //New Raycast. Uses touch position to make the raycast instead of the middle. Could created a variable var like in the old experience manager script.
        if (!raycastManager.Raycast(touch.screenPosition, hits, raycastMask))
            return;

        PlaceAnchoredObject(hits[0]);
    }

    private void PlaceAnchoredObject(ARRaycastHit hit)
    {
        if (objectPrefab == null)
            return;

        // If we hit a plane, attach anchor to the plane (best)
        ARPlane plane = hit.trackable as ARPlane;
        ARAnchor anchor = null;

        if (plane != null)
        {
            //Attaches anchor to plane at the hit pose
            anchor = anchorManager.AttachAnchor(plane, hit.pose);
            if (anchor == null)
            {
                Debug.LogWarning("Failed to attach anchor to plane.");
                return;
            }
        }
        else
        {
            // Fallback: create a standalone anchor at pose
            var anchorGO = new GameObject("Anchor");
            anchorGO.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
            anchor = anchorGO.AddComponent<ARAnchor>();
        }

        // Spawn your model and parent it to the anchor (stable)
        GameObject placedObject = Instantiate(objectPrefab, hit.pose.position, hit.pose.rotation);
       
        placedObject.transform.SetParent(anchor.transform, worldPositionStays: true);
    }
}
