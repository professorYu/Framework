using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 框架设置窗口
/// </summary>
public class FramewrokSettingWindow : EditorWindow
{
    [MenuItem("Framework/Setting Window")]
    public static void Open()
    {
        EditorWindow editorWindow = GetWindow<FramewrokSettingWindow>();
        editorWindow.titleContent = new GUIContent("Framework Setting");
    }

    private DefaultAsset _folderGo;
    private DefaultAsset FolderGo
    {
        get
        {
            if (_folderGo == null)
            {
                _folderGo = (DefaultAsset)AssetDatabase.LoadAssetAtPath(FramewrokSettingConfig.UIScriptGenFolderPath, typeof(DefaultAsset));
            }
            return _folderGo;
        }
        set
        {
            if (value != _folderGo)
            {
                _folderGo = value;
                FramewrokSettingConfig.UIScriptGenFolderPath = AssetDatabase.GetAssetPath(_folderGo);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("UI代码生成文件夹 : " + FramewrokSettingConfig.UIScriptGenFolderPath);
        FolderGo = (DefaultAsset)EditorGUILayout.ObjectField(FolderGo, typeof(DefaultAsset), false, GUILayout.Width(200));
    }

}
