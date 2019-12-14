using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBasePanel : MonoBehaviour
{
    protected void CloseSelf()
    {
        UIManager.Instance.Close(name);
    }
}
