using System;

public abstract class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Activator.CreateInstance<T>();
                (_instance as Singleton<T>)?.Init();
            }

            return _instance;
        }
    }

    public static void Release()
    {
        if (_instance != null)
        {
            _instance = (T)null;
        }
    }

    public virtual void Init()
    {

    }

}
