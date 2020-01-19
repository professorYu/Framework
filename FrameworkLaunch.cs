using UnityEngine;

/// <summary>
/// 初始化框架
/// </summary>
public class FrameworkLaunch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitFramework()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        PoolManager.Instance.Startup();
        AudioManager.Instance.Startup();
        UIManager.Instance.Startup();
    }
}
