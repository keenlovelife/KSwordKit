using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace KSwordKit
{
    public class KitFileDialog
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //如果这里写成class而不是struct，最后只能获得单文件，不能获得所有文件
        //参考链接：https://blog.csdn.net/weixin_39766005/article/details/103705178
        public struct OpenFileName
        {
            public int structSize;
            public IntPtr dlgOwner;
            public IntPtr instance;
            public String filter;
            public String customFilter;
            public int maxCustFilter;
            public int filterIndex;
            public String file;
            public int maxFile;
            public String fileTitle;
            public int maxFileTitle;
            public String initialDir;
            public String title;
            public int flags;
            public short fileOffset;
            public short fileExtension;
            public String defExt;
            public IntPtr custData;
            public IntPtr hook;
            public String templateName;
            public IntPtr reservedPtr;
            public int reservedInt;
            public int flagsEx;
        }
        //链接指定系统函数       打开文件对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        //链接指定系统函数        另存为对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenDialogDir
        {
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr pidlRoot = IntPtr.Zero;
            public String pszDisplayName = null;
            public String lpszTitle = null;
            public UInt32 ulFlags = 0;
            public IntPtr lpfn = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;
        }
        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
        /// <summary>
        /// 打开一个文件选择对话框（可多选）
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="filter">文件筛选；格式如："文本(*.txt)\0*.txt"</param>
        /// <param name="results">如果选择了多个文件，第一个元素是文件夹名，后面各个元素是纯文件名；如果只选择一个文件，则第一个元素文件完整路径。</param>
        /// <returns>确认或取</returns>
        public static bool Open(string title, string filter, out string[] results)
        {
            results = null;

            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = filter; // "所有文件(*.*)\0*.*";
            openFileName.file = new string(new char[1024]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.initialDir = "".Replace('/', '\\');//默认路径
            openFileName.title = title;
            //openFileName.defExt = "TXT";
            //0x00080000 | ==  OFN_EXPLORER |对于旧风格对话框，目录 和文件字符串是被空格分隔的，函数为带有空格的文件名使用短文件名
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

            // 这里面需要注意的是，如果openFileName.flags取消选择0x00080000，默认为旧格式（但大家应该不会去用这个，太丑了），多选时分割方式与新格式的对话框是不一样的
            // 分割出来的字符串，第一个元素是文件夹名，后面各个元素是纯文件名，中间全部用NULL字符串分割，将他们分离时使用”\0”作为分隔字符。

            if (GetOpenFileName(openFileName))
            {
                string[] SplitStr = { "\0" };
                results = openFileName.file.Split(SplitStr, StringSplitOptions.RemoveEmptyEntries);
                return true;
            }
            return false;
        }
        public static void OpenWindowsDialog()
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string(new char[2000]); ; // 存放目录路径缓冲区
            ofn2.lpszTitle = "Open Project";// 标题
                                            //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框
            IntPtr pidlPtr = SHBrowseForFolder(ofn2);

            char[] charArray = new char[2000];
            for (int i = 0; i < 2000; i++)
                charArray[i] = '\0';

            SHGetPathFromIDList(pidlPtr, charArray);
            string fullDirPath = new String(charArray);

            fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

            Debug.Log( fullDirPath);//这个就是选择的目录路径。
        }
    }
}
