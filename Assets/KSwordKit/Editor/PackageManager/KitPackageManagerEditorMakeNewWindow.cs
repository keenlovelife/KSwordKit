using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KSwordKit.Editor.PackageManager
{
    public class KitPackageManagerEditorMakeNewWindow : EditorWindow
    {
        class PGKDir
        {
            public bool selected;
            public string dirpath;
            public string dispalypath;
            public GUIStyle richText;
            public KitPackageConfig Config;
        }

        static KitPackageManagerEditorMakeNewWindow window;
        static KitPackageConfig newConfig;
        static string windowTitle;
        /// <summary>
        /// 窗口打开显示函数
        /// </summary>
        /// <param name="data">窗口数据</param>
        public static void Open(string subtitle, bool needinit = false)
        {
            windowTitle = KitConst.KitName + "：" + subtitle;
            window = GetWindow<KitPackageManagerEditorMakeNewWindow>(true, windowTitle);
            window.minSize = new Vector2(600, 800);
            window.blod = new GUIStyle();
            if (needinit)
            {
                if (string.IsNullOrEmpty(window.packageDir))
                {
                    var temp = System.IO.Path.Combine(Application.temporaryCachePath, window.tempPackageName);
                    if (System.IO.File.Exists(temp))
                        window.packageDir = System.IO.File.ReadAllText(temp);
                }
                var _temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageNames);
                var lines = System.IO.File.ReadAllLines(_temp);
                if (packageDirs == null) packageDirs = new List<PGKDir>();
                packageDirs.Clear();
                foreach(var line in lines)
                {
                    var r = new GUIStyle();
                    r.fontSize = 11;
                    r.normal.textColor = Color.white;
                    var configPath = System.IO.Path.Combine(line, KitConst.KitPackageConfigFilename);
                    KitPackageConfig kitPackageConfig = null;
                    if (System.IO.File.Exists(configPath))
                        kitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(configPath, System.Text.Encoding.UTF8));
                    else
                        kitPackageConfig = new KitPackageConfig();
                    packageDirs.Add(new PGKDir()
                    {
                        selected = true,
                        dirpath = line,
                        dispalypath = System.IO.Path.GetFileName(line),
                        richText = r,
                        Config = kitPackageConfig
                    });
                }
                window.init();
            }
        }

        public class FileItem
        {
            public string filename;
            public string filepath;
            public string rootpath;
            public string relativepath;
            public bool isDir;
            public bool foldout;
            public bool enableUnstaill = true;
            public string settingpath;
            public FileItem partentFileItem;
            public List<FileItem> fileItems;

            public static FileItem MakeNew(string rootpath, string path)
            {
                var fileitem = new FileItem();
                if (System.IO.Directory.Exists(path))
                    fileitem.isDir = true;
                fileitem.rootpath = rootpath;
                fileitem.rootpath = fileitem.rootpath.Replace("\\", "/");
                fileitem.filepath = path;
                fileitem.filepath = fileitem.filepath.Replace("\\", "/");
                fileitem.relativepath = fileitem.filepath.Replace(fileitem.rootpath, "");
                fileitem.relativepath = fileitem.relativepath.Replace("\\", "/");
                if (fileitem.relativepath.StartsWith("/")) fileitem.relativepath = fileitem.relativepath.Substring(1);
                fileitem.filename = System.IO.Path.GetFileName(path);
                fileitem.fileItems = new List<FileItem>();

                if (!fileitem.isDir) return fileitem;

                var dirinfo = new System.IO.DirectoryInfo(path);
                foreach (var fileinfo in dirinfo.GetFiles())
                {
                    var fi = MakeNew(rootpath, fileinfo.FullName);
                    fi.partentFileItem = fileitem;
                    fileitem.fileItems.Add(fi);
                }
                foreach (var dir in dirinfo.GetDirectories())
                {
                    var fi = MakeNew(rootpath, dir.FullName);
                    fi.partentFileItem = fileitem;
                    fileitem.fileItems.Add(fi);
                }
                return fileitem;
            }
        }

        bool supportMultipleSelect = true;
        static List<PGKDir> packageDirs = new List<PGKDir>();

        string _name = "";
        string version = "";
        string author = "";
        string contact = "";
        string homePage = "";
        string description = "";
        Vector2 scorllPos_pgkDirs;
        Vector2 scorllPos;
        Vector2 scorllPos_dependencies;
        Vector2 scorllPos_fileSetting;
        GUIStyle blod;
        string packageDir = "";
        string Tags = "以';'号分割标签";
        string Dependencies = "以';'号分割依赖";
        static FileItem packageItem;
        string tempPackageName = ".tempPackage";
        static string tempPackageNames = ".tempPackages";
        bool showSetConfig = true;
        int spaceCount = 40;
        int space = 40;
        bool foldout = false;
        bool exportJsonFile = true;
        bool exportConfigFile = true;


        void init(PGKDir pgk = null)
        {
            if (!string.IsNullOrEmpty(packageDir))
            {
                packageItem = FileItem.MakeNew(packageDir, packageDir);

                var configPath = System.IO.Path.Combine(packageDir, KitConst.KitPackageConfigFilename);
                if (System.IO.File.Exists(configPath))
                    newConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(configPath, System.Text.Encoding.UTF8));
                else
                    newConfig = new KitPackageConfig();
                if (pgk != null) pgk.Config = newConfig;
                _name = newConfig.Name;
                if (string.IsNullOrEmpty(_name))
                    _name = System.IO.Path.GetFileName(packageDir);
                version = newConfig.Version;
                if (string.IsNullOrEmpty(version))
                    version = "v1.0.0";
                author = newConfig.Author;
                contact = newConfig.Contact;
                homePage = newConfig.HomePage;
                description = newConfig.Description;
                if (newConfig.Dependencies != null && newConfig.Dependencies.Count > 0)
                {
                    var d = "";
                    foreach (var de in newConfig.Dependencies)
                        d += de + ";\n";
                    Dependencies = d;
                }
                else
                    Dependencies = "以';'号分割依赖";
                if (newConfig.Tags != null && newConfig.Tags.Count > 0)
                {
                    var t = "";
                    foreach (var _T in newConfig.Tags)
                        t += _T + ";";
                    Tags = t;
                }
                else
                    Tags = "以';'号分割标签";
                if (newConfig.FileSettings != null && newConfig.FileSettings.Count > 0)
                {
                    foreach (var f in newConfig.FileSettings)
                    {
                        setConfigFileSettings(packageItem, f);
                    }
                }
            }
        }
        private void OnGUI()
        {
            if(window == null)
                Open(KitPackageManagerEditor.MakeNewWindowTitle, true);

            blod.fontSize = 13;
            blod.normal.textColor = new Color(255, 200, 200);
            EditorGUILayout.Space(10);
            GUI.enabled = false;
            GUILayout.Button("您需要先选择包所在目录，填写包配置信息后，导出包文件。", GUILayout.Height(40));
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("");
            supportMultipleSelect = EditorGUILayout.Toggle(supportMultipleSelect, GUILayout.Width(10));
            GUILayout.Label("批量导出包", GUILayout.Width(80));
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            EditorGUILayout.LabelField("选择包目录：", blod, GUILayout.Width(80));
            blod.fontSize = 12;
            if (supportMultipleSelect)
            {
                float scorllHeight = packageDirs.Count * 20;
                if (scorllHeight < 100) scorllHeight = 100;
                else if (scorllHeight > window.position.height / 2) scorllHeight = window.position.height / 2;
                scorllPos_pgkDirs = EditorGUILayout.BeginScrollView(scorllPos_pgkDirs, false, false, GUILayout.Height(scorllHeight));
                GUILayout.BeginVertical();
                PGKDir removepgk = null;
                foreach (var pgkdir in packageDirs)
                {
                    if (showSetConfig && newConfig != null && !string.IsNullOrEmpty(packageDir) && pgkdir.dirpath == packageDir)
                        pgkdir.richText.normal.textColor = Color.yellow;
                    GUILayout.BeginHorizontal();
                    pgkdir.selected = EditorGUILayout.Toggle(pgkdir.selected, GUILayout.Width(15));
                    if(GUILayout.Button(pgkdir.dispalypath, pgkdir.richText))
                    {
                        pgkdir.richText.normal.textColor = Color.yellow;
                        foreach(var _pgk in packageDirs)
                        {
                            if (_pgk == pgkdir) continue;
                            _pgk.richText.normal.textColor = Color.white;
                        }
                        packageDir = pgkdir.dirpath;
                        var temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageName);
                        if (System.IO.File.Exists(temp))
                            System.IO.File.Delete(temp);
                        System.IO.File.WriteAllText(temp, packageDir);
                        init(pgkdir);
                    }
                    if (GUILayout.Button("×", GUILayout.Width(20)))
                    {
                        removepgk = pgkdir;
                        if (packageDir == removepgk.dirpath)
                        {
                            packageDir = "";
                            var temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageName);
                            if (System.IO.File.Exists(temp))
                                System.IO.File.Delete(temp);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Button("", GUILayout.Height(1));
                }
                if (removepgk != null)
                {
                    packageDirs.Remove(removepgk);
                    var temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageNames);
                    if (System.IO.File.Exists(temp))
                        System.IO.File.Delete(temp);
                    if(packageDirs.Count > 0)
                    {
                        var content = "";
                        foreach (var pgk in packageDirs)
                        {
                            if (string.IsNullOrEmpty(content)) content = pgk.dirpath;
                            else content += "\n" + pgk.dirpath;
                        }
                        System.IO.File.WriteAllText(temp, content);
                    }
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                packageDir = EditorGUILayout.TextField(packageDir);
            }

            if (GUILayout.Button("浏览", GUILayout.Width(90), GUILayout.Height(30)))
            {
                packageDir = EditorUtility.OpenFolderPanel(windowTitle, packageDir, "");
                if (!string.IsNullOrEmpty(packageDir))
                {
                    if (supportMultipleSelect)
                    {
                        bool find = false;
                        foreach (var pgk in packageDirs)
                            if (pgk.dirpath == packageDir)
                            {
                                find = true;
                                break;
                            }
                        if (!find) 
                        {
                            var r = new GUIStyle();
                            r.fontSize = 11;
                            r.normal.textColor = Color.white;
                            var configPath = System.IO.Path.Combine(packageDir, KitConst.KitPackageConfigFilename);
                            if (System.IO.File.Exists(configPath))
                                newConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(configPath, System.Text.Encoding.UTF8));
                            else
                                newConfig = new KitPackageConfig();
                            packageDirs.Add(new PGKDir()
                            {
                                selected = true,
                                dirpath = packageDir,
                                dispalypath = System.IO.Path.GetFileName(packageDir),
                                richText = r,
                                Config = newConfig
                            });
                            var _temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageNames);
                            var content = packageDir;
                            if (System.IO.File.Exists(_temp))
                            {
                                var c = System.IO.File.ReadAllText(_temp);
                                if (!string.IsNullOrEmpty(c)) content = c + "\n" + content;
                            }
                            System.IO.File.WriteAllText(_temp, content);
                        } 
                    }
                    var temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageName);
                    if (System.IO.File.Exists(temp))
                        System.IO.File.Delete(temp);
                    System.IO.File.WriteAllText(temp, packageDir);
                    init();
                }
            }
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();


            if (!string.IsNullOrEmpty(packageDir))
            {
                EditorGUILayout.Space(10);
                if(GUILayout.Button("设置包配置", GUILayout.Height(25)))
                {
                    showSetConfig = !showSetConfig;
                }
                GUILayout.Space(2);
                if (showSetConfig)
                {
                    scorllPos = EditorGUILayout.BeginScrollView(scorllPos, false, false);
                    EditorGUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("ID：", blod, GUILayout.Width(60));
                    newConfig.ID = EditorGUILayout.TextField(_name + "@" + version);
                    GUILayout.Space(space);
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("名称：", blod, GUILayout.Width(60));
                    _name = EditorGUILayout.TextField(_name);
                    newConfig.Name = _name;
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("版本：", blod, GUILayout.Width(60));
                    version = EditorGUILayout.TextField(version);
                    newConfig.Version = version;
                    newConfig.liveWithOtherVersion = EditorGUILayout.Toggle("", newConfig.liveWithOtherVersion, GUILayout.Width(15));
                    EditorGUILayout.LabelField("可以和旧版共存", GUILayout.Width(120));
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("作者：", blod, GUILayout.Width(60));
                    author = EditorGUILayout.TextField(author);
                    newConfig.Author = author;
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("联系：", blod, GUILayout.Width(60));
                    contact = EditorGUILayout.TextField(contact);
                    newConfig.Contact = contact;
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("主页：", blod, GUILayout.Width(60));
                    homePage = EditorGUILayout.TextField(homePage);
                    newConfig.HomePage = homePage;
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("日期：", blod, GUILayout.Width(60));
                    newConfig.Date = EditorGUILayout.TextField(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    GUILayout.Space(space);
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("标签：", blod, GUILayout.Width(60));
                    Tags = EditorGUILayout.TextField(Tags);
                    if (!string.IsNullOrEmpty(Tags) && Tags != "以';'号分割标签")
                    {
                        if (newConfig.Tags == null)
                            newConfig.Tags = new List<string>();
                        newConfig.Tags.Clear();
                        var _tags = Tags.Split(';');
                        foreach (var tag in _tags)
                        {
                            if (!string.IsNullOrEmpty(tag))
                                newConfig.Tags.Add(tag);
                        }
                    }
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("描述：", blod, GUILayout.Width(60));
                    description = EditorGUILayout.TextArea(description, GUILayout.Height(100));
                    newConfig.Description = description;
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    EditorGUILayout.LabelField("依赖：", blod, GUILayout.Width(60));

                    GUILayout.BeginVertical();
                    if (newConfig.Dependencies == null) newConfig.Dependencies = new List<string>();
                    newConfig.Dependencies.Clear();
                    Dependencies = EditorGUILayout.TextArea(Dependencies, GUILayout.Height(100));
                    var ds = Dependencies.Replace("\n", "").Split(';');
                    foreach (var d in ds)
                        if (!string.IsNullOrEmpty(d) && Dependencies != "以';'号分割依赖" && !newConfig.Dependencies.Contains(d))
                            newConfig.Dependencies.Add(d);
                    GUILayout.BeginHorizontal();
                    blod.fontSize = 11;
                    EditorGUILayout.LabelField("可用包列表 -> ", blod, GUILayout.Width(80));
                    scorllPos_dependencies = EditorGUILayout.BeginScrollView(scorllPos_dependencies, false, false, GUILayout.Height(120));
                    if (KitInitializeEditor.KitOriginConfig != null && KitInitializeEditor.KitOriginConfig.PackageList != null)
                        foreach (var packageConfigID in KitInitializeEditor.KitOriginConfig.PackageList)
                        {
                            GUILayout.Button("", GUILayout.Height(1));
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(packageConfigID, blod);
                            var dslist = new List<string>();
                            dslist.AddRange(ds);
                            if (dslist.Contains(packageConfigID))
                                GUI.enabled = false;
                            if (GUILayout.Button("Add", GUILayout.Width(40), GUILayout.Height(20)))
                            {
                                if (Dependencies == "以';'号分割依赖")
                                    Dependencies = packageConfigID + ";";
                                else
                                    Dependencies += "\n" + packageConfigID + ";";
                            }
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                        }
                    GUILayout.Button("", GUILayout.Height(1));
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();


                    EditorGUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(space);
                    blod.fontSize = 13;
                    GUILayout.BeginVertical();
                    foldout = EditorGUILayout.Foldout(foldout, "特定文件导出设置", true);
                    if (foldout)
                    {
                        if (packageItem == null || packageItem.fileItems == null)
                        {
                            var temp = System.IO.Path.Combine(Application.temporaryCachePath, tempPackageName);
                            if (System.IO.File.Exists(temp))
                            {
                                var packagedir = System.IO.File.ReadAllText(temp);
                                packageItem = null;
                                packageItem = FileItem.MakeNew(packageDir, packageDir);
                            }
                        }

                        if (packageItem.partentFileItem != null)
                        {
                            if (GUILayout.Button("返回上级目录", GUILayout.Width(100)))
                            {
                                packageItem = packageItem.partentFileItem;
                            }
                        }

                        scorllPos_fileSetting = EditorGUILayout.BeginScrollView(scorllPos_fileSetting, false, false, GUILayout.Height(180));
                        blod.fontSize = 11;

                        if (packageItem != null)
                        {
                            string label = "../" + packageItem.filename + "/";
                            EditorGUILayout.LabelField(label, blod);
                            foreach (var item in packageItem.fileItems)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(15);
                                if (item.isDir)
                                {
                                    item.foldout = EditorGUILayout.Foldout(item.foldout, item.filename, true);
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(item.filename, blod);
                                }
                                item.enableUnstaill = EditorGUILayout.Toggle("", item.enableUnstaill, GUILayout.Width(15));
                                EditorGUILayout.LabelField("卸载时删除", GUILayout.Width(65));
                                item.settingpath = EditorGUILayout.TextField(item.settingpath, GUILayout.Width(190));
                                if (!string.IsNullOrEmpty(item.settingpath) && !item.settingpath.EndsWith(item.filename))
                                    item.settingpath += "/" + item.filename;

                                GUILayout.EndHorizontal();
                                if (item.foldout)
                                {
                                    item.foldout = false;
                                    packageItem = item;
                                }
                            }

                            var _item = packageItem;
                            while (_item.partentFileItem != null)
                                _item = _item.partentFileItem;
                            setConfig(_item, newConfig);
                        }

                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(space);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.Space(10);

                    var json = JsonUtility.ToJson(newConfig, true);
                    var configPath = System.IO.Path.Combine(packageDir, KitConst.KitPackageConfigFilename);
                    if (System.IO.File.Exists(configPath))
                        System.IO.File.Delete(configPath);
                    System.IO.File.WriteAllText(configPath, json);

                    if (!supportMultipleSelect)
                    {
                        if (GUILayout.Button("导出", GUILayout.Height(45)))
                        {
                            saveFile();
                        }
                        //exportJsonFile = EditorGUILayout.Toggle("同时导出json文件", exportJsonFile);
                        //exportConfigFile = EditorGUILayout.Toggle("同时导出配置文件", exportConfigFile);
                    }
                    else
                    {
                        foreach(var pgk in packageDirs)
                            if(pgk.dirpath == packageDir)
                            {
                                pgk.Config = newConfig;
                                break;
                            }
                    }
                }
            }

            if (supportMultipleSelect)
            {
                var packLabel = "批量导出";
                var count = 0;
                foreach (var pgk in packageDirs)
                    if (pgk.selected) count++;
                packLabel += " (" + count + ")";
                if (GUILayout.Button(packLabel, GUILayout.Height(45)))
                {
                    saveFiles();
                }
            }
            GUILayout.Space(20);
        }

        string saveDir = "";
        void saveFile()
        {
            if (checkConfig())
            {
                bool can = true;
                var savepath = EditorUtility.SaveFilePanel("导出到:", saveDir, newConfig.ID, "kkp");
                if (string.IsNullOrEmpty(savepath))
                    can = false;

                if (can)
                {
                    var json = JsonUtility.ToJson(newConfig, true);
                    var configPath = System.IO.Path.Combine(packageDir, KitConst.KitPackageConfigFilename);
                    if (System.IO.File.Exists(configPath))
                        System.IO.File.Delete(configPath);
                    System.IO.File.WriteAllText(configPath, json);

                    KitPacker.Pack(packageDir, savepath, exportConfigFile, exportJsonFile, (filename, progress, done, error) =>
                    {
                        EditorUtility.DisplayCancelableProgressBar(windowTitle + System.IO.Path.GetFileName(savepath),
                            "正在导出: " + filename, progress);
                        if (done && string.IsNullOrEmpty(error))
                        {
                            EditorUtility.DisplayDialog(windowTitle, "导出成功！\n\n成功导出 " + System.IO.Path.GetFileName(savepath), "确认");
                            saveDir = System.IO.Path.GetDirectoryName(savepath);
                        }
                        else if (done)
                        {
                            EditorUtility.DisplayDialog(windowTitle, "导出失败：" + error, "确认");
                        }

                        if (done)
                            EditorUtility.ClearProgressBar();
                    });
                }
            }
            else
            {
                EditorUtility.DisplayDialog(windowTitle, "以下字段不能为空！\n\n‘名称’\n‘版本’\n‘描述’", "知道了");
            }
        }
        void saveFiles()
        {
            bool can = true;
            foreach (var _pgk in packageDirs)
                if(!checkConfig(_pgk.Config))
                {
                    can = false;
                    EditorUtility.DisplayDialog(windowTitle, _pgk.dispalypath + ": " + _pgk.dirpath + "的配置文件中\n\n‘名称’\n‘版本’\n‘描述’\n\n字段不能为空！\n", "知道了");
                    break;
                }
            if (can)
            {
                saveDir = EditorUtility.SaveFolderPanel("导出到:", saveDir, KitPacker.FileFormat);
                if (string.IsNullOrEmpty(saveDir))
                    can = false;
            }

            if (can)
            {
                int donecount = 0;
                string allerror = "";
                int successCount = 0;
                int errorCount = 0;
                string successInfo = "";
                string errorInfo = "";
                EditorUtility.DisplayProgressBar(windowTitle, "准备批量导出...", 0);
                foreach (var _pgk in packageDirs)
                {
                    var pgk = _pgk;
                    var json = JsonUtility.ToJson(pgk.Config, true);
                    var configPath = System.IO.Path.Combine(packageDir, KitConst.KitPackageConfigFilename);
                    if (System.IO.File.Exists(configPath))
                        System.IO.File.Delete(configPath);
                    System.IO.File.WriteAllText(configPath, json);

                    KitPacker.Pack(
                        pgk.dirpath,
                        saveDir + "/" + pgk.Config.ID + "." + KitPacker.FileFormat, 
                        exportConfigFile, 
                        exportJsonFile, 
                        (filename, progress, done, error) =>
                        {
                            EditorUtility.DisplayProgressBar(windowTitle, "正在批量导出: " + pgk.Config.ID + "->" + filename, (float)donecount / packageDirs.Count + (float)1 / packageDirs.Count * progress);
                            if (done)
                            {
                                donecount++;
                                if (string.IsNullOrEmpty(error))
                                {
                                    successCount++;
                                    if (string.IsNullOrEmpty(successInfo)) successInfo = "导出成功：" + pgk.Config.ID;
                                    else successInfo += "\n导出成功：" + pgk.Config.ID;
                                    EditorUtility.DisplayProgressBar(windowTitle, "批量导出: " + pgk.Config.ID + " 成功！", (float)donecount / packageDirs.Count);
                                }
                                else
                                {
                                    errorCount++;
                                    if (string.IsNullOrEmpty(errorInfo)) errorInfo = "导出失败：" + pgk.Config.ID;
                                    else errorInfo += "\n导出失败：" + pgk.Config.ID;
                                    if (string.IsNullOrEmpty(allerror))
                                        allerror = error;
                                    else allerror += "\n" + error;
                                }

                                if (donecount == packageDirs.Count)
                                {
                                    EditorUtility.ClearProgressBar();
                                    if (string.IsNullOrEmpty(allerror))
                                        EditorUtility.DisplayDialog(windowTitle, "全部批量导出成功！\n\n" + successInfo, "确定");
                                    else
                                    {
                                        Debug.LogError(KitConst.KitName + ": 批量导出失败! 成功(" + successCount + "), 失败(" + errorCount + ")\n\n" + successInfo + "\n\n" + errorInfo + "\n\n" + allerror);
                                        EditorUtility.DisplayDialog(windowTitle, "批量导出失败！成功(" + successCount + "), 失败(" + errorCount + ")\n\n" + successInfo + "\n\n" + errorInfo + "\n\n" + allerror, "确定");
                                    }
                                }
                            }
                        }
                    );
                }
            }
        }
        bool checkConfig(KitPackageConfig kitPackageConfig = null)
        {
            if (kitPackageConfig == null) kitPackageConfig = newConfig;
            if (kitPackageConfig.ID == "@" || string.IsNullOrEmpty(kitPackageConfig.ID)) return false;
            if (string.IsNullOrEmpty(kitPackageConfig.Name)) return false;
            if (string.IsNullOrEmpty(kitPackageConfig.Version)) return false;
            if (string.IsNullOrEmpty(kitPackageConfig.Description)) return false;
            return true;
        }

        void setConfigFileSettings(FileItem fileItem, KitPackageConfigFileSetting setting)
        {
            if (fileItem.relativepath == setting.SourcePath)
                fileItem.settingpath = setting.TargetPath;
            else if (fileItem.isDir)
            {
                foreach (var f in fileItem.fileItems)
                    setConfigFileSettings(f, setting);
            }
        }
        int findFileSetting(KitPackageConfig config, string relativepath)
        {
            if (config == null || config.FileSettings == null) return -1;

            for (var i = 0; i < config.FileSettings.Count; i++)
                if (relativepath == config.FileSettings[i].SourcePath) return i;

            return -1;
        }
        void setConfig(FileItem fileItem, KitPackageConfig config, string settingpath = "")
        {
            if (config.FileSettings == null)
                config.FileSettings = new List<KitPackageConfigFileSetting>();

            if (!fileItem.isDir)
            {
                if (!string.IsNullOrEmpty(fileItem.settingpath))
                {
                    var index = findFileSetting(config, fileItem.relativepath);
                    if (index == -1)
                        config.FileSettings.Add(new KitPackageConfigFileSetting()
                        {
                            enableUninstall = fileItem.enableUnstaill,
                            SourcePath = fileItem.relativepath,
                            TargetPath = fileItem.settingpath
                        });
                    else
                    {
                        config.FileSettings[index].enableUninstall = fileItem.enableUnstaill;
                        config.FileSettings[index].TargetPath = fileItem.settingpath;
                    }
                }
                else if (!string.IsNullOrEmpty(settingpath))
                {
                    var index = findFileSetting(config, fileItem.relativepath);
                    if (index == -1)
                        config.FileSettings.Add(new KitPackageConfigFileSetting()
                        {
                            enableUninstall = fileItem.enableUnstaill,
                            SourcePath = fileItem.relativepath,
                            TargetPath = settingpath + "/" + fileItem.filename
                        });
                    else
                    {
                        config.FileSettings[index].enableUninstall = fileItem.enableUnstaill;
                        config.FileSettings[index].TargetPath = settingpath + "/" + fileItem.filename;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(fileItem.settingpath))
                {
                    var index = findFileSetting(config, fileItem.relativepath);
                    if (index == -1)
                        config.FileSettings.Add(new KitPackageConfigFileSetting()
                        {
                            enableUninstall = fileItem.enableUnstaill,
                            isDir = true,
                            SourcePath = fileItem.relativepath,
                            TargetPath = fileItem.settingpath
                        });
                    else 
                    {
                        config.FileSettings[index].enableUninstall = fileItem.enableUnstaill;
                        config.FileSettings[index].isDir = true;
                        config.FileSettings[index].TargetPath = fileItem.settingpath;
                    }
                } 
                else if (!string.IsNullOrEmpty(settingpath))
                {
                    var index = findFileSetting(config, fileItem.relativepath);
                    if (index == -1)
                        config.FileSettings.Add(new KitPackageConfigFileSetting()
                        {
                            enableUninstall = fileItem.enableUnstaill,
                            isDir = true,
                            SourcePath = fileItem.relativepath,
                            TargetPath = settingpath + "/" + fileItem.filename
                        });
                    else
                    {
                        config.FileSettings[index].enableUninstall = fileItem.enableUnstaill;
                        config.FileSettings[index].isDir = true;
                        config.FileSettings[index].TargetPath = settingpath + "/" + fileItem.filename;
                    }
                }
                    
                foreach (var item in fileItem.fileItems)
                {
                    if (!item.isDir)
                    {
                        if (string.IsNullOrEmpty(item.settingpath))
                        {
                            if (!string.IsNullOrEmpty(fileItem.settingpath))
                            {
                                var index = findFileSetting(config, item.relativepath);
                                if (index == -1)
                                    config.FileSettings.Add(new KitPackageConfigFileSetting()
                                    {
                                        enableUninstall = item.enableUnstaill,
                                        SourcePath = item.relativepath,
                                        TargetPath = fileItem.settingpath + "/" + item.filename
                                    });
                                else
                                {
                                    config.FileSettings[index].enableUninstall = item.enableUnstaill;
                                    config.FileSettings[index].TargetPath = fileItem.settingpath + "/" + item.filename;
                                }
                            }
                            else if (!string.IsNullOrEmpty(settingpath))
                            {
                                var index = findFileSetting(config, item.relativepath);
                                if (index == -1)
                                    config.FileSettings.Add(new KitPackageConfigFileSetting()
                                    {
                                        enableUninstall = item.enableUnstaill,
                                        SourcePath = item.relativepath,
                                        TargetPath = settingpath + "/" + item.filename
                                    });
                                else
                                {
                                    config.FileSettings[index].enableUninstall = item.enableUnstaill;
                                    config.FileSettings[index].TargetPath = fileItem.settingpath + "/" + item.filename;
                                }
                            }
                        }
                        else
                        {
                            var index = findFileSetting(config, item.relativepath);

                            if (index == -1)
                                config.FileSettings.Add(new KitPackageConfigFileSetting()
                                {
                                    enableUninstall = item.enableUnstaill,
                                    SourcePath = item.relativepath,
                                    TargetPath = item.settingpath
                                });
                            else
                            {
                                config.FileSettings[index].enableUninstall = item.enableUnstaill;
                                config.FileSettings[index].TargetPath = item.settingpath;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.settingpath))
                            setConfig(item, config, item.settingpath);
                        else if (!string.IsNullOrEmpty(fileItem.settingpath))
                            setConfig(item, config, fileItem.settingpath);
                        else
                            setConfig(item, config, settingpath);
                    }
                }
            }
        }
    }
}

