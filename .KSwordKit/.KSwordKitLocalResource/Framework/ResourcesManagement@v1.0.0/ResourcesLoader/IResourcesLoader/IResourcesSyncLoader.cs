/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: IResourcesSyncLoader.cs
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
	/// 定义同步加载资源的几个接口
	/// </summary>
	public interface IResourcesSyncLoader
	{
		/// <summary>
		/// 根据 assetPath 同步加载资源
		/// </summary>
		/// <param name="assetPath">资源路径（文件）</param>
		/// <returns>返回有效资源； 加载成功，返回资源对象； 如果加载出错，返回 null</returns>
		UnityEngine.Object Load(string assetPath);
		/// <summary>
		/// 根据 assetPath 同步加载资源
		/// </summary>
		/// <param name="assetPath">资源路径（文件）</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源； 加载成功，返回资源对象； 如果加载出错，返回 null</returns>
		UnityEngine.Object Load(string assetPath, out string error);
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		UnityEngine.Object[] LoadAll(string assetPath);
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		UnityEngine.Object[] LoadAll(string assetPath, out string error);
		/// <summary>
		/// 根据指定路径同步加载指定类型T的资源
		/// </summary>
		/// <typeparam name="T">标识要加载资源的类型</typeparam>
		/// <param name="assetPath">资源的路径</param>
		/// <returns>返回有效资源；加载成功，则返回T类型的资源；加载失败，则返回null</returns>
		T Load<T>(string assetPath) where T : UnityEngine.Object;
		/// <summary>
		/// 根据指定路径同步加载指定类型T的资源
		/// </summary>
		/// <typeparam name="T">标识要加载资源的类型</typeparam>
		/// <param name="assetPath">资源的路径</param>
		/// <param name="error">加载错误时，输出错误信息</param>
		/// <returns>返回有效资源；加载成功，则返回T类型的资源；加载失败，则返回null</returns>
		T Load<T>(string assetPath, out string error) where T : UnityEngine.Object;
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <param name="assetPaths">给定的资源路径（文件夹或文件）</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		T[] LoadAll<T>(string asstPath) where T : UnityEngine.Object;
		/// <summary>
		/// 给定的路径（文件夹或文件）中同步加载所有资源
		/// </summary>
		/// <typeparam name="T">将加载资源的类型</typeparam>
		/// <param name="assetPath">资源路径</param>
		/// <param name="error">加载出错时，输出错误信息</param>
		/// <returns>返回有效资源; 如果全部加载失败，则返回空数组</returns>
		T[] LoadAll<T>(string assetPath, out string error) where T : UnityEngine.Object;
	}
}
