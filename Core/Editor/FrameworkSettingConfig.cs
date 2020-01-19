using UnityEngine;

/// <summary>
/// 框架设置配置文件  SO文件
/// </summary>
public class FrameworkSettingConfig : ScriptableObject
{
    //UI代码生成文件夹  通过 FrameworkSetting窗口设置  如果文件夹不存在  则使用Asset文件夹作为目录
    public string UIScriptGenFolderPath;

}
