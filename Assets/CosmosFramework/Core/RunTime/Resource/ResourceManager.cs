﻿//------------------------------------------------------------------------------
//      当前算一个早期版本，持续更新中。后期会做分类处理
//      当前资源管理需要精简，拆分功能
//在移动平台下，Application.streamingAssetsPath是只读的，不能写入数据。
//Application.persistentDataPath 可以读取和写入数据。

//在PC下，可以用File类API（如File.ReadAllText）读写StreamingAssets文件夹中的文件；
//在IOS和Android平台下，不能用File类API读取。

//所有平台上都可以用www方式异步读取StreamingAssets文件夹，
//PC和IOS平台下，读取路径必须加上"file://"，而安卓不需要。

//在IOS和Android下，还能用AssetBundle.LoadFromFile来同步读取数据。

//----
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Cosmos.Config;
using System.Collections;
using UnityEngine.Networking;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{

    internal enum ResourceLoadMode : byte
    {
        Resource = 0,
        AssetBundle = 1
    }
    internal sealed class ResourceManager : Module<ResourceManager>
    {
        #region Properties
        //缓存的所有AssetBundle包 <AB包名称、AB包>
        Dictionary<string, AssetBundle> assetBundleDict;
        //所有AssetBundle验证的Hash128值 <AB包名称、Hash128值>
        Dictionary<string, Hash128> assetBundleHashDict;
        //所有AssetBundle资源包清单
        AssetBundleManifest assetBundleManifest;
        string assetBundleManifestName;
        //AssetBundle资源加载根路径
        string assetBundleRootPath;
        bool isLoading = false;
        #endregion

        #region Methods
        #region  Resources 
        public override void OnInitialization()
        {
            assetBundleDict = new Dictionary<string, AssetBundle>();
            assetBundleHashDict = new Dictionary<string, Hash128>();
        }
        /// <summary>
        /// 同步加载资源，若可选参数为true，则返回实例化后的对象，否则只返回资源对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">相对Resource路径</param>
        /// <param name="instantiate">是否实例化GameObject类型</param>
        /// <returns></returns>
        internal T LoadResAsset<T>(string path, bool instantiate = false)
            where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(path);
            if (res == null)
                throw new ArgumentNullException("ResourceManager-- >> " + "Assets: " + path + " not exist, check your path!");
            if (instantiate)
            {
                if (res is GameObject)
                {
                    var result = GameObject.Instantiate(res);
                    return result;
                }
            }
            return res;
        }
        /// <summary>
        /// 利用挂载特性的泛型对象同步加载Prefab；
        /// </summary>
        /// <typeparam name="T">需要加载的类型</typeparam>
        /// <param name="instantiate">是否生实例化对象</param>
        /// <returns>返回实体化或未实例化的资源对象</returns>
        internal GameObject LoadResPrefab<T>(bool instantiate )
            where T : MonoBehaviour
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            GameObject prefab = default;
            if (attribute != null)
            {
                prefab = Resources.Load<GameObject>(attribute.ResourcePath);
                if (prefab == null)
                    throw new ArgumentNullException("ResourceManager-->>" + "Assets: " + attribute.ResourcePath + " not exist,check your path!");
                if (instantiate)
                    prefab = Utility.Unity.Instantiate<T>(prefab).gameObject;
            }
            return prefab;
        }
        internal T LoadResPrefab<T>()
    where T : MonoBehaviour
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            GameObject prefab = default;
            T Comp = default;
            if (attribute != null)
            {
                prefab = Resources.Load<GameObject>(attribute.ResourcePath);
                if (prefab == null)
                    throw new ArgumentNullException("ResourceManager-->>" + "Assets: " + attribute.ResourcePath + " not exist,check your path!");
                    Comp = Utility.Unity.Instantiate<T>(prefab);
            }
            return Comp;
        }
        /// <summary>
        /// 利用挂载特性的泛型对象同步加载PrefabObject；
        /// </summary>
        /// <typeparam name="T">实现了引用池的非Mono对象</typeparam>
        /// <param name="go">载入的资源对象</param>
        /// <param name="instantiate">是否实例化</param>
        /// <returns>载入的对象</returns>
        internal GameObject LoadResPrefabInstance<T>(bool instantiate = false)
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
           GameObject  go = default;
            T referObj = default;
            if (attribute != null)
            {
                go = Resources.Load<GameObject>(attribute.ResourcePath);
                if (go == null)
                    throw new ArgumentNullException("ResourceManager-->>" + "Assets: " + attribute.ResourcePath + " not exist,check your path!");
                referObj = Facade.SpawnReference<T>();
                if (instantiate)
                    go = GameObject.Instantiate(go);
            }
            return go;
        }
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="callBack">载入完毕后的回调</param>
        internal void LoadResPrefabAsync<T>(Action<T> callBack = null)
            where T : MonoBehaviour
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute != null)
                Facade.StartCoroutine(EnumLoadResPrefabAsync<T>(attribute.ResourcePath, callBack));
        }
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// 泛型对象为Mono类型；
        /// </summary>
        /// <typeparam name="T">非Mono对象</typeparam>
        /// <param name="callBack">加载完毕后的回调</param>
        internal void LoadResPrefabInstanceAsync<T>(Action<T,GameObject> callBack = null)
            where T : class,IReference, new()
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute != null)
                Facade.StartCoroutine(EnumLoadResPrefabInstanceAsync<T>(attribute.ResourcePath, callBack));
        }
        internal T LoadResAsset<T>(bool instantiateGameObject = false)
            where T : UnityEngine.Component
        {
            Type type = typeof(T);
            ResourceUnitAttribute attribute = type.GetCustomAttribute<ResourceUnitAttribute>();
            T res = default;
            if (attribute != null)
            {
                var go = Resources.Load<GameObject>(attribute.ResourcePath);
                if (go == null)
                {
                    Utility.DebugError("ResourceManager-->>" + "Assets: " + attribute.ResourcePath + " not exist,check your path!");
                    return null;
                }
                if (instantiateGameObject)
                {
                    if (go is GameObject)
                    {
                        var result = GameObject.Instantiate(go);
                        res = result.AddComponent<T>();
                        return res;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        internal void LoadResAysnc<T>(string path, Action<T> callBack = null)
            where T : UnityEngine.Object
        {
            Facade.StartCoroutine(EnumLoadResAsync(path, callBack));
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        internal void LoadResAssetAysnc<T>(string path, Action<T> callBack = null)
            where T : UnityEngine.Object
        {
            Facade.StartCoroutine(EnumLoadResAssetAsync(path, callBack));
        }
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        internal List<T> LoadResFolderAssets<T>(string path)
       where T : class
        {
            List<T> list = new List<T>();
            var resList = Resources.LoadAll(path, typeof(T)) as T[];
            for (int i = 0; i < resList.Length; i++)
            {
                list.Add(resList[i]);
            }
            return list;
        }
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        internal T[] LoadResAll<T>(string path)
            where T : UnityEngine.Object
        {
            T[] res = Resources.LoadAll<T>(path);
            if (res == null)
            {
                Utility.DebugError("ResourceManager-->>" + "Assets: " + path + "  not exist,check your path!");
                return null;
            }
            if (res is GameObject)
            {
                for (int i = 0; i < res.Length; i++)
                    GameObject.Instantiate(res[i]);
            }
            return res;
        }
        #endregion
        #region  AssetBundles
        //TODO ResourceManager AB资源的特性加载
        internal void SetManifestName(string name)
        {
            this.assetBundleManifestName = name;
        }
        /// <summary>
        /// 从一个AB包中获得资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal T LoadABAsset<T>(string path, string name)
            where T : UnityEngine.Object
        {
            var ab = AssetBundle.LoadFromFile(path);
            var asset = ab.LoadAsset<T>(name);
            return asset;
        }
        /// <summary>
        /// 异步加载AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resUnit"></param>
        /// <param name="loadingCallBack"></param>
        /// <param name="loadDoneCallBack"></param>
        internal void LoadABAssetAsync<T>(ResourceUnit resUnit, Action<float> loadingCallBack, Action<T> loadDoneCallBack)
            where T : UnityEngine.Object
        {
            Facade.StartCoroutine(EnumLoadABAssetAsync(resUnit, loadingCallBack, loadDoneCallBack));
        }
        /// <summary>
        /// 异步加载AB依赖包
        /// </summary>
        /// <param name="abName"></param>
        internal void LoadDependenciesABAsync(string abName)
        {
            Facade.StartCoroutine(EnumLoadDependenciesABAsyn(abName));
        }
        /// <summary>
        /// 异步加载AB包，若不存在，则从web端加载
        /// </summary>
        /// <param name="abName">AssetBundle Name</param>
        /// <param name="isManifest">是否为AB清单</param>
        internal void LoadABAsync(string abName, bool isManifest = false)
        {
            Facade.StartCoroutine(EnumLoadABAsync(abName, isManifest));
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        internal void LoadABManifestAsync()
        {
            Facade.StartCoroutine(EnumLoadABManifestAsync());
        }
        internal void UnloadAsset(string abName, bool unloadAllAssets = false)
        {
            var ab = QueryAssetBundle(abName);
            if (ab != null)
            {
                ab.Unload(unloadAllAssets);
                assetBundleDict.Remove(abName);
            }
        }
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="unloadAllAssets">是否卸所有实体对象</param>
        internal void UnloadAllAsset(bool unloadAllAssets = false)
        {
            foreach (var ab in assetBundleDict)
            {
                ab.Value.Unload(unloadAllAssets);
            }
            assetBundleDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllAssets);
        }
        #endregion
        #region Resources Private
        IEnumerator EnumLoadResPrefabInstanceAsync<T>(string path, Action<T,GameObject> callBack)
            where T : class, IReference, new()
        {
            ResourceRequest req = Resources.LoadAsync<GameObject>(path);
            yield return req;
            var obj = Facade.SpawnReference<T>();
            callBack?.Invoke(obj,GameObject.Instantiate(req.asset) as GameObject);
        }
        IEnumerator EnumLoadResPrefabAsync<T>(string path, Action<T> callBack)
            where T : MonoBehaviour
        {
            ResourceRequest req = Resources.LoadAsync<GameObject>(path);
            yield return req;
            var go = Utility.Unity.Instantiate<T>(req.asset as GameObject);
            callBack?.Invoke(go);
        }
        IEnumerator EnumLoadResAssetAsync<T>(string path, Action<T> callBack = null)
            where T : UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(path);
            yield return req;
            callBack?.Invoke(req.asset as T);
        }
        IEnumerator EnumLoadResAsync<T>(string path, Action<T> callBack = null)
            where T : UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(path);
            yield return req;
            if (req.asset is GameObject)
                callBack?.Invoke(GameObject.Instantiate(req.asset) as T);
        }
        #endregion
        #region    AssetBundles Private
        IEnumerator EnumLoadABAssetAsync<T>(ResourceUnit resUnit, Action<float> loadingCallBack, Action<T> loadDoneCallBack)
    where T : UnityEngine.Object
        {
            //先加载依赖资源
            yield return Facade.StartCoroutine(EnumLoadDependenciesABAsyn(resUnit.AssetBundleName));

            var ab = QueryAssetBundle(resUnit.AssetBundleName);
            if (ab != null)
            {
                loadingCallBack?.Invoke(1);
                yield return null;
                UnityEngine.Object asset = ab.LoadAsset<T>(resUnit.AssetPath);
                loadDoneCallBack?.Invoke(asset as T);
            }
        }
        IEnumerator EnumLoadDependenciesABAsyn(string abName)
        {
            yield return Facade.StartCoroutine(EnumLoadABManifestAsync());
            if (assetBundleManifest)
            {
                string[] dependencies = assetBundleManifest.GetAllDependencies(abName);
                foreach (var dep in dependencies)
                {
                    if (HasAssetBundle(dep))
                        continue;
                    yield return Facade.StartCoroutine(EnumLoadABAsync(dep));
                }
            }
        }
        IEnumerator EnumLoadABAsync(string abName, bool isManifest = false)
        {
            if (!HasAssetBundle(abName))
            {
                using (UnityWebRequest request = isManifest ? UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + abName) :
                    UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + abName, GetABHash(abName)))
                {
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        //若网络通畅
                        var bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle != null)
                        {
                            assetBundleDict.Add(abName, bundle);
                        }
                        else
                        {
                            //TODO  resMgr异常
                            throw new ArgumentNullException("Requet :" + request.url + "have not assetBundle");
                        }
                    }
                    else
                    {
                        //TODO resMgr异常
                        throw new ArgumentNullException("Requet :" + request.url + " net work error ,check your netWork");
                    }
                }
            }
            yield return null;
        }
        IEnumerator EnumLoadABManifestAsync()
        {
            if (string.IsNullOrEmpty(assetBundleManifestName))
            {
                //TODO 协程异常 resMgr
                //throw new CFrameworkException("AssetBundle Manifest name empty , please reset abManifest !");
                Utility.Assert.NotNull(assetBundleManifestName);
            }
            else
            {
                if (assetBundleManifest == null)
                {
                    yield return Facade.StartCoroutine(EnumLoadABAsync(assetBundleManifestName, true));
                    if (HasAssetBundle(assetBundleManifestName))
                    {
                        assetBundleManifest = assetBundleDict[assetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnloadAsset(assetBundleManifestName);
                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 获得AB包的哈希值
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        Hash128 GetABHash(string abName)
        {
            if (HashAssetBundleHash(abName))
            {
                return assetBundleHashDict[abName];
            }
            else
            {
                Hash128 hash = assetBundleManifest.GetAssetBundleHash(abName);
                assetBundleHashDict.Add(abName, hash);
                return hash;
            }
        }
        /// <summary>
        /// 查询是否包含资源单位
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        AssetBundle QueryAssetBundle(string name)
        {
            if (assetBundleDict.ContainsKey(name))
                return assetBundleDict[name];
            return null;
        }
        bool HashAssetBundleHash(string abName)
        {
            return assetBundleHashDict.ContainsKey(abName);
        }
        bool HasAssetBundle(string name)
        {
            return assetBundleDict.ContainsKey(name);
        }
        #endregion
        #endregion
    }

}

