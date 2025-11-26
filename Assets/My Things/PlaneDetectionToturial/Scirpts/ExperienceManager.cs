using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ExperienceManager : MonoBehaviour
{
    [SerializeField] private Button addHumenModel;
    [SerializeField] private ARRaycastManager ARRaycastManager;
    [SerializeField] private GameObject humanModelPrefab;


    private bool _canAddHumanModel;
    private GameObject _humanModelPreview;
    private Vector3 detectedPosition = new Vector3();
    private Quaternion detectedRotation = Quaternion.identity;
    private ARTrackable _currentTrackable = null;

    private void Start()
    {
        InputHandler.OnTap += SpawnHumanModel;
        _humanModelPreview = Instantiate(humanModelPrefab);
        SetCanAddHumanModel(true);
    }


    private void SpawnHumanModel()
    {
        if (!_canAddHumanModel) return; 
       
        var humanModel = Instantiate(humanModelPrefab);
        humanModel.GetComponent<HumanModel>().PlaceHumanModel(_currentTrackable);
        humanModel.transform.position = detectedPosition;
        humanModel.transform.rotation = detectedRotation;

        SetCanAddHumanModel(false);
    }

    private void Update()
    {
        GetRayCastHitTransform();
    }

    private void GetRayCastHitTransform()
    {
        var hits = new List<ARRaycastHit>();
        var middleScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        if (ARRaycastManager.Raycast(middleScreen, hits, TrackableType.PlaneWithinPolygon))
        {
            detectedPosition = hits[0].pose.position;
            detectedRotation = hits[0].pose.rotation;
            _humanModelPreview.transform.position = detectedPosition;
            _humanModelPreview.transform.rotation = detectedRotation;
            _currentTrackable = hits[0].trackable;
        }
    }

    private void OnDestroy()
    {
        InputHandler.OnTap -= SpawnHumanModel;
    }

    public void SetCanAddHumanModel(bool canAddHumanModel)
    {
        _canAddHumanModel = canAddHumanModel;
        addHumenModel.gameObject.SetActive(!_canAddHumanModel);
        _humanModelPreview.gameObject.SetActive(canAddHumanModel);
    }
}
