using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class WaterMesh : MonoBehaviour
{
    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;
    private Vector2[] uvs;

    public Vector2Int quads;
    public float size;

    [ExecuteInEditMode]
    void OnDrawGizmos()
    {
        // TODO improve this gizmos (they dont take in account quads)
        Vector3 topLeftCorner = transform.position + new Vector3(-size / 2, 0, -size / 2);
        Vector3 topRightCorner = transform.position + new Vector3(size / 2, 0, -size / 2);
        Vector3 bottomLeftCorner = transform.position + new Vector3(-size / 2, 0, size / 2);
        Vector3 bottomRightCorner = transform.position + new Vector3(size / 2, 0, size / 2);

        Gizmos.DrawLine(topLeftCorner, topRightCorner);
        Gizmos.DrawLine(topRightCorner, bottomRightCorner);
        Gizmos.DrawLine(bottomRightCorner, bottomLeftCorner);
        Gizmos.DrawLine(bottomLeftCorner, topLeftCorner);
    }

    // Start is called before the first frame update
    void Awake()
    {
        mesh = new Mesh();

        GenerateShape();
        UpdateMesh();
    }

    /// <summary>
    /// Return index of vertex by the given local position
    /// Might return index out of bounds!
    /// </summary>
    /// <param name="localPosition"></param>
    /// <returns></returns>
    public int GetVertexIndex(Vector3 localPosition)
    {
        float xPos = localPosition.x / size * quads.x + quads.x / 2;
        float zPos = localPosition.z / size * quads.y + quads.y / 2;
        return Mathf.RoundToInt(xPos) + Mathf.RoundToInt(zPos) * (quads.x + 1);
    }
    /// <summary>
    /// Returns neighbouring vertex given a local position and offset
    /// </summary>
    /// <param name="localPosition"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetZ"></param>
    /// <returns></returns>
    public int GetVertexNeighbours(Vector3 localPosition, int offsetX, int offsetZ)
    {
        return GetVertexIndex(localPosition) + offsetX + offsetZ * (quads.x + 1);
    }

    public void UpdateMesh()
    {
        mesh.MarkDynamic();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void GenerateShape()
    {
        vertices = new Vector3[(quads.x + 1) * (quads.y + 1)];
        uvs = new Vector2[(quads.x + 1) * (quads.y + 1)];

        int vertIndex = 0;
        for (float y = 0; y < (quads.y + 1); ++y)
        {
            for (float x = 0; x < (quads.x + 1); ++x)
            {
                uvs[vertIndex] = new Vector2(x / quads.x, y / quads.y);
                vertices[vertIndex++] = new Vector3(x / quads.x * size - size / 2, 0, y / quads.x * size - size / 2);
            }
        }

        triangles = new int[quads.x * quads.y * 6];

        int indicesIndex = 0;
        for (int y = 0; y < quads.y; ++y)
        {
            for (int x = 0; x < quads.x; ++x)
            {
                int start = y * (quads.x + 1) + x;
                triangles[indicesIndex++] = start + 1 + quads.x;
                triangles[indicesIndex++] = start + 1;
                triangles[indicesIndex++] = start;
                triangles[indicesIndex++] = start + 1 + quads.x;
                triangles[indicesIndex++] = start + 2 + quads.x;
                triangles[indicesIndex++] = start + 1;
            }
        }
    }
}
