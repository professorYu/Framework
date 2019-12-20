using UnityEngine;

/// <summary>
/// 初始化框架
/// </summary>
public class FrameworkLaunch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitFramework()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.Startup();
        UIManager.Instance.Startup();
    }
}
