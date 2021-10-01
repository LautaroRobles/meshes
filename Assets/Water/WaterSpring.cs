using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterMesh))]
public class WaterSpring : MonoBehaviour
{
    WaterMesh waterMesh;

    public float maxHeight = 2f;
    public float k = 0.01f;
    public float d = 0.01f;
    public float spread = 0.03f;
    public float speedEpsilon = 0.003f;

    private float[] velocities;
    private float[] forwardDeltas;
    private float[] rightDeltas;
    private float[] backDeltas;
    private float[] leftDeltas;

    public Camera cameraTest;
    public Vector3 testPosition;
    public float testSpeed;

    void Start()
    {
        //Time.timeScale = 0.01f;
        waterMesh = GetComponent<WaterMesh>();
        velocities = new float[waterMesh.vertices.Length];
        forwardDeltas = new float[waterMesh.vertices.Length];
        rightDeltas = new float[waterMesh.vertices.Length];
        backDeltas = new float[waterMesh.vertices.Length];
        leftDeltas = new float[waterMesh.vertices.Length];
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = cameraTest.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                WaterSpring myself = objectHit.GetComponent<WaterSpring>();

                if(myself != null)
                    myself.CreateWaterDrop(hit.transform.worldToLocalMatrix.MultiplyPoint(hit.point), testSpeed);
            }

        }
    }

    private void FixedUpdate()
    {
        UpdateWater();
    }

    public void CreateWaterDrop(Vector3 position, float speed)
    {
        Vector3[] vertices = waterMesh.vertices;

        int index = waterMesh.GetVertexIndex(position);

        if (index > 0 && index < waterMesh.vertices.Length)
        {
            velocities[index] += speed;
        }
    }

    void UpdateWater()
    {
        // Update physics of every vertex
        Vector3[] vertices = waterMesh.vertices;

        for (var index = 0; index < vertices.Length; index++)
        {
            vertices[index] = UpdateWaterVertex(vertices[index], index, k, d);
        }

        for (var index = 0; index < vertices.Length; index++)
        {
            int forwardNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 0, 1);
            int rightNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 1, 0);
            int backNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 0, -1);
            int leftNeighbour = waterMesh.GetVertexNeighbours(vertices[index], -1, 0);

            if (forwardNeighbour >= 0 && forwardNeighbour < vertices.Length)
            {
                forwardDeltas[index] = spread * (vertices[index].y - vertices[forwardNeighbour].y);
                velocities[forwardNeighbour] += forwardDeltas[index];
                vertices[forwardNeighbour].y += forwardDeltas[index];
            }
            if (rightNeighbour >= 0 && rightNeighbour < vertices.Length)
            {
                rightDeltas[index] = spread * (vertices[index].y - vertices[rightNeighbour].y);
                velocities[rightNeighbour] += rightDeltas[index];
                vertices[rightNeighbour].y += rightDeltas[index];
            }
            if (backNeighbour >= 0 && backNeighbour < vertices.Length)
            {
                backDeltas[index] = spread * (vertices[index].y - vertices[backNeighbour].y);
                velocities[backNeighbour] += backDeltas[index];
                vertices[backNeighbour].y += backDeltas[index];
            }
            if (leftNeighbour >= 0 && leftNeighbour < vertices.Length)
            {
                leftDeltas[index] = spread * (vertices[index].y - vertices[leftNeighbour].y);
                velocities[leftNeighbour] += leftDeltas[index];
                vertices[leftNeighbour].y += leftDeltas[index];
            }

        }

        /*
        for (var index = 0; index < vertices.Length; index++)
        {
            int forwardNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 0, 1);
            int rightNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 1, 0);
            int backNeighbour = waterMesh.GetVertexNeighbours(vertices[index], 0, -1);
            int leftNeighbour = waterMesh.GetVertexNeighbours(vertices[index], -1, 0);

            if (forwardNeighbour >= 0 && forwardNeighbour < vertices.Length)
            {
                vertices[forwardNeighbour].y += forwardDeltas[index];
            }
            if (rightNeighbour >= 0 && rightNeighbour < vertices.Length)
            {
                vertices[rightNeighbour].y += rightDeltas[index];
            }
            if (backNeighbour >= 0 && backNeighbour < vertices.Length)
            {
                vertices[backNeighbour].y += backDeltas[index];
            }
            if (leftNeighbour >= 0 && leftNeighbour < vertices.Length)
            {
                vertices[leftNeighbour].y += leftDeltas[index];
            }

        }
        */

        waterMesh.vertices = vertices;
        waterMesh.UpdateMesh();
    }

    Vector3 UpdateWaterVertex(Vector3 vertex, int index, float springConstant, float dampening)
    {
        float target_height = 0;
        float height = vertex.y;
        float x = height - target_height;
        float loss = -dampening * velocities[index];
        float force = -springConstant * x + loss;

        vertex.y += velocities[index];
        velocities[index] += force;

        if (Mathf.Abs(velocities[index]) < speedEpsilon)
        {
            velocities[index] = 0;
        }

        vertex.y = Mathf.Clamp(vertex.y, -maxHeight, maxHeight);
        velocities[index] = Mathf.Clamp(velocities[index], -maxHeight, maxHeight);

        return vertex;
    }
}
