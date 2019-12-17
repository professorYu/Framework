
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor.Callbacks;
using static System.String;
using Application = UnityEngine.Application;
using Object = UnityEngine.Object;
using RectTransform = UnityEngine.RectTransform;

public class UICodeGenerator
{
    private static string UIMainTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIMainTemplate.txt";
    private static string UIReferenceTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIReferenceTemplate.txt";
    private static string GenerateUIKey = "GenerateUIKey";

    //需要配置
    private static string UIScriptSaveFolider = Application.dataPath + "/";

    [MenuItem("Assets/生成UI代码 Ctrl+E刷新 %e", true, 2001)]
    public static bool IsShowGenUICode()
    {
        var uiPrefabArr = Selection.GetFiltered(typeof(RectTransform), SelectionMode.Assets);
        return uiPrefabArr.Length > 0;
    }

    [MenuItem("Assets/生成UI代码 Ctrl+E刷新 %e", false, 2001)]
    public static void CreateUICode()
    {
        Object[] uiPrefabArr = Selection.GetFiltered(typeof(RectTransform), SelectionMode.Assets);
        EditorUtility.DisplayProgressBar("", "生成UI代码中...", 0);

        //所有UI的路径合集  编译完要添加组件
        string allPathStr = "";
        for (var i = 0; i < uiPrefabArr.Length; i++)
        {
            RectTransform panelRect = uiPrefabArr[i] as RectTransform;
            string uiPrefabPath = AssetDatabase.GetAssetPath(panelRect);
            allPathStr += uiPrefabPath + (i == uiPrefabArr.Length - 1 ? "" : "|");

            string componentName = panelRect.name;
            WriteReferenceCode(panelRect, componentName);
            WriteMainCode(componentName);

            EditorUtility.DisplayProgressBar("", "生成UI代码中...", (float)(i + 1) / uiPrefabArr.Length);
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();


        if (EditorApplication.isCompiling)
        {
            //如果需要编译   保存文件名  等编译完就添加
            PlayerPrefs.SetString(GenerateUIKey, allPathStr);
        }
        else
        {
            //不需要编译   则 直接添加组件
            GenUIPanel(allPathStr);
        }
    }

    //写引用文件
    private static void WriteReferenceCode(RectTransform panelGo, string componentName)
    {
        UIMark[] marks = panelGo.transform.GetComponentsInChildren<UIMark>();
        string referenceStr = "";

        for (int i = 0; i < marks.Length; i++)
        {
            UIMark mark = marks[i];
            referenceStr += "public " + mark.ComponentName + " " + mark.name + ";" + (i == marks.Length - 1 ? "" : "\r\t");
        }

        string template = File.ReadAllText(UIReferenceTemplatePath);
        template = template.Replace("[ClassName]", componentName);
        template = template.Replace("[Reference]", referenceStr);

        File.WriteAllText(UIScriptSaveFolider + componentName + ".Reference.cs", template, Encoding.UTF8);
    }

    //写主文件
    private static void WriteMainCode(string componentName)
    {
        string mainCodePath = UIScriptSaveFolider + componentName + ".cs";
        if (!File.Exists(mainCodePath))
        {
            string template = File.ReadAllText(UIMainTemplatePath);
            template = template.Replace("[ClassName]", componentName);
            File.WriteAllText(UIScriptSaveFolider + componentName + ".cs", template, Encoding.UTF8);
        }
    }

    [DidReloadScripts]
    static void OnCompileEnd()
    {
        string allPathStr = PlayerPrefs.GetString(GenerateUIKey);
        GenUIPanel(allPathStr);
        PlayerPrefs.DeleteKey(GenerateUIKey);
    }

    //清理UI上的错误Panel组件   （通常是复制UI时候顺带复制过来的  先清理）
    static void DeleteErrorComponent(Component component)
    {
        if (component != null)
        {
            if (component.GetType().Name != component.name)
            {
                Object.DestroyImmediate(component);
            }
        }
    }

    //添加UI对应组件
    static void AddPanelComponent(ref Component component, GameObject clone)
    {
        //UI上没有Panel组件  添加上去
        if (component == null)
        {
            Assembly assembly = AppDomain.CurrentDomain.Load("Assembly-CSharp");
            Type type = assembly.GetType(clone.name);
            if (type != null)
            {
                component = clone.AddComponent(type);
            }
        }
    }

    //查找关联引用
    static void FindReference(ref Component component, GameObject clone, string prefabPath)
    {
        //处理引用关联
        if (component != null)
        {
            FieldInfo[] fileFieldInfos = component.GetType().GetFields();
            UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
            foreach (FieldInfo fileFieldInfo in fileFieldInfos)
            {
                for (int j = 0; j < marks.Length; j++)
                {
                    UIMark mark = marks[j];
                    if (mark.name == fileFieldInfo.Name)
                    {
                        fileFieldInfo.SetValue(component, mark.GetComponent(mark.ComponentName));
                        break;
                    }
                }
            }
        }
    }

    //添加生成的UI 组件
    static void GenUIPanel(string allPathStr)
    {
        if (IsNullOrEmpty(allPathStr)) return;

        string[] prefabPathArr = allPathStr.Split('|');

        for (int i = 0; i < prefabPathArr.Length; i++)
        {
            string prefabPath = prefabPathArr[i];
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null) continue;

            var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            Component component = clone.GetComponent<UIBasePanel>();

            //清理UI上的错误Panel组件   （通常是复制UI时候顺带复制过来的  先清理）
            DeleteErrorComponent(component);
            //UI上没有Panel组件  添加上去
            AddPanelComponent(ref component, clone);
            //关联引用
            FindReference(ref component, clone, prefabPath);

            PrefabUtility.SaveAsPrefabAssetAndConnect(clone, prefabPath, InteractionMode.AutomatedAction);
            Object.DestroyImmediate(clone);
        }
    }
}