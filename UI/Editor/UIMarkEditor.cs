using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMark))]
public class UIMarkEditor : Editor
{
    private UIMark _target;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _target = (UIMark)target;

        GUILayout.Label("类型 : " + _target.ComponentName);
    }
}
