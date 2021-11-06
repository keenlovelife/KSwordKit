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
	/// �첽������Դ�ļ����ӿ�
	/// </summary>
	public interface IResourcesAsyncLoader
	{
		/// <summary>
		/// ���� assetPath �첽������Դ
		/// <para>���� asyncAction �Ǽ�����Դ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">��Դ·��</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ� </param>
		void LoadAsync(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// ������·�����ļ��л��ļ����м���������Դ
		/// <para>�����ݹ�Ŀ¼��������Դ����ʹ��<seealso cref="LoadAllAsync(string, bool, System.Action{ResourcesRequestAsyncOperation})"/></para>
		/// <para>���� asyncAction �Ǽ�����Դ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">��������Դ·�����ļ��л��ļ���</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ�</param>
		void LoadAllAsync(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// ������·�����ļ��л��ļ����м���������Դ
		/// <para>���� deepResources ָʾ�����Ƿ�ݹ������Ŀ¼�ڵ���Դ����ֵΪfalseʱ��������Ϊ��<seealso cref="LoadAllAsync(string, System.Action{ResourcesRequestAsyncOperation})"/>��ͬ</para>
		/// <para>���� asyncAction �Ǽ�����Դ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation"/></para>
		/// </summary>
		/// <param name="assetPath">��������Դ·�����ļ��л��ļ���</param>
		/// <param name="deepResources">ָʾ�����Ƿ������Ŀ¼������Դ</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ�</param>
		void LoadAllAsync(string assetPath, bool deepResources, System.Action<ResourcesRequestAsyncOperation> asyncAction);
		/// <summary>
		/// ����ָ��·������ָ������T����Դ
		/// <para>���� asyncAction �Ǽ�����Դ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <typeparam name="T">��ʶҪ������Դ������</typeparam>
		/// <param name="assetPath">��Դ��·��</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ�</param>
		void LoadAsync<T>(string assetPath, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
		/// <summary>
		/// ������·�����ļ��л��ļ����м���������Դ
		/// <para>Ĭ�ϲ���ݹ������Ŀ¼����Դ</para>
		/// <para>�����ݹ�Ŀ¼��������Դ����ʹ��<seealso cref="LoadAllAsync{T}(string, bool, System.Action{ResourcesRequestAsyncOperation})"/></para>
		/// <para>���� asyncAction ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <param name="assetPath">��������Դ·�����ļ��л��ļ���</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ�</param>
		void LoadAllAsync<T>(string asstPath, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
		/// <summary>
		/// ������·�����ļ��л��ļ����м���������Դ
		/// <para>������ deepResources ֵΪ falseʱ��������Ϊ��<seealso cref="LoadAllAsync{T}(string, System.Action{ResourcesRequestAsyncOperation})"/>��ͬ</para>
		/// <para>���� asyncAction ���첽�ص����鿴<see cref="ResourcesRequestAsyncOperation{T}"/></para>
		/// </summary>
		/// <param name="asstPath">��������Դ·�����ļ��л��ļ���</param>
		/// <param name="deepResources">ָʾ�����Ƿ������Ŀ¼������Դ</param>
		/// <param name="asyncAction">�첽�ص����������첽������ڲ��������ȡ�������Ϣ�����ؽ�������ݣ�</param>
		void LoadAllAsync<T>(string asstPath, bool deepResources, System.Action<ResourcesRequestAsyncOperation> asyncAction) where T : UnityEngine.Object;
	}
}

