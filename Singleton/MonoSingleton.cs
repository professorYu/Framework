
using System.Reflection;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateMonoSingleton();
            }

            return _instance;
        }
    }

    public void Startup()
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

    private static T CreateMonoSingleton()
    {
        T instance = null;

        //根据attribute  创建
        MemberInfo info = typeof(T);
        var attributes = info.GetCustomAttributes(true);
        foreach (var atribute in attributes)
        {
            var defineAttri = atribute as MonoSingletonPath;
            if (defineAttri == null)
            {
                continue;
            }

            instance = CreateByPath(defineAttri.PathInHierarchy);
            break;
        }

        //如果没有attribute  直接用类字创建
        if (instance == null)
        {
            var obj = new GameObject(typeof(T).Name);
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<T>();
        }

        return instance;
    }

    //通过MonoSingletonPath的路径创建
    private static T CreateByPath(string path)
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
