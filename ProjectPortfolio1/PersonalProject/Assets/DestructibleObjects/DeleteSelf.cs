using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RemoveShatteredObject());
    }



    IEnumerator RemoveShatteredObject()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
