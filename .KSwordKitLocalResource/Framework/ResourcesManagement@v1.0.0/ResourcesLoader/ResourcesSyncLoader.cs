/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ResourcesSyncLoader.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-11
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit.Contents.ResourcesManagement
{
	/// <summary>
	/// 同步加载资源加载器
	/// </summary>
	public class ResourcesSyncLoader : IResourcesSyncLoader
	{
		const string KSwordKitName = "KSwordKit";

		/// <summary>
		/// 构造函数
		/// <para>参数 resourcesLoadingLocation 的不同将指示实例对象使用不同的加载方式加载资源; 默认值为 Resources </para>
		/// <para>异常：参数为 Remote 和 StreamingAssets 时，因不支持同步加载资源而抛出异常 。</para>
		/// </summary>
		/// <param name="resourcesLoadingLocation">指示函数加载的位置</param>
		public ResourcesSyncLoader(ResourcesLoadingLocation resourcesLoadingLocation = ResourcesLoadingLocation.Resources)
		{
			switch(resourcesLoadingLocation)
            {
				case ResourcesLoadingLocation.RemotePath:
				case ResourcesLoadingLocation.StreamingAssetsPath:
					var error = KSwordKitName + ": 同步资源加载不能加载 " + resourcesLoadingLocation + " 位置的资源! 请替换 ResourcesAsyncLoader 类进行加载";
					throw new System.ArgumentException(error);
            }
			_resourcesLoadingLocation = resourcesLoadingLocation;
        }

		/// <summary>
		/// 内部使用的错误信息
		/// </summary>
		string _error;

		private ResourcesLoadingLocation _resourcesLoadingLocation;
		/// <summary>
		/// 资源加载的位置
		/// <para>该值在构造函数中传入</para>
		/// </summary>
		public ResourcesLoadingLocation ResourcesLoadingLocation { get { return _resourcesLoadingLocation; } }
		/// <summary>
		/// 根据 assetPath 同步加载资源
		/// </summary>
		/// <param name="assetPath">资源路径（文件）</param>
		/// <returns>返回有效资源； 加载成功，返回资源对象； 如果加载出错，返回 null</returns>
		public UnityEngine.Object Load(string assetPath)
        {
			switch(ResourcesLoadingLocation)
            {
				case ResourcesLoadingLocation.Resources:
					return Resources.Load(assetPath);
				case ResourcesLoadingLocation.PersistentDataPath:
					break;
			}
			return null;
        }

		UnityEngine.Object LoadByResources(string assetPath, out string error)
        {
			var asset = Resources.Load(assetPath);
			if (asset == null)
				error = "加载失败或者文件不存在；assetPath = " + assetPath;
			else
				error = null;
			return asset;
        }

		/// <summary>
		/// 根据 assetPath 同步加载资源
		/// </summary>
		/// <param name="assetPath">资源路径（文件）</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源； 加载成功，返回资源对象； 如果加载出错，返回 null</returns>
		public UnityEngine.Object Load(string assetPath, out string error)
        {
			switch (ResourcesLoadingLocation)
			{
				case ResourcesLoadingLocation.Resources:
					return LoadByResources(assetPath, out error);
			}

			error = "加载失败或者文件不存在；assetPath = " + assetPath;
			return null;
		}
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		public UnityEngine.Object[] LoadAll(string assetPath)
		{
			return null;
		}
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		public UnityEngine.Object[] LoadAll(string assetPath, out string error)
		{
			error = null;
			return null;
		}
		/// <summary>
		/// 根据指定路径同步加载指定类型T的资源
		/// </summary>
		/// <typeparam name="T">标识要加载资源的类型</typeparam>
		/// <param name="assetPath">资源的路径</param>
		/// <returns>返回有效资源；加载成功，则返回T类型的资源；加载失败，则返回null</returns>
		public T Load<T>(string assetPath) where T : UnityEngine.Object
		{

			return null;
		}
		/// <summary>
		/// 根据指定路径同步加载指定类型T的资源
		/// </summary>
		/// <typeparam name="T">标识要加载资源的类型</typeparam>
		/// <param name="assetPath">资源的路径</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源；加载成功，则返回T类型的资源；加载失败，则返回null</returns>
		public T Load<T>(string assetPath, out string error) where T : UnityEngine.Object
		{
			error = null;

			return null;
		}
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		public T[] LoadAll<T>(string asstPath) where T : UnityEngine.Object
		{
			return null;
		}
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <typeparam name="T">将加载资源的类型</typeparam>
		/// <param name="assetPath">资源路径</param>
		/// <param name="error">加载出错时，输出错误信息</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		public T[] LoadAll<T>(string assetPath, out string error) where T : UnityEngine.Object
		{
			error = null;

			return null;
		}
	}
}
