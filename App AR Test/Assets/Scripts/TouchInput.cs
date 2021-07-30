using UnityEngine;
//using UnityEngine.XR.ARFoundation;

public class TouchInput : MonoBehaviour
{
    #region Fields

    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform verimaObj = null;
    [SerializeField] private float rotationSensitivity = 10f;
    [SerializeField] private float moveSensibility = 5f;
    [SerializeField] private float scaleMultiplier = 0.1f;
    [SerializeField] private float precision = 1;

    private float previewsTouchesDistance = 0f;

    #endregion

    #region UnityCallbacks

    void Update()
    {
        if (Input.touchCount > 0 && verimaObj != null)
            GetInput();
    }

    #endregion

    #region Methods

    private void GetInput()
    {
        Touch mainTouch = Input.GetTouch(0);
        Touch secondaryTouch = Input.GetTouch(Input.touches.Length - 1); // it will use the last recorded touch input 

        if (Input.touchCount == 1)
        {
            #region Rotation

            if (mainTouch.phase == TouchPhase.Moved)
            {
                Vector3 yAxisRotation = cam.up * -mainTouch.deltaPosition.x;
                Vector3 xAxisRotation = cam.right * mainTouch.deltaPosition.y;
                Vector3 rotationVector = yAxisRotation + xAxisRotation;
                float rotationSpeed = mainTouch.deltaPosition.magnitude * rotationSensitivity * Time.deltaTime;
                verimaObj.RotateAround(verimaObj.position, rotationVector, rotationSpeed);
            }

            #endregion
        }
        else
        {
            #region Scaling & Movement

            if (mainTouch.phase == TouchPhase.Moved && secondaryTouch.phase == TouchPhase.Moved)
            {
                // scale
                if (Vector3.Magnitude(mainTouch.deltaPosition - secondaryTouch.deltaPosition) >= precision)
                {
                    Debug.Log("scale");
                    float scaleFactor = Vector3.Magnitude(mainTouch.deltaPosition - secondaryTouch.deltaPosition);
                    float touchesDistance = Vector2.Distance(mainTouch.position, secondaryTouch.position);
                    float scaleDir = touchesDistance - previewsTouchesDistance;
                    float dirSign = scaleDir != 0 ? Mathf.Sign(scaleDir) : 0;
                    verimaObj.localScale += Vector3.one * scaleFactor * scaleMultiplier * dirSign;
                    previewsTouchesDistance = touchesDistance;
                }
                // moving
                else
                {
                    Debug.Log("moving");
                    Vector3 yAxisMovement = cam.up * mainTouch.deltaPosition.y;
                    Vector3 xAxisMovement = cam.right * mainTouch.deltaPosition.x;
                    Vector3 movementVector = yAxisMovement + xAxisMovement;
                    float movementSpeed = moveSensibility * Time.deltaTime;
                    verimaObj.Translate(movementVector * movementSpeed, Space.World);
                }
            }

            #endregion
        }
    }

    #endregion


}
