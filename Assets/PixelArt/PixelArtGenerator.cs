using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelArtGenerator : MonoBehaviour
{
    public GameObject pixelPrefab;
    public GameObject[] pixels;
    public Vector2Int resolution;
    public float scale;
    public Texture2D sprite;

    void Start()
    {
        DeletePixels();
        GeneratePixels();
        UpdatePixels();
    }

    void GeneratePixels()
    {
        pixels = new GameObject[resolution.x * resolution.y];

        var i = 0;
        for (var x = 0; x < resolution.x; x++)
        {
            for (var y = 0; y < resolution.y; y++)
            {
                pixels[i] = Instantiate(pixelPrefab);
                pixels[i].transform.parent = transform;
                pixels[i].transform.localPosition = new Vector3((float)x / (float)resolution.x * scale, (float)y / (float)resolution.x * scale, 0);
                pixels[i].transform.localScale = new Vector3(scale / resolution.x, scale / resolution.x, scale / resolution.x);
                i++;
            }
        }
    }
    void UpdatePixels()
    {
        var i = 0;
        for (var x = 0; x < sprite.width; x++)
        {
            for (var y = 0; y < sprite.height; y++)
            {
                Color color = sprite.GetPixel(x, y);

                if (color.a == 0 && i < pixels.Length)
                    pixels[i].SetActive(false);

                Material material = pixels[i].GetComponent<MeshRenderer>().material;
                material.SetColor("_Color", color);

                i++;
            }
        }
    }

    void DeletePixels()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }   
    }

    void Update()
    {
        
    }
}
