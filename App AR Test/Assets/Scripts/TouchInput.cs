using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TouchInput : MonoBehaviour
{
    #region Fields

    [SerializeField] private AnimationControl animationControl = null;
    [SerializeField] private ARRaycastManager arRaycastManager = null;
    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform parentTarget = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private float rotationSensitivity = 10f;
    [SerializeField] private float moveSensibility = 5f;
    [SerializeField] private float scaleMultiplier = 0.1f;
    [SerializeField] private float precision = 1;

    private float previewsTouchesDistance = 0f;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    #endregion

    #region UnityCallbacks

    private void Awake()
    {
        if (!animationControl)
            animationControl = GetComponent<AnimationControl>();
        if (!arRaycastManager)
            arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null ||
            Input.touchCount == 0 || target == null || cam == null)
            return;

        Touch mainTouch = Input.GetTouch(0);
        Touch secondaryTouch = Input.GetTouch(Input.touches.Length - 1); // it will use the last recorded touch input

        if (animationControl.IsAnimating)
            PauseAnimationOnTouch();

        if (Input.touchCount == 1)
            RotateObject(mainTouch);
        else
        {
            if (mainTouch.phase == TouchPhase.Moved && secondaryTouch.phase == TouchPhase.Moved)
            {
                if (Vector3.Magnitude(mainTouch.deltaPosition - secondaryTouch.deltaPosition) >= precision)
                    ScaleObject(mainTouch, secondaryTouch);
                else
                {
                    //MoveObjectOnFreeSpace(mainTouch, secondaryTouch);
                    MoveObjectOnPlane(mainTouch, secondaryTouch);
                }
            }
        }
    }

    #endregion

    #region Methods

    private void PauseAnimationOnTouch()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
            animationControl.StartRotation();
        else if (Input.GetTouch(0).phase == TouchPhase.Began)
            animationControl.StopRotation();
    }

    private void RotateObject(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved)
        {
            Vector3 yAxisRotation = cam.up * -touch.deltaPosition.x;
            Vector3 xAxisRotation = cam.right * touch.deltaPosition.y;
            Vector3 rotationVector = yAxisRotation + xAxisRotation;
            float rotationSpeed = touch.deltaPosition.magnitude * rotationSensitivity * Time.deltaTime;
            target.RotateAround(target.position, rotationVector, rotationSpeed);
        }
    }

    private void ScaleObject(Touch primaryTouch, Touch secondaryTouch)
    {
        float scaleFactor = Vector3.Magnitude(primaryTouch.deltaPosition - secondaryTouch.deltaPosition);
        float touchesDistance = Vector2.Distance(primaryTouch.position, secondaryTouch.position);
        float scaleDir = touchesDistance - previewsTouchesDistance;
        float dirSign = scaleDir != 0 ? Mathf.Sign(scaleDir) : 0;
        parentTarget.localScale += Vector3.one * scaleFactor * scaleMultiplier * dirSign;
        previewsTouchesDistance = touchesDistance;
    }

    private void MoveObjectOnFreeSpace(Touch primaryTouch, Touch secondaryTouch)
    {
        Vector3 yAxisMovement = cam.up * primaryTouch.deltaPosition.y;
        Vector3 xAxisMovement = cam.right * primaryTouch.deltaPosition.x;
        Vector3 movementVector = yAxisMovement + xAxisMovement;
        float movementSpeed = moveSensibility * Time.deltaTime;
        parentTarget.Translate(movementVector * movementSpeed, Space.World);
    }

    private void MoveObjectOnPlane(Touch primaryTouch, Touch secondaryTouch)
    {
        Vector2 primaryTouchPosition = primaryTouch.position;
        Vector2 secondaryTouchPosition = secondaryTouch.position;
        Pose primaryHitPose = new Pose(), secondaryHitPose = new Pose();
        
        if (arRaycastManager.Raycast(primaryTouchPosition, hits, TrackableType.PlaneWithinPolygon))
            primaryHitPose = hits[0].pose;

        if (arRaycastManager.Raycast(secondaryTouchPosition, hits, TrackableType.PlaneWithinPolygon))
            secondaryHitPose = hits[0].pose;

        Vector3 midPoint = (primaryHitPose.position + secondaryHitPose.position) / 2f;
        parentTarget.transform.position = midPoint;
    }

    #endregion


}
