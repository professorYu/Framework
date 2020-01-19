
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

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


    public static void SetSprite(this Image image, string spriteName)
    {
        ResManager.Instance.Load<Sprite>(spriteName, sprite =>
        {
            if (image)
            {
                image.sprite = sprite;
            }
        });
    }

    public static void SetMaterial(this Renderer renderer, string materialName)
    {
        ResManager.Instance.Load<Material>(materialName, material =>
        {
            if (renderer)
            {
                renderer.material = material;
            }
        });

    }
}