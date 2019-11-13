using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashObject : MonoBehaviour
{
    [SerializeField] private GameObject _destroyedObject;
    private void OnMouseDown()
    {
        Instantiate(_destroyedObject, transform.position, transform.rotation);
        Destroy(gameObject);
       
    }
    
}
