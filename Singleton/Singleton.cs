using System;
using System.Reflection;

public abstract class Singleton<T> : ISingleton where T : Singleton<T>
{
    private static T _instance;

    private static readonly object _lock = new object();

    protected Singleton()
    {
    }

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 获取私有构造函数
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                    // 获取无参构造函数
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if (ctor == null)
                    {
                        throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
                    }

                    // 通过构造函数，常见实例
                    _instance = ctor.Invoke(null) as T;
                    _instance.OnSingletonInit();

                }
            }

            return _instance;
        }
    }

    public virtual void Dispose()
    {
        _instance = null;
    }

    public virtual void OnSingletonInit()
    {
    }
}
