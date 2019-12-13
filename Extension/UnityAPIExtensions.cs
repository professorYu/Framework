
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityAPIExtensions
{
    public static void SetLayer(this GameObject go, int layer, bool includeChild = true)
    {
        go.layer = layer;
        if (includeChild)
        {
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetLayer(layer, includeChild);
            }
        }
    }

    public static void SetLayer(this Transform trans, int layer, bool includeChild = true)
    {
        trans.gameObject.SetLayer(layer, includeChild);
    }

    public static void SetLayer(this GameObject go, string layerName, bool includeChild = true)
    {
        int layer = LayerMask.NameToLayer(layerName);
        go.SetLayer(layer, includeChild);
    }

    public static void SetLayer(this Transform trans, string layerName, bool includeChild = true)
    {
        trans.gameObject.SetLayer(layerName, includeChild);
    }
}