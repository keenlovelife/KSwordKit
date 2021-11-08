using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSwordKit
{
    public class KitPacker
    {
        public const string FileFormat = "kkp";
        static readonly string tag = FileFormat + "@v1.0.0;make by ks";
        [Serializable]
        public class FileIndexs
        {
            public string dir;
            public long fileBytesLength;
            public string MD5Value;
            public int fileCount;
            public List<FileIndex> fileIndexList;
        }
        public enum FileIndexState
        {
            None,
            NewFile,
            CanUpdate,
            Same
        }
        [Serializable]
        public class FileIndex
        {
            public bool isDir;
            public string fileName;
            public string relativeFilePath;
            public string MD5Value;
            public long fileBytesLength;
            public long filePosition;
            [NonSerialized]
            public bool selected;
            [NonSerialized]
            public bool per_selected;
            [NonSerialized]
            public bool foldout;
            [NonSerialized]
            public FileIndexState fileIndexState;
            [NonSerialized]
            public List<int> childFileindexList;
        }
        public static void CopyDirectory(string inDir, string toDir, bool overwrite = true)
        {
            if (!System.IO.Directory.Exists(inDir)) return;
            EachFileAndDir(
                inDir, 
                (dirinfo) => {
                    var relativeFilePath = dirinfo.FullName.Replace(new System.IO.DirectoryInfo(inDir).FullName, "");
                    relativeFilePath = relativeFilePath.Replace("\\", "/");
                    if (relativeFilePath.Length > 0 && relativeFilePath[0] == '/')
                        relativeFilePath = relativeFilePath.Substring(1);
                    var path = System.IO.Path.Combine(toDir, relativeFilePath);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                }, 
                (fileinfo) => {
                    var relativeFilePath = fileinfo.FullName.Replace(new System.IO.DirectoryInfo(inDir).FullName, "");
                    relativeFilePath = relativeFilePath.Replace("\\", "/");
                    if (relativeFilePath.Length > 0 && relativeFilePath[0] == '/')
                        relativeFilePath = relativeFilePath.Substring(1);
                    var path = System.IO.Path.Combine(toDir, relativeFilePath);
                    if (System.IO.File.Exists(path) && !overwrite) return;
                    if(System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    var filedir = System.IO.Path.GetDirectoryName(path);
                    if (!System.IO.Directory.Exists(filedir))
                        System.IO.Directory.CreateDirectory(filedir);
                    System.IO.File.WriteAllBytes(path, System.IO.File.ReadAllBytes(fileinfo.FullName));
                });
        }
        public static void DeleteDirectory(string dir, bool deleteWithMataFile = false)
        {
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
            if (deleteWithMataFile)
            {
                var dirMetaFilePath = dir + ".meta";
                if (System.IO.File.Exists(dirMetaFilePath))
                    System.IO.File.Delete(dirMetaFilePath);
            }
        }
        public static void EachFileAndDir(string inDir, System.Action<System.IO.DirectoryInfo> findDirAction, System.Action<System.IO.FileInfo> findFileAction)
        {
            if (!System.IO.Directory.Exists(inDir)) return;
            var dirinfo = new System.IO.DirectoryInfo(inDir);
            foreach (var dir in dirinfo.GetDirectories())
            {
                if(findDirAction != null) findDirAction(dir);
                EachFileAndDir(dir.FullName, findDirAction, findFileAction);
            }
            foreach (var fileinfo in dirinfo.GetFiles())
                if (findFileAction != null) findFileAction(fileinfo);
        }
        public static void Pack(string inDir, string outFilepath, bool exportConfigJsonFile = true, bool exportFileIndexsJsonFile = true, System.Action<string, float, bool, string> progress = null, bool overwrite = true)
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

            EachFileAndDir(
                inDir,
                (dirinfo) => {
                     var fileIndex = new FileIndex();
                     fileIndex.isDir = true;
                     fileIndex.fileName = dirinfo.Name;
                     fileIndex.relativeFilePath = dirinfo.FullName.Replace(new System.IO.DirectoryInfo(inDir).FullName, "");
                     fileIndex.relativeFilePath = fileIndex.relativeFilePath.Replace("\\", "/");
                     if (fileIndex.relativeFilePath.Length > 0 && fileIndex.relativeFilePath[0] == '/')
                         fileIndex.relativeFilePath = fileIndex.relativeFilePath.Substring(1);
                     fileIndex.filePosition = -1;
                     fileIndex.fileBytesLength = -1;
                     fileIndexs.fileIndexList.Add(fileIndex);
                 },
                (fileinfo) => {
                    var fileIndex = new FileIndex();
                    fileIndex.fileName = fileinfo.Name;
                    fileIndex.relativeFilePath = fileinfo.FullName.Replace(new System.IO.DirectoryInfo(inDir).FullName, "");
                    fileIndex.relativeFilePath = fileIndex.relativeFilePath.Replace("\\", "/");
                    if (fileIndex.relativeFilePath.Length > 0 && fileIndex.relativeFilePath[0] == '/')
                        fileIndex.relativeFilePath = fileIndex.relativeFilePath.Substring(1);
                    fileIndex.filePosition = outFileStream.Position;
                    var infileBytes = System.IO.File.ReadAllBytes(fileinfo.FullName);
                    fileIndex.fileBytesLength = infileBytes.LongLength;
                    fileIndex.MD5Value = CheckMD5(fileinfo.FullName);
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

            var md5 = CheckMD5(outFilepath);
            if (exportConfigJsonFile)
            {
                var configPath = System.IO.Path.Combine(outdir, outfilename + "." + KitConst.KitPackageConfigFilename);
                var inconfigPath = System.IO.Path.Combine(inDir, KitConst.KitPackageConfigFilename);
                if(System.IO.File.Exists(inconfigPath))
                {
                    if (System.IO.File.Exists(configPath))
                        System.IO.File.Delete(configPath);
                    var config = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(inconfigPath, System.Text.Encoding.UTF8));
                    config.MD5Value = md5;
                    System.IO.File.WriteAllText(configPath, JsonUtility.ToJson(config, true));
                }
                else
                    Debug.LogWarning(KitConst.KitName + ": 配置文件 " + KitConst.KitPackageConfigFilename + " 文件不存在！\n包：" + inDir);
            }

            if (exportFileIndexsJsonFile)
            {
                fileIndexs.MD5Value = md5;
                jsonString = JsonUtility.ToJson(fileIndexs, true);
                var fileIndexsJsonPath = System.IO.Path.Combine(outdir, outfilename + ".fileIndexs.json");
                if (System.IO.File.Exists(fileIndexsJsonPath))
                    System.IO.File.Delete(fileIndexsJsonPath);
                System.IO.File.WriteAllText(fileIndexsJsonPath, jsonString);
            }

            if (progress != null) progress("", 1, true, null);
        }
        public static void Unpack(string inPackageFilepath, string outDir, System.Action<string, float, bool, string, List<string>> progress = null, bool overwrite = true, bool runFileSettings = true, List<FileIndex> withFileIndexList = null)
        {
            if (progress != null) progress("", 0, false,null, null);
            var fs = System.IO.File.OpenRead(inPackageFilepath);

            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var tagCount = tagbytes.Length;
            var tagBytes = new byte[tagCount];
            fs.Seek(-tagCount, System.IO.SeekOrigin.End);
            fs.Read(tagBytes, 0, tagCount);
            var getTag = System.Text.Encoding.UTF8.GetString(tagBytes);
            if(!getTag.StartsWith(FileFormat))
            {
                fs.Close();
                if (progress != null) progress("", 1, true, "该文件不是" + FileFormat + "文件格式，无法解析！", null);
                return;
            }

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
                if(withFileIndexList != null)
                {
                    bool can = true;
                    foreach(var fi in withFileIndexList)
                    {
                        if(fi.relativeFilePath == fileindex.relativeFilePath)
                        {
                            if (!fi.selected)
                                can = false;
                            break;
                        }
                    }
                    if (!can) continue;
                }
                if (fileindex.isDir)
                {
                    var dirpath = System.IO.Path.Combine(outDir, fileindex.relativeFilePath);
                    if (!System.IO.Directory.Exists(dirpath))
                        System.IO.Directory.CreateDirectory(dirpath);
                }
                else
                {
                    var filepath = System.IO.Path.Combine(outDir, fileindex.relativeFilePath);
                    if (System.IO.File.Exists(filepath) && !overwrite) continue;
                    else if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
                    var filedir = System.IO.Path.GetDirectoryName(filepath);
                    if (!System.IO.Directory.Exists(filedir))
                        System.IO.Directory.CreateDirectory(filedir);
                    int count = (int)fileindex.fileBytesLength;
                    byte[] bytes = new byte[count];
                    fs.Seek((int)fileindex.filePosition, System.IO.SeekOrigin.Begin);
                    fs.Read(bytes, 0, count);
                    System.IO.File.WriteAllBytes(filepath, bytes);
                }
                if (progress != null) progress(fileindex.fileName, (i + 1) / (float)fileIndexs.fileIndexList.Count, false, null, null);
            }
            fs.Close();

            var kitPackageConfigFilepath = System.IO.Path.Combine(outDir, KitConst.KitPackageConfigFilename);
            if(System.IO.File.Exists(kitPackageConfigFilepath))
            {
                var kitPackageConfig = JsonUtility.FromJson<KSwordKit.KitPackageConfig>(System.IO.File.ReadAllText(kitPackageConfigFilepath, System.Text.Encoding.UTF8));
                // 整理文件
                if (runFileSettings && kitPackageConfig.FileSettings != null && kitPackageConfig.FileSettings.Count > 0)
                {
                    var fileFileSettings = new List<KitPackageConfigFileSetting>();
                    var dirFileSettings = new List<KitPackageConfigFileSetting>();
                    foreach (var fileSetting in kitPackageConfig.FileSettings)
                        if (fileSetting.isDir) dirFileSettings.Add(fileSetting);
                        else fileFileSettings.Add(fileSetting);
                    foreach (var fileSetting in fileFileSettings)
                    {
                        var filepath = System.IO.Path.Combine(outDir, fileSetting.SourcePath);
                        var targetPath = KitPackageConfigFileSetting.TargetPathToRealPath(fileSetting.TargetPath);
                        if (!System.IO.File.Exists(filepath) && System.IO.File.Exists(targetPath)) continue;
                        if (!System.IO.File.Exists(filepath) && !System.IO.File.Exists(targetPath))
                        {
                            Debug.LogWarning(KitConst.KitName + ": 文件意外丢失！ " + filepath);
                            continue;
                        }
                        if (System.IO.File.Exists(targetPath) && !overwrite) continue;
                        if (System.IO.File.Exists(targetPath)) System.IO.File.Delete(targetPath);
                        var targetDir = System.IO.Path.GetDirectoryName(targetPath);
                        if (!System.IO.Directory.Exists(targetDir))
                            System.IO.Directory.CreateDirectory(targetDir);
                        System.IO.File.WriteAllBytes(targetPath, System.IO.File.ReadAllBytes(filepath));
                        System.IO.File.Delete(filepath);
                    }
                    var temp_dirFileSettings = new List<KitPackageConfigFileSetting>();
                    while(temp_dirFileSettings.Count != dirFileSettings.Count)
                    {
                        foreach (var fileSetting in dirFileSettings)
                        {
                            if (temp_dirFileSettings.Contains(fileSetting)) continue;
                            bool find = false;
                            foreach (var _fileSetting in dirFileSettings)
                            {
                                if (fileSetting == _fileSetting || temp_dirFileSettings.Contains(_fileSetting)) continue;
                                if (_fileSetting.SourcePath.StartsWith(fileSetting.SourcePath))
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if(!find)
                                temp_dirFileSettings.Add(fileSetting);
                        }
                    }
                    foreach (var fileSetting in temp_dirFileSettings)
                    {
                        var dirpath = System.IO.Path.Combine(outDir, fileSetting.SourcePath);
                        var targetPath = KitPackageConfigFileSetting.TargetPathToRealPath(fileSetting.TargetPath);
                        if (!System.IO.Directory.Exists(dirpath) && System.IO.Directory.Exists(targetPath)) continue;
                        if (!System.IO.Directory.Exists(dirpath))
                        {
                            Debug.LogWarning(KitConst.KitName + ": 文件目录意外丢失！ " + dirpath);
                            if (!System.IO.Directory.Exists(targetPath))
                                System.IO.Directory.CreateDirectory(targetPath);
                            continue;
                        }
                        CopyDirectory(dirpath, targetPath);
                        DeleteDirectory(dirpath);
                    }

                    fileFileSettings.Clear();
                    fileFileSettings = null;
                    dirFileSettings.Clear();
                    dirFileSettings = null;
                    temp_dirFileSettings.Clear();
                    temp_dirFileSettings = null;
                }
                // 输出依赖
                if (progress != null) progress("", 1, true, null, kitPackageConfig.Dependencies);
            }
            else if (progress != null) progress("", 1, true, null, null);
        }
        public static string Unpack_getFileIndexs_Text(string inPackageFilepath, out string error)
        {
            var fs = System.IO.File.OpenRead(inPackageFilepath);

            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var tagCount = tagbytes.Length;
            var tagBytes = new byte[tagCount];
            fs.Seek(-tagCount, System.IO.SeekOrigin.End);
            fs.Read(tagBytes, 0, tagCount);
            var getTag = System.Text.Encoding.UTF8.GetString(tagBytes);
            if (!getTag.StartsWith(FileFormat))
            {
                error = "该文件不是" + FileFormat + "文件格式，无法解析！";
                fs.Close();
                return null;
            }

            error = null;
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
            fs.Close();
            return jsonstring;
        }
        public static bool IsKKP(string inPackageFilepath)
        {
            var fs = System.IO.File.OpenRead(inPackageFilepath);
            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var tagCount = tagbytes.Length;
            var tagBytes = new byte[tagCount];
            fs.Seek(-tagCount, System.IO.SeekOrigin.End);
            fs.Read(tagBytes, 0, tagCount);
            var getTag = System.Text.Encoding.UTF8.GetString(tagBytes);
            fs.Close();
            return getTag.StartsWith(FileFormat);
        }
        public static string Unpack_getKitPackageConfig_Text(string inPackageFilepath, out string error)
        {
            var fs = System.IO.File.OpenRead(inPackageFilepath);

            var tagbytes = System.Text.Encoding.UTF8.GetBytes(tag);
            var tagCount = tagbytes.Length;
            var tagBytes = new byte[tagCount];
            fs.Seek(-tagCount, System.IO.SeekOrigin.End);
            fs.Read(tagBytes, 0, tagCount);
            var getTag = System.Text.Encoding.UTF8.GetString(tagBytes);
            if (!getTag.StartsWith(FileFormat))
            {
                error = "该文件不是" + FileFormat + "文件格式，无法解析！";
                fs.Close();
                return null;
            }

            error = null;
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
            for (var i = 0; i < fileIndexs.fileIndexList.Count; i++)
            {
                var fileindex = fileIndexs.fileIndexList[i];
                if(fileindex.fileName == KitConst.KitPackageConfigFilename)
                {
                    int count = (int)fileindex.fileBytesLength;
                    byte[] bytes = new byte[count];
                    fs.Seek((int)fileindex.filePosition, System.IO.SeekOrigin.Begin);
                    fs.Read(bytes, 0, count);
                    var configjsonstr = System.Text.Encoding.UTF8.GetString(bytes);
                    fs.Close();
                    return configjsonstr;
                }
            }
            fs.Close();
            error = "该文件是" + FileFormat + "文件，但是它里面不包含 " + KitConst.KitPackageConfigFilename + " 文件！";
            return null;
        }
        public static string CheckMD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    return System.BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","");
                }
            }
        }
    }
}
