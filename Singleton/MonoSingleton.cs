
using System.Reflection;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateMonoSingleton<T>();
            }

            return _instance;
        }
    }

    public virtual void OnSingletonInit()
    {

    }

    public virtual void Dispose()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }

    #region Mono单例创建逻辑

    private static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
    {
        T instance = null;

        MemberInfo info = typeof(T);
        var attributes = info.GetCustomAttributes(true);
        foreach (var atribute in attributes)
        {
            var defineAttri = atribute as MonoSingletonPath;
            if (defineAttri == null)
            {
                continue;
            }

            instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy);
            break;
        }

        if (instance == null)
        {
            var obj = new GameObject(typeof(T).Name);
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<T>();
        }

        instance.OnSingletonInit();
        return instance;
    }

    private static T CreateComponentOnGameObject<T>(string path) where T : MonoBehaviour
    {
        string[] subPath = path.Split('/');

        GameObject go = null;

        for (int index = 0; index < subPath.Length; index++)
        {
            string goName = subPath[index];

            if (index == 0)
            {
                go = GameObject.Find(goName);
                if (go == null)
                {
                    go = new GameObject(goName);
                    DontDestroyOnLoad(go);
                }
            }
            else
            {
                Transform child = go.transform.Find(goName);
                if (child == null)
                {
                    child = new GameObject(goName).transform;
                    child.SetParent(go.transform);
                }

                go = child.gameObject;
            }
        }

        T component = null;
        if (go)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    #endregion
}
