using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;

namespace KSwordKit.Editor.PackageManager
{
    [ScriptedImporter(1, "kkp")]
    public class KitImportKKP : ScriptedImporter
    {
        public const string kkpFilepathsTempFilename = "kkpFilepaths.tmp";
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var tempfilepath = System.IO.Path.Combine(Application.temporaryCachePath, System.IO.Path.GetTempFileName());
            System.IO.File.WriteAllBytes(tempfilepath, System.IO.File.ReadAllBytes(ctx.assetPath));
            var kkptempfilepath = System.IO.Path.Combine(Application.temporaryCachePath, kkpFilepathsTempFilename);
            if (!System.IO.File.Exists(kkptempfilepath))
                System.IO.File.WriteAllText(kkptempfilepath, tempfilepath, System.Text.Encoding.UTF8);
            else
            {
                var lines = System.IO.File.ReadAllLines(kkptempfilepath, System.Text.Encoding.UTF8);
                var allLines = new List<string>();
                allLines.AddRange(lines);
                allLines.Add(tempfilepath);
                System.IO.File.Delete(kkptempfilepath);
                System.IO.File.WriteAllLines(kkptempfilepath, allLines, System.Text.Encoding.UTF8);
            }
            KitImportKKPEditorWindow.Open("µ¼Èë°ü", ctx.assetPath);
        }
    }
}
