using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR.Stacks
{
    public class ArPlacement : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToPlace;

        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        private GameObject spawnedObject;


        [SerializeField] private InputActionReference screenTouchPosition;
        [SerializeField] private InputActionReference screenTouchContact;
        private Vector2 touchPosition;
        private Camera mainCam;
        private bool isTouching = false;
        private void Awake()
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            mainCam = Camera.main;
            screenTouchPosition.asset.Enable();
            screenTouchContact.action.started += ctx => isTouching = true;
            screenTouchContact.action.canceled += ctx => isTouching = false;
        }
        private void OnDestroy()
        {
            screenTouchPosition.asset.Disable();
        }

        private void Update()
        {
            if (isTouching)
            {
                touchPosition = screenTouchPosition.action.ReadValue<Vector2>();
                Ray ray = mainCam.ScreenPointToRay(touchPosition);


                List<ARRaycastHit> hits = new List<ARRaycastHit>();


                if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (spawnedObject == null && screenTouchContact.action.triggered)
                    {
                        spawnedObject = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        spawnedObject.transform.position = hitPose.position;
                        spawnedObject.transform.rotation = hitPose.rotation;
                    }
                }
            }

        }
    }

}