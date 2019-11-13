using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods { 
  public static Vector2 ToXZ(this Vector3 v)
    {
        return new Vector3(v.x, v.z);
    }
}
