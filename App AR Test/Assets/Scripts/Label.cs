using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : MonoBehaviour
{
    #region Fields

    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform anchor = null;
    [SerializeField] private Vector2 anchorOffset = Vector2.zero;

    #endregion

    #region UnityCallbacks

    private void Update()
    {
        FollowAnchor();
        transform.rotation = cam.rotation;
    }

    #endregion

    #region Methods

    private void FollowAnchor()
    {
        transform.position = anchor.position + (Vector3)anchorOffset;
    }

    #endregion
}
