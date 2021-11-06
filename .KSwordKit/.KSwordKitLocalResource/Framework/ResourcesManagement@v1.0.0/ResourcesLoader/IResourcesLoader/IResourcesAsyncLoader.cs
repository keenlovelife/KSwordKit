/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: IResourcesASyncLoader.cs
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
	/// 异步加载资源的几个接口
	/// </summary>
	public interface IResourcesAsyncLoader
	{
		/// <summary>
		/// 根据 assetPath 异步加载资源
		/// <para>参数 asyncAction 是加载资源的异步回调；查看<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">资源路径</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容； </param>
		void LoadAsync(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// 给定的路径（文件夹或文件）中加载所有资源
		/// <para>如果想递归目录内所有资源可以使用<seealso cref="LoadAllAsync(string, bool, System.Action{ResourcesRequestAsyncOperation})"/></para>
		/// <para>参数 asyncAction 是加载资源的异步回调；查看<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">给定的资源路径（文件夹或文件）</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容；</param>
		void LoadAllAsync(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// 给定的路径（文件夹或文件）中加载所有资源
		/// <para>参数 deepResources 指示函数是否递归遍历子目录内的资源，当值为false时，函数行为和<seealso cref="LoadAllAsync(string, System.Action{ResourcesRequestAsyncOperation})"/>相同</para>
		/// <para>参数 asyncAction 是加载资源的异步回调；查看<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">给定的资源路径（文件夹或文件）</param>
		/// <param name="deepResources">指示函数是否遍历子目录所有资源</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容；</param>
		void LoadAllAsync(string assetPath, bool deepResources, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// 根据指定路径加载指定类型T的资源
		/// <para>参数 asyncAction 是加载资源的异步回调；查看<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <typeparam name="T">标识要加载资源的类型</typeparam>
		/// <param name="assetPath">资源的路径</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容；</param>
		void LoadAsync<T>(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
		/// <summary>
		/// 给定的路径（文件夹或文件）中加载所有资源
		/// <para>默认不会递归遍历子目录内资源</para>
		/// <para>如果想递归目录内所有资源可以使用<seealso cref="LoadAllAsync{T}(string, bool, System.Action{ResourcesRequestAsyncOperation})"/></para>
		/// <para>参数 asyncAction 是异步回调；查看<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <param name="assetPath">给定的资源路径（文件夹或文件）</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容；</param>
		void LoadAllAsync<T>(string asstPath, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
		/// <summary>
		/// 给定的路径（文件夹或文件）中加载所有资源
		/// <para>当参数 deepResources 值为 false时，函数行为和<seealso cref="LoadAllAsync{T}(string, System.Action{ResourcesRequestAsyncOperation})"/>相同</para>
		/// <para>参数 asyncAction 是异步回调；查看<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <param name="asstPath">给定的资源路径（文件夹或文件）</param>
		/// <param name="deepResources">指示函数是否遍历子目录所有资源</param>
		/// <param name="asyncAction">异步回调；参数是异步结果，内部包含进度、错误信息、加载结果等内容；</param>
		void LoadAllAsync<T>(string asstPath, bool deepResources, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
	}
}

