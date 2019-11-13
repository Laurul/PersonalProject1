using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] private Vector2Int imageDim;
    [SerializeField] private int _regionAmount;
    [SerializeField] bool _distanceDraw = false;
    [SerializeField] private float alpha = 1.0f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create((_distanceDraw? getDiagramByDistance() : getDiagram()), new Rect(0, 0, imageDim.x, imageDim.y), Vector2.one * 0.5f);
    }

    private Texture2D getDiagram() {
        Color[] _regions = new Color[_regionAmount];
        Vector2Int[] center = new Vector2Int[_regionAmount];
        for(int i = 0; i < _regionAmount; i++)
        {
            center[i] = new Vector2Int(Random.Range(0,imageDim.x), Random.Range(0, imageDim.x));
            _regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), alpha);
        }

        Color[] pixelColor = new Color[imageDim.x * imageDim.y];

        for(int j = 0; j < imageDim.x; j++)
        {
            for(int k = 0; k < imageDim.y; k++)
            {
                int index = j * imageDim.x + k;
                pixelColor[index] = _regions[getNearestCenterIndex(new Vector2Int(j, k), center)];
            }
        }
        return GetImageFromColorArray(pixelColor);
    }

    private Texture2D getDiagramByDistance()
    {
       
        Vector2Int[] center = new Vector2Int[_regionAmount];
        for (int i = 0; i < _regionAmount; i++)
        {
            center[i] = new Vector2Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.x));
            
        }

        Color[] pixelColor = new Color[imageDim.x * imageDim.y];
        float[] distance = new float[imageDim.x * imageDim.y];
        for (int j = 0; j < imageDim.x; j++)
        {
            for (int k = 0; k < imageDim.y; k++)
            {
                int index = j * imageDim.x + k;
                distance[index] = Vector2.Distance(new Vector2Int(j, k), center[getNearestCenterIndex(new Vector2Int(j, k), center)]);
            }
        }
        float _maxDist = GetMaxDistance(distance);
        for(int i = 0; i < distance.Length; i++)
        {
            float colorValue = distance[i]/_maxDist;
            pixelColor[i] = new Color(colorValue,colorValue,colorValue,1f);
        }
        return GetImageFromColorArray(pixelColor);
    }

    float GetMaxDistance(float[] distances)
    {
        float maxDist = float.MinValue;
        for(int i = 0; i < distances.Length; i++)
        {
            if (distances[i] > maxDist)
            {
                maxDist = distances[i];
            }
            
        }
        return maxDist;
    }

    int getNearestCenterIndex(Vector2Int pixelPos, Vector2Int[] ceneter)
    {
        float smallestDst = float.MaxValue;
        int index = 0;
        for(int i = 0; i < ceneter.Length; i++)
        {
            if (Vector2.Distance(pixelPos, ceneter[i]) < smallestDst)
            {
                smallestDst = Vector2.Distance(pixelPos, ceneter[i]);
                index = i;
            }
        }
        return index;
    }

    private Texture2D GetImageFromColorArray(Color[] pixelColor)
    {
        Texture2D _texture = new Texture2D(imageDim.x, imageDim.y);
        _texture.filterMode = FilterMode.Point;
        _texture.SetPixels(pixelColor);
        _texture.Apply();
        return _texture;
    }
}
