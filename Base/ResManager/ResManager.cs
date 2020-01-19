using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 资源管理类
/// </summary>
[MonoSingletonPath("[Framework]/ResManager")]
public class ResManager : MonoSingleton<ResManager>
{
    public AsyncOperationHandle Load<T>(string path, Action<T> loadComplete = null)
    {
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(path);
        if (handle.IsDone)
        {
            loadComplete?.Invoke((T)handle.Result);
        }
        else
        {
            handle.Completed += operationHandle =>
            {
                loadComplete?.Invoke((T)operationHandle.Result);
            };
        }

        return handle;
    }

    public AsyncOperationHandle Instantiate(string path, Action<GameObject> loadComplete = null)
    {
        AsyncOperationHandle handle = Addressables.InstantiateAsync(path);
        if (handle.IsDone)
        {
            loadComplete?.Invoke((GameObject)handle.Result);
        }
        else
        {
            handle.Completed += operationHandle =>
            {
                loadComplete?.Invoke((GameObject)operationHandle.Result);
            };
        }

        return handle;
    }

}
