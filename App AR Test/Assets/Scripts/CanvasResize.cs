using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResize : MonoBehaviour
{
    #region Fields

    [SerializeField] Transform meshParent = null;
    [SerializeField] MeshFilter meshFilterTarget = null;
    [SerializeField] RectTransform canvasRect = null;

    #endregion

    #region UnityCallbacks

    private void Start()
    {
        Resize();
    }

    #endregion

    #region Methods

    public void Resize()
    {
        (Vector2 up, Vector2 right, Vector2 bottom, Vector2 left) vertices = FindExtremeVerticesWorldCoordinates();
        
        float width =  vertices.right.x - vertices.left.x;
        float height =  vertices.up.y - vertices.bottom.y;
        canvasRect.sizeDelta = new Vector2(width, height);
    }

    private (Vector2 up, Vector2 right, Vector2 bottom, Vector2 left) FindExtremeVerticesWorldCoordinates()
    {
        Mesh mesh = meshFilterTarget.mesh;
        Vector2[] extremeVertices = new Vector2[4]
        {
            new Vector2(0, -100000), // up
            new Vector2(-100000, 0), // right
            new Vector2(0, 100000), // down
            new Vector2(100000, 0)  // left
        };

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // check for highest vertex
            if (mesh.vertices[i].y > extremeVertices[0].y)
                extremeVertices[0] = mesh.vertices[i];
            // check for farthest right vertex
            if (mesh.vertices[i].x > extremeVertices[1].x)
                extremeVertices[1] = mesh.vertices[i];
            // check for lowset vertex
            if (mesh.vertices[i].y < extremeVertices[2].y)
                extremeVertices[2] = mesh.vertices[i];
            // check for farthest left vertex
            if (mesh.vertices[i].x < extremeVertices[3].x)
                extremeVertices[3] = mesh.vertices[i];
        }

        return (extremeVertices[0], extremeVertices[1], extremeVertices[2], extremeVertices[3]);
    }

    #endregion
}
