using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSwordKit
{
    public class KitPacker
    {
        [Serializable]
        public class FileIndex
        {
            public string fileName;
            public string relativeFilePath;
            public long fileBytesLength;
            public long filePosition;
        }
        [Serializable]
        public class FileIndexs
        {
            public string dir;
            public long fileBytesLength;
            public int fileCount;
            public List<FileIndex> fileIndexList;
        }

        static readonly string tag = "kkp@v1.0.0";

        public static void Pack(string inDir, string outFilepath, bool exportJsonFile = true, System.Action<string, float, bool, string> progress = null, bool overwrite = true)
        {
            if (progress != null) progress("", 0, false, null);

            if (System.IO.File.Exists(outFilepath) && !overwrite)
            {
                Debug.LogError(KitConst.KitName + ": 输出文件已存在！" + outFilepath);
                if (progress != null) progress("", 1, true, "文件已存在！" + outFilepath);
                return;
            }

            if (System.IO.File.Exists(outFilepath))
                System.IO.File.Delete(outFilepath);
            System.IO.FileStream outFileStream = System.IO.File.Create(outFilepath);

            var outfilename = System.IO.Path.GetFileName(outFilepath);
            var outdir = System.IO.Path.GetDirectoryName(outFilepath);
            if (!System.IO.Directory.Exists(outdir))
                System.IO.Directory.CreateDirectory(outdir);

            var fileIndexs = new FileIndexs();
            fileIndexs.fileIndexList = new List<FileIndex>();
            fileIndexs.dir = inDir;

            EachFile(inDir, (fileinfo) => {
                var fileIndex = new FileIndex();
                fileIndex.fileName = fileinfo.Name;
                fileIndex.relativeFilePath = fileinfo.FullName.Replace(new System.IO.DirectoryInfo(inDir).FullName, "");
                fileIndex.relativeFilePath = fileIndex.relativeFilePath.Replace("\\", "/");
                if (fileIndex.relativeFilePath.Length > 0 && fileIndex.relativeFilePath[0] == '/')
                    fileIndex.relativeFilePath = fileIndex.relativeFilePath.Substring(1);
                fileIndex.filePosition = outFileStream.Position;
                var infileBytes = System.IO.File.ReadAllBytes(fileinfo.FullName);
                fileIndex.fileBytesLength = infileBytes.LongLength;

                if (infileBytes.LongLength <= int.MaxValue)
                {
                    var count = (int)infileBytes.LongLength;
                    outFileStream.Write(infileBytes, 0, count);
                    fileIndexs.fileCount++;
                    fileIndexs.fileBytesLength += count;
                    fileIndexs.fileIndexList.Add(fileIndex);
                }
                else
                    Debug.Log(KitConst.KitName + ": 文件尺寸超过 " + int.MaxValue + " 字节数，不能被写入！" + fileinfo.FullName);
                if (progress != null) progress(fileIndex.fileName, UnityEngine.Random.Range(0.1f,0.9f), false, null);
            });

            var pos = outFileStream.Position;
            var posBytes = System.BitConverter.GetBytes(pos);
            var jsonString = JsonUtility.ToJson(fileIndexs, true);
            var jsonbytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            var count = jsonbytes.Length;
            var countBytes = System.BitConverter.GetBytes(count);
            outFileStream.Write(jsonbytes, 0, count);
            outFileStream.Write(posBytes,0, posBytes.Length);
            outFileStream.Write(countBytes, 0, countBytes.Length);
            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            outFileStream.Write(tagbytes, 0, tagbytes.Length);
            outFileStream.Close();

            if (exportJsonFile)
            {
                var fileIndexsJsonPath = System.IO.Path.Combine(outdir, outfilename + ".fileIndexs.json");
                if (System.IO.File.Exists(fileIndexsJsonPath))
                    System.IO.File.Delete(fileIndexsJsonPath);
                System.IO.File.WriteAllText(fileIndexsJsonPath, jsonString);
            }

            if (progress != null) progress("", 1, true, null);
        }

        public static void Unpack(string inPackageFilepath, string outDir, System.Action<string, float, bool> progress = null, bool overwrite = true)
        {
            if (progress != null) progress("", 0, false);
            var fs = System.IO.File.OpenRead(inPackageFilepath);

            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var tagCount = tagbytes.Length;
            var tagBytes = new byte[tagCount];
            fs.Seek(-tagCount, System.IO.SeekOrigin.End);
            fs.Read(tagBytes, 0, tagCount);
            var getTag = System.Text.Encoding.UTF8.GetString(tagBytes);

            fs.Seek(-12 - tagCount, System.IO.SeekOrigin.End);
            byte[] longbytes = new byte[8];
            fs.Read(longbytes, 0, 8);
            byte[] countbytes = new byte[4];
            fs.Read(countbytes, 0, 4);
            var pos = System.BitConverter.ToInt64(longbytes, 0);
            var jsoncount = System.BitConverter.ToInt32(countbytes, 0);
            fs.Seek(pos, System.IO.SeekOrigin.Begin);
            byte[] jsonbytes = new byte[jsoncount];
            fs.Read(jsonbytes, 0, jsoncount);
            var jsonstring = System.Text.Encoding.UTF8.GetString(jsonbytes);
            var fileIndexs = JsonUtility.FromJson<FileIndexs>(jsonstring);
            for(var i = 0; i < fileIndexs.fileIndexList.Count; i++)
            {
                var fileindex = fileIndexs.fileIndexList[i];
                var filepath = System.IO.Path.Combine(outDir, fileindex.relativeFilePath);
                if (System.IO.File.Exists(filepath) && !overwrite) continue;
                else if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
                var filedir = System.IO.Path.GetDirectoryName(filepath);
                var filename = System.IO.Path.GetFileName(filepath);
                if (!System.IO.Directory.Exists(filedir))
                    System.IO.Directory.CreateDirectory(filedir);
                int count = (int)fileindex.fileBytesLength;
                byte[] bytes = new byte[count];
                fs.Seek((int)fileindex.filePosition, System.IO.SeekOrigin.Begin);
                fs.Read(bytes, 0, count);
                System.IO.File.WriteAllBytes(filepath, bytes);
                if (progress != null) progress(filename, (i + 1) / (float)fileIndexs.fileIndexList.Count, false);
            }
            fs.Close();
            if (progress != null) progress("", 1, true);
        }

        public static void EachFile(string inDir, System.Action<System.IO.FileInfo> findFileAction)
        {
            if (!System.IO.Directory.Exists(inDir) || findFileAction == null) return;
            var dirinfo = new System.IO.DirectoryInfo(inDir);
            foreach (var fileinfo in dirinfo.GetFiles())
                findFileAction(fileinfo);
            foreach (var dir in dirinfo.GetDirectories())
                EachFile(dir.FullName, findFileAction);
        }
    }
}
