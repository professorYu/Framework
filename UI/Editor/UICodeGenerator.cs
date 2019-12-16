
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor.Callbacks;
using static System.String;
using Object = UnityEngine.Object;

public class UICodeGenerator
{
    private static string UIMainTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIMainTemplate.txt";
    private static string UIReferenceTemplatePath = Application.dataPath + "/Framework/UI/ConfigData/UIReferenceTemplate.txt";
    private static string UIScriptSaveFolider = Application.dataPath + "/";

    private static string GenerateUIKey = "GenerateUIKey";

    [MenuItem("Assets/生成UI代码")]
    public static void CreateUICode()
    {
        var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
        EditorUtility.DisplayProgressBar("", "生成UI代码中...", 0);

        //所有UI的路径合集  编译完要添加组件
        string allPathStr = "";
        for (var i = 0; i < objs.Length; i++)
        {
            GameObject obj = objs[i] as GameObject;
            if (obj == null) continue;

            string uiPrefabPath = AssetDatabase.GetAssetPath(obj);
            allPathStr += uiPrefabPath + (i == objs.Length - 1 ? "" : "|");

            string componentName = obj.name;
            WriteReferenceCode(obj, componentName);
            WriteMainCode(componentName);

            EditorUtility.DisplayProgressBar("", "生成UI代码中...", (float)(i + 1) / objs.Length);
        }

        Debug.LogError(allPathStr);
        PlayerPrefs.SetString(GenerateUIKey, allPathStr);

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
        AddGenComponent();

    }

    //写引用文件
    private static void WriteReferenceCode(GameObject clone, string componentName)
    {
        UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
        string referenceStr = "";

        for (int i = 0; i < marks.Length; i++)
        {
            referenceStr += "public GameObject " + marks[i].name + ";\n\t";
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
        AddGenComponent();
    }

    //添加生成的UI 组件
    static void AddGenComponent()
    {
        string allPathStr = PlayerPrefs.GetString(GenerateUIKey);
        if (IsNullOrEmpty(allPathStr)) return;

        string[] prefabPathArr = allPathStr.Split('|');

        foreach (var prefabPath in prefabPathArr)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null) continue;

            var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            string componentName = obj.name;
            Component component = clone.GetComponent(componentName);

            if (component == null)
            {
                Assembly assembly = AppDomain.CurrentDomain.Load("Assembly-CSharp");
                Type type = assembly.GetType(componentName);
                if (type != null)
                {
                    component = clone.AddComponent(type);
                }
            }

            if (component != null)
            {
                FieldInfo[] fileFieldInfos = component.GetType().GetFields();
                UIMark[] marks = clone.transform.GetComponentsInChildren<UIMark>();
                foreach (FieldInfo fileFieldInfo in fileFieldInfos)
                {
                    for (int i = 0; i < marks.Length; i++)
                    {
                        if (marks[i].name == fileFieldInfo.Name)
                        {
                            fileFieldInfo.SetValue(component, marks[i].gameObject);
                            break;
                        }
                    }
                }
                PrefabUtility.SaveAsPrefabAssetAndConnect(clone, prefabPath, InteractionMode.AutomatedAction);
                PlayerPrefs.DeleteKey(GenerateUIKey);
            }


            Object.DestroyImmediate(clone);
        }

    }
}