using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 框架设置配置
/// </summary>
public static class FramewrokSettingConfig
{
    //UI代码生成文件夹  通过 FrameworkSetting窗口设置  如果文件夹不存在  则使用Asset文件夹作为目录
    public static string UIScriptGenFolderPath
    {
        get
        {
            string path = PlayerPrefs.GetString("FramewrokSetting_UIScriptGenFolderPath");
            if (!Directory.Exists(path))
            {
                PlayerPrefs.SetString("FramewrokSetting_UIScriptGenFolderPath", "Assets");
                path = "Assets";
            }

            return path;
        }
        set { PlayerPrefs.SetString("FramewrokSetting_UIScriptGenFolderPath", value); }
    }

}
