
namespace KSwordKit.Core.ProjectFileManagement.Editor
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    public class AutomaticFileAnnotationEditor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 对特定文件后缀进行处理
        /// </summary>
        private static string[] extensions = new string[]{
            ".cs", ".js", ".shader",".compute"
        };

        private static string annotationStr = @"/*************************************************************************
 *  Copyright (C), #CopyrightYear#. All rights reserved.
 *
 *  FileName: #ScriptName#.cs
 *  Author: #Author#   
 *  Version: #Version#   
 *  CreateDate: #CreateDate#
 *  File Description: Ignore.
 *************************************************************************/
";

        static void OnWillCreateAsset(string metaPath)
        {
            var assetPath = metaPath.Replace(".meta", string.Empty);
            bool can = false;
            var ext = Path.GetExtension(assetPath);
            foreach(var e in extensions)
            {
                if(e == ext.ToLower())
                {
                    can = true;
                    break;
                }
            }
            if (can)
            {
                annotationStr = annotationStr.Replace("#CopyrightYear#", DateTime.Now.Year + "-" + (DateTime.Now.Year + 1));                
                annotationStr = annotationStr.Replace("#ScriptName#",Path.GetFileNameWithoutExtension(assetPath));
                annotationStr = annotationStr.Replace("#Author#", "ks");
                annotationStr = annotationStr.Replace("#Version#", "1.0.0");
                annotationStr = annotationStr.Replace("#CreateDate#", DateTime.Now.Year +"-"+DateTime.Now.Month+"-"+DateTime.Now.Day);
                
                annotationStr += File.ReadAllText(assetPath);
                File.WriteAllText(assetPath, annotationStr);
                AssetDatabase.Refresh();
            }
        }
    }
}

