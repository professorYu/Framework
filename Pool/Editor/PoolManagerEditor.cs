using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
    private PoolManager _target;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _target = (PoolManager)this.target;

        //PropertyInfo propertyInfos = _target.GetType().GetProperty("_pools", BindingFlags.NonPublic | BindingFlags.Instance);
        ////foreach (var propertyInfo in propertyInfos)
        ////{
            
        ////}
        //Debug.LogError(propertyInfos);
        //Debug.LogError("a");
        ////propertyInfos.


        foreach (var targetPool in _target.Pools)
        {
            EditorGUILayout.LabelField(targetPool.Key.ToString() + " : ");

            foreach (Object obj in targetPool.Value.ObjStack)
            {
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
            }
        }

    }

}
