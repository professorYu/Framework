using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 编辑器上标记自动生成UI
/// </summary>
///
public class UIMark : MonoBehaviour
{

    public string ComponentName
    {
        get
        {
#if UNITY_EDITOR

            //if (GetComponent("SkeletonAnimation")) return "SkeletonAnimation";
            if (GetComponent<ScrollRect>()) return "ScrollRect";
            if (GetComponent<InputField>()) return "InputField";

            // text mesh pro supported
            if (GetComponent("TMP.TextMeshProUGUI")) return "TMP.TextMeshProUGUI";
            if (GetComponent("TMPro.TextMeshProUGUI")) return "TMPro.TextMeshProUGUI";
            if (GetComponent("TMPro.TextMeshPro")) return "TMPro.TextMeshPro";
            if (GetComponent("TMPro.TMP_InputField")) return "TMPro.TMP_InputField";

            // UGUI  
            if (GetComponent<Button>()) return "Button";
            if (GetComponent<Text>()) return "Text";
            if (GetComponent<RawImage>()) return "RawImage";
            if (GetComponent<Toggle>()) return "Toggle";
            if (GetComponent<Slider>()) return "Slider";
            if (GetComponent<Scrollbar>()) return "Scrollbar";
            if (GetComponent<Image>()) return "Image";
            if (GetComponent<ToggleGroup>()) return "ToggleGroup";

            // other
            if (GetComponent<Rigidbody>()) return "Rigidbody";
            if (GetComponent<Rigidbody2D>()) return "Rigidbody2D";

            if (GetComponent<BoxCollider2D>()) return "BoxCollider2D";
            if (GetComponent<BoxCollider>()) return "BoxCollider";
            if (GetComponent<CircleCollider2D>()) return "CircleCollider2D";
            if (GetComponent<SphereCollider>()) return "SphereCollider";
            if (GetComponent<MeshCollider>()) return "MeshCollider";

            if (GetComponent<Collider>()) return "Collider";
            if (GetComponent<Collider2D>()) return "Collider2D";

            if (GetComponent<Animator>()) return "Animator";
            if (GetComponent<Canvas>()) return "Canvas";
            if (GetComponent<Camera>()) return "Camera";
            if (GetComponent("EmptyGraphic")) return "EmptyGraphic";
            if (GetComponent<RectTransform>()) return "RectTransform";
            if (GetComponent<MeshRenderer>()) return "MeshRenderer";

            if (GetComponent<SpriteRenderer>()) return "SpriteRenderer";
#endif

            return "Transform";
        }
    }
}
