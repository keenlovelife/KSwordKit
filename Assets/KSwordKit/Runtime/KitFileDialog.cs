using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace KSwordKit
{
    public class KitFileDialog
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //�������д��class������struct�����ֻ�ܻ�õ��ļ������ܻ�������ļ�
        //�ο����ӣ�https://blog.csdn.net/weixin_39766005/article/details/103705178
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
        //����ָ��ϵͳ����       ���ļ��Ի���
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        //����ָ��ϵͳ����        ���Ϊ�Ի���
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
        /// ��һ���ļ�ѡ��Ի��򣨿ɶ�ѡ��
        /// </summary>
        /// <param name="title">�Ի������</param>
        /// <param name="filter">�ļ�ɸѡ����ʽ�磺"�ı�(*.txt)\0*.txt"</param>
        /// <param name="results">���ѡ���˶���ļ�����һ��Ԫ�����ļ��������������Ԫ���Ǵ��ļ��������ֻѡ��һ���ļ������һ��Ԫ���ļ�����·����</param>
        /// <returns>ȷ�ϻ�ȡ</returns>
        public static bool Open(string title, string filter, out string[] results)
        {
            results = null;

            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = filter; // "�����ļ�(*.*)\0*.*";
            openFileName.file = new string(new char[1024]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.initialDir = "".Replace('/', '\\');//Ĭ��·��
            openFileName.title = title;
            //openFileName.defExt = "TXT";
            //0x00080000 | ==  OFN_EXPLORER |���ھɷ��Ի���Ŀ¼ ���ļ��ַ����Ǳ��ո�ָ��ģ�����Ϊ���пո���ļ���ʹ�ö��ļ���
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

            // ��������Ҫע����ǣ����openFileName.flagsȡ��ѡ��0x00080000��Ĭ��Ϊ�ɸ�ʽ�������Ӧ�ò���ȥ�������̫���ˣ�����ѡʱ�ָʽ���¸�ʽ�ĶԻ����ǲ�һ����
            // �ָ�������ַ�������һ��Ԫ�����ļ��������������Ԫ���Ǵ��ļ������м�ȫ����NULL�ַ����ָ�����Ƿ���ʱʹ�á�\0����Ϊ�ָ��ַ���

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
            ofn2.pszDisplayName = new string(new char[2000]); ; // ���Ŀ¼·��������
            ofn2.lpszTitle = "Open Project";// ����
                                            //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // �µ���ʽ,���༭��
            IntPtr pidlPtr = SHBrowseForFolder(ofn2);

            char[] charArray = new char[2000];
            for (int i = 0; i < 2000; i++)
                charArray[i] = '\0';

            SHGetPathFromIDList(pidlPtr, charArray);
            string fullDirPath = new String(charArray);

            fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

            Debug.Log( fullDirPath);//�������ѡ���Ŀ¼·����
        }
    }
}
