using UnityEngine;

public class GenerateByImage : MonoBehaviour
{
    [SerializeField] private Texture2D _map;
    [SerializeField] private PixelColorToPrefab[] _pixelColorMapping;

    private int nr=0;
    // Start is called before the first frame update
    void Start()
    {
         GenerateCityLayout();
    }

    private void GenerateCityLayout()
    {
        for(int i = 0; i < _map.width; i++)
        {
            for(int j = 0; j < _map.height; j++)
            {
                PlaceObject(i, j);
            }
        }
        //Debug.Log(nr);
    }

    void PlaceObject(int x ,int y)
    {
        Color _imagePixelColor = _map.GetPixel(x, y);

        Vector2 pos = new Vector3(x, y);
        

        foreach(PixelColorToPrefab pixel in _pixelColorMapping)
        {
            if (pixel._prefabColor.Equals(_imagePixelColor))
            {
                //Vector3 pos = new Vector3(x,0,y);
                Instantiate(pixel._prefab, pos, Quaternion.identity,transform);
            }
        }
    }
}
