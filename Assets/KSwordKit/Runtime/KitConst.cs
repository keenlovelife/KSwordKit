using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit
{
    /// <summary>
    /// KSwordKit�е����г���
    /// <para>���������������ڶ���ĳ�������������ʹ�õĳ����ɰ��Լ������ʹ�á�</para>
    /// <para>����ĳ����������޸ġ����Է�ֹ���������</para>
    /// </summary>
    public class KitConst
    {
        /// <summary>
        /// KSwordKit����
        /// </summary>
        public const string KitName = "KSwordKit";
        public const string KitVersion = "v1.0.0";
        /// <summary>
        /// KSwordKit��װ��Ŀ¼
        /// <para>��Ŀ¼ָʾ��KSwordKit�ı��û���װ�����ڵĸ�Ŀ¼��Ҳ��KSwordKit�Ĺ���Ŀ¼</para>
        /// <para>KSwordKit�ڵ�������а�Ҳ������ڸ�Ŀ¼�ڣ�������Ŀ��������ձ��롣</para>
        /// </summary>
        public static string KitInstallationDirectory
        {
            get
            {
                if (config == null)
                    config = Resources.Load<KSwordKitConfig>("KSwordKitConfig");
                if (config == null) return null;
                return config.KitInstallationPath;
            }
        }
        static KSwordKit.KSwordKitConfig config;
        /// <summary>
        /// KSwordKit�ѵ�����Ŀ�е����а����ڵĸ�Ŀ¼
        /// <para>��Ŀ¼ָʾ���ڵ�� `�����` ʱ���ð����ᱻ���뵽��Ŀ¼�С�</para>
        /// </summary>
        public const string KitPackagesImportRootDirectory = "Packages";
        /// <summary>
        /// ���������ļ���
        /// </summary>
        public const string KitPackageConfigFilename = "kitPackageConfig.json";
        /// <summary>
        /// ������ʱ����Դ�л�ȡ�İ������ļ���
        /// </summary>
        public const string KitOriginPackageConfigFilename = ".KitConfig.json";
        /// <summary>
        /// KSwordKit���õ����еİ����ڵĸ�Ŀ¼
        /// <para>��Ŀ¼ָʾ�����û�ִ�� `�����`ʱ, ���˴���ȡ���ݡ�</para>
        /// <para>��Ŀ¼�ڵ�ǰUnity��Ŀ��Ŀ¼�£���������� `Assets` Ŀ¼�У����ⱻUnity�༭�����롣</para>
        /// </summary>
        public const string KitPackagesRootDirectory = ".KSwordKit/Packages";
        /// <summary>
        /// KSwordKit�����µ�ַ
        /// </summary>
        public const string KitCheckForUpdates = "https://gitee.com/keenlovelife/ksword-kit/raw/kkp/.KitConfig.json";
        /// <summary>
        /// KSwordKit ��Ŀ¼��Դ����·��
        /// </summary>
        public const string KitOriginPackagesURL = "https://gitee.com/keenlovelife/ksword-kit/raw/kkp/.KSwordKit/kkp";
        /// <summary>
        /// KSwordKit��ܸ���ʱʹ�õ�URLǰ׺
        /// </summary>
        public const string KitUpdateURLPrefix = "https://github.com/keenlovelife/KSwordKit/releases/tag";
    }
}
