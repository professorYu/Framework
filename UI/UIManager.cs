using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UILayer
{
    Bg,   //背景层     一般是底层常驻UI  例如 : 主界面的常驻UI
    Common, //通用层    普通UI层
    Pop,  //弹出层     常规弹窗  比一般UI高
    Tips,  //提示层    提示飘字  一般覆盖普通弹窗
    Top,  //最顶层    一般是重要必须显示在最上面的  例如 : 退出游戏弹窗
}

public class PanelData
{
    public UIBasePanel panel;
    public AsyncOperationHandle handle;
}

[MonoSingletonPath("[UI]/UIManager")]
public class UIManager : MonoSingleton<UIManager>
{
    //缓存UI父节点  分层
    private Dictionary<UILayer, RectTransform> _allPanelParentDict = new Dictionary<UILayer, RectTransform>();

    //缓存打开的UI 
    private Dictionary<string, PanelData> _allPanelData = new Dictionary<string, PanelData>();

    private void Awake()
    {
        //加载UI
        string uiRootPath = "UIRoot";
        GameObject root = Instantiate(Resources.Load<GameObject>(uiRootPath));
        root.name = uiRootPath;
        DontDestroyOnLoad(root);

        //创建分层UI
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            string strName = layer.ToString();
            GameObject child = new GameObject(strName);
            RectTransform rect = child.AddComponent<RectTransform>();
            rect.anchorMax = Vector2.one;
            rect.anchorMin = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            child.transform.SetParent(root.transform, false);

            if (!_allPanelParentDict.ContainsKey(layer))
            {
                _allPanelParentDict.Add(layer, rect);
            }
        }
    }

    public T GetPanel<T>() where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();
        _allPanelData.TryGetValue(panelName, out var panelData);
        return panelData == null ? null : panelData.panel as T;
    }

    public void Open<T>(UILayer layer = UILayer.Common, Action<T> callback = null) where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();

        if (_allPanelData.ContainsKey(panelName)) return;

        AsyncOperationHandle op = Addressables.InstantiateAsync(panelName);

        PanelData panelData = new PanelData();
        panelData.handle = op;
        _allPanelData.Add(panelName, panelData);

        op.Completed += delegate(AsyncOperationHandle handle)
        {

        };

        op.Completed += handle=>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject go = (GameObject)op.Result;
                go.name = panelName;
                go.transform.SetParent(_allPanelParentDict[layer], false);
                T uiPanel = go.GetComponent<T>();
                panelData.panel = uiPanel;
                callback?.Invoke(uiPanel);
            }
        };
    }

    public void Close<T>() where T : UIBasePanel
    {
        string panelName = typeof(T).ToString();
        Close(panelName);
    }

    public void Close(string panelName)
    {
        if (_allPanelData.TryGetValue(panelName, out var panelData))
        {
            Addressables.ReleaseInstance(panelData.handle);
            _allPanelData.Remove(panelName);
        }
    }

}
