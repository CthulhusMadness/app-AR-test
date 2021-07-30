using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
    #region Fields

    [SerializeField] private AnimationControl animationControl = null;
    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform parentTarget = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private float rotationSensitivity = 10f;
    [SerializeField] private float moveSensibility = 5f;
    [SerializeField] private float scaleMultiplier = 0.1f;
    [SerializeField] private float precision = 1;

    private float previewsTouchesDistance = 0f;

    #endregion

    #region UnityCallbacks

    private void Awake()
    {
        if (!animationControl)
            animationControl = GetComponent<AnimationControl>();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null &&
            Input.touchCount > 0 && target != null && cam != null)
            GetInput();
    }

    #endregion

    #region Methods

    private void GetInput()
    {
        Touch mainTouch = Input.GetTouch(0);
        Touch secondaryTouch = Input.GetTouch(Input.touches.Length - 1); // it will use the last recorded touch input 

        if (animationControl.IsAnimating)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
                animationControl.StartRotation();
            else if (Input.GetTouch(0).phase == TouchPhase.Began)
                animationControl.StopRotation();
        }

        if (Input.touchCount == 1)
        {
            #region Rotation
            if (mainTouch.phase == TouchPhase.Moved)
            {
                Vector3 yAxisRotation = cam.up * -mainTouch.deltaPosition.x;
                Vector3 xAxisRotation = cam.right * mainTouch.deltaPosition.y;
                Vector3 rotationVector = yAxisRotation + xAxisRotation;
                float rotationSpeed = mainTouch.deltaPosition.magnitude * rotationSensitivity * Time.deltaTime;
                target.RotateAround(target.position, rotationVector, rotationSpeed);
            }

            #endregion
        }
        else if (Input.touchCount > 1)
        {
            #region Scaling & Movement

            if (mainTouch.phase == TouchPhase.Moved && secondaryTouch.phase == TouchPhase.Moved)
            {
                // scale
                if (Vector3.Magnitude(mainTouch.deltaPosition - secondaryTouch.deltaPosition) >= precision)
                {
                    float scaleFactor = Vector3.Magnitude(mainTouch.deltaPosition - secondaryTouch.deltaPosition);
                    float touchesDistance = Vector2.Distance(mainTouch.position, secondaryTouch.position);
                    float scaleDir = touchesDistance - previewsTouchesDistance;
                    float dirSign = scaleDir != 0 ? Mathf.Sign(scaleDir) : 0;
                    parentTarget.localScale += Vector3.one * scaleFactor * scaleMultiplier * dirSign;
                    previewsTouchesDistance = touchesDistance;
                }
                // moving
                else
                {
                    Vector3 yAxisMovement = cam.up * mainTouch.deltaPosition.y;
                    Vector3 xAxisMovement = cam.right * mainTouch.deltaPosition.x;
                    Vector3 movementVector = yAxisMovement + xAxisMovement;
                    float movementSpeed = moveSensibility * Time.deltaTime;
                    parentTarget.Translate(movementVector * movementSpeed, Space.World);
                }
            }

            #endregion
        }
    } 

    #endregion


}
