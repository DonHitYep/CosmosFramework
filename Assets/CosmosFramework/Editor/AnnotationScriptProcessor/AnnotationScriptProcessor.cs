using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using Cosmos;
namespace Cosmos.CosmosEditor
{
    public class AnnotationScriptProcessor : UnityEditor.AssetModificationProcessor
    {
        static string annotationStr =
@"//====================================
//* Author :
//* CreateTime :#CreateTime#
//* Version :
//* Description :
//==================================== " + "\n";
        /// <summary>
        /// 设置注释
        /// </summary>
        /// <param name="annotation"></param>
        public static void SetAnnotation(string annotation)
        {
            annotationStr = annotation;
        }
        public static void OnWillCreateAsset(string name)
        {
            if (!ApplicationConst.Editor.EnableScriptTemplateAnnotation)
                return;
            string path = name.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                annotationStr = annotationStr.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:ss"));
                annotationStr += File.ReadAllText(path);
                File.WriteAllText(path, annotationStr);
            }
            //GenericMenu genericMenu = new GenericMenu();
            //genericMenu.AddItem(new GUIContent("确定"), false, (o) =>
            //{
            //    if (path.EndsWith(".cs"))
            //    {
            //        annotationStr = annotationStr.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy/MM/dd HH:ss"));
            //        annotationStr += File.ReadAllText(path);
            //        File.WriteAllText(path, annotationStr);
            //    }
            //}, "Done");
            //genericMenu.AddSeparator("");
            //genericMenu.AddItem(new GUIContent("取消"), false, () => { return; });
            //genericMenu.ShowAsContext();
        }
    }
}