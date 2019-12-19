using UnityEngine;

/// <summary>
/// 游戏启动脚本   初始化一切 
/// </summary>
public class GameLaunch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoadBeforeScene()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.Startup();
        UIManager.Instance.Startup();
    }
}
