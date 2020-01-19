using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 框架设置单例类   用来读取设置数据 [对外]
/// </summary>
public class FrameworkSettingManager :Singleton<FrameworkSettingManager>
{
    private FrameworkSettingConfig _frameworkSetting;

    public override void Init()
    {
        base.Init();
        string path = "Assets/Framework/Core/Config/FrameworkSettingConfig.asset";
        _frameworkSetting = AssetDatabase.LoadAssetAtPath<FrameworkSettingConfig>(path);

        if (_frameworkSetting == null)
        {
            _frameworkSetting = (FrameworkSettingConfig)ScriptableObject.CreateInstance("FrameworkSettingConfig");
            AssetDatabase.CreateAsset(_frameworkSetting, path);

        }

    }

    public string UIScriptGenFolderPath 
    { 
         get
         {
             if (string.IsNullOrEmpty(_frameworkSetting.UIScriptGenFolderPath))
             {
                 _frameworkSetting.UIScriptGenFolderPath = "Assets";
             }
            
             if (!Directory.Exists(_frameworkSetting.UIScriptGenFolderPath))
             { 
                Directory.CreateDirectory(_frameworkSetting.UIScriptGenFolderPath);
                AssetDatabase.Refresh();
             }

             return _frameworkSetting.UIScriptGenFolderPath;
         }

         set
         {
             _frameworkSetting.UIScriptGenFolderPath = value;
             AssetDatabase.SaveAssets();
         }
    }
}
