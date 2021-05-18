﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using System.Collections;
#if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public static class CosmosEditorUtility
    {
        public const string CosmosFramework = "CosmosFramework";
        public static string LibraryPath
        {
            get
            {
                if (string.IsNullOrEmpty(libraryPath))
                {
                    var editorPath = new DirectoryInfo(Application.dataPath);
                    var rootPath = editorPath.Parent.FullName + "/Library/";
                    libraryPath = Utility.IO.Combine(rootPath, CosmosFramework);
                }
                return libraryPath;
            }
        }
        static string libraryPath;
        static readonly Vector2 cosmosDevWinSize = new Vector2(512f, 384f);
        static readonly Vector2 cosmosMaxWinSize = new Vector2(768f, 768f);
        public static Vector2 CosmosDevWinSize { get { return cosmosDevWinSize; } }
        public static Vector2 CosmosMaxWinSize { get { return cosmosMaxWinSize; } }
        /// <summary>
        /// 刷新unity编辑器；
        /// </summary>
        public static void RefreshEditor()
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        /// <summary>
        /// 写入方式为覆写；
        /// 对象数据会被存储为json；
        /// </summary>
        public static void SaveData<T>(string fileName, T editorData)
            where T : class, new()
        {
            var json = JsonUtility.ToJson(editorData, true);
            SaveDataJson(fileName, json);
        }
        public static void SaveDataJson(string fileName, string context)
        {
            Utility.IO.OverwriteTextFile(LibraryPath, fileName, context);
        }
        public static T GetData<T>(string fileName)
            where T : class, new()
        {
            var json = GetDataJson(fileName);
            var obj = JsonUtility.FromJson<T>(json);
            return obj;
        }
        public static string GetDataJson(string fileName)
        {
            var filePath = Utility.IO.CombineRelativeFilePath(fileName, LibraryPath);
            var cfgStr = Utility.IO.ReadTextFileContent(filePath);
            return cfgStr.ToString();
        }
        public static void ClearData(string fileName)
        {
            var filePath = Utility.IO.CombineRelativeFilePath(fileName, LibraryPath);
            Utility.IO.DeleteFile(filePath);
        }
        public static string GetDefaultLogOutputDirectory()
        {
            DirectoryInfo info = new DirectoryInfo(Application.dataPath);
            string path = info.Parent.FullName;
            return path;
        }
        public static void DrawVerticalContext(Action context)
        {
            GUILayout.BeginVertical();
            context?.Invoke();
            GUILayout.EndVertical();
        }
        public static void DrawVerticalContext(Action context, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            context?.Invoke();
            GUILayout.EndVertical();
        }
        public static void DrawHorizontalContext(Action context, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            context?.Invoke();
            GUILayout.EndHorizontal();
        }
        public static void DrawHorizontalContext(Action context)
        {
            GUILayout.BeginHorizontal();
            context?.Invoke();
            GUILayout.EndHorizontal();
        }
        #region Coroutine
        public static EditorCoroutine StartCoroutine(IEnumerator coroutine)
        {
            return EditorCoroutineCore.StartCoroutine(coroutine);
        }
        public static void StopCoroutine(EditorCoroutine coroutine)
        {
            EditorCoroutineCore.StopCoroutine(coroutine);
        }
        public static void StopCoroutine(IEnumerator coroutine)
        {
            EditorCoroutineCore.StopCoroutine(coroutine);
        }
        #endregion
        /// <summary>
        /// 定位目标路径在Assets下的位置；
        /// </summary>
        /// <param name="path">目标路径</param>
        public static void PingAndActiveObject(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        public static void LogInfo(object msg, object context = null)
        {
            if (context == null)
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"} </color></b>{msg}");
            else
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public static void LogWarning(object msg, object context = null)
        {
            if (context == null)
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}");
            else
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}", context as Object);
        }
        public static void LogError(object msg, object context = null)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"} </color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"}</color></b>{msg}", context as Object);
        }
        public static void LogFatal(object msg, object context = null)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}", context as Object);
        }
    }
}
#endif
