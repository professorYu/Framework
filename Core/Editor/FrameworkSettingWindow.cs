using UnityEditor;
using UnityEngine;

/// <summary>
/// 框架设置窗口  可视化操作窗口
/// </summary>
public class FrameworkSettingWindow : EditorWindow
{
    [MenuItem("Framework/Setting Window")]
    public static void Open()
    {
        EditorWindow editorWindow = GetWindow<FrameworkSettingWindow>();
        editorWindow.titleContent = new GUIContent("Framework Setting");
    }

    private DefaultAsset _folderGo;
    private DefaultAsset FolderGo
    {
        get
        {
            if (_folderGo == null)
            {
                _folderGo = (DefaultAsset)AssetDatabase.LoadAssetAtPath(FrameworkSettingManager.Instance.UIScriptGenFolderPath, typeof(DefaultAsset));
            }
            return _folderGo;
        }
        set
        {
            if (value != _folderGo)
            {
                _folderGo = value;
                FrameworkSettingManager.Instance.UIScriptGenFolderPath = AssetDatabase.GetAssetPath(_folderGo);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("UI代码生成文件夹 : " + FrameworkSettingManager.Instance.UIScriptGenFolderPath);
        FolderGo = (DefaultAsset)EditorGUILayout.ObjectField(FolderGo, typeof(DefaultAsset), false, GUILayout.Width(200));
    }

}
