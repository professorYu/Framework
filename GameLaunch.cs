using UnityEngine;

/// <summary>
/// 初始化框架
/// </summary>
public class GameLaunch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitGame()
    {
        //Application.targetFrameRate = 60;
        //AudioManager.Instance.Startup();
        //UIManager.Instance.Startup();
    }
}
