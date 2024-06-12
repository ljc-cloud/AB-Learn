using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class ABMgr : MonoBehaviour {
    public static ABMgr Instance { get; private set; }


    private void Awake() {
        Instance = this;
        _abDictionary = new Dictionary<string, AssetBundle>();
    }

    private string Path => $"{Application.streamingAssetsPath}/";

    private string MainAbName {
        get {
#if UNITY_IOS
return "IOS";
#elif UNITY_ANDROID
return "Android";
#else
            return "PC";
#endif
        }
    }


    private Dictionary<string, AssetBundle> _abDictionary;
    private AssetBundle _mainAb;
    private AssetBundleManifest _mainAbManifest;

    public void LoadAb(string abName) {
        if (_mainAb == null) {
            Debug.Log("Fetching Mainfest...");
            // 通过主包获取依赖信息
            _mainAb = AssetBundle.LoadFromFile(Path + MainAbName);
            // 加载主包的配置文件
            _mainAbManifest = _mainAb.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
        }
        // 加载依赖包
        AssetBundle ab;
        // 从配置文件中获取所有的依赖信息
        var allDependencies = _mainAbManifest.GetAllDependencies(abName);
        // 加载每个依赖AB包
        foreach (var dep in allDependencies) {
            if (!_abDictionary.ContainsKey(dep)) {
                Debug.Log("Loading AB Dependencies");
                ab = AssetBundle.LoadFromFile(Path + dep);
                _abDictionary.Add(dep, ab);
            }
        }
        // 加载资源包
        if (!_abDictionary.ContainsKey(abName)) {
            Debug.Log("Loading... AB");
            ab = AssetBundle.LoadFromFile(Path + abName);
            _abDictionary.Add(abName, ab);
        }
    }

    public void LoadAbAsync(string abName) {
        StartCoroutine(ToLoadAbAsync(abName));
    }

    private IEnumerator ToLoadAbAsync(string abName) {
        if (_mainAb == null) {
            // 通过主包获取依赖信息
            var abcr = AssetBundle.LoadFromFileAsync(Path + MainAbName);
            yield return abcr;
            _mainAb = abcr.assetBundle;
            // 加载主包的配置文件
            var abr = _mainAb.LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest));
            yield return abr;
            _mainAbManifest = abr.asset as AssetBundleManifest;
        }
        // 加载依赖包
        AssetBundle ab;
        // 从配置文件中获取所有的依赖信息
        var allDependencies = _mainAbManifest.GetAllDependencies(abName);
        foreach (var dep in allDependencies) {
            if (!_abDictionary.ContainsKey(dep)) {
                var abcr = AssetBundle.LoadFromFileAsync(Path + dep);
                yield return abcr;
                _abDictionary.Add(dep, abcr.assetBundle);
            }
        }
        // 加载资源包
        if (!_abDictionary.ContainsKey(abName)) {
            var abcr = AssetBundle.LoadFromFileAsync(Path + abName);
            yield return abcr;
            _abDictionary.Add(abName, abcr.assetBundle);
        }
    }

    // 同步加载
    public Object LoadAsset(string abName, string resName) {
        // 加载AB包
        // 获取AB包的依赖信息
        // 加载主包
        // 获取主包的配置文件，获取依赖信息
        LoadAb(abName);
        // 加载资源
        return _abDictionary[abName].LoadAsset(resName);
    }

    public Object LoadAsset(string abName, string resName, Type type) {
        // 加载AB包
        // 获取AB包的依赖信息
        // 加载主包
        // 获取主包的配置文件，获取依赖信息
        LoadAb(abName);
        // 加载资源
        return _abDictionary[abName].LoadAsset(resName, type);
    }

    public T LoadAsset<T>(string abName, string resName) where T : Object {
        // 加载AB包
        // 获取AB包的依赖信息
        // 加载主包
        // 获取主包的配置文件，获取依赖信息
        LoadAb(abName);
        // 加载资源
        return _abDictionary[abName].LoadAsset<T>(resName);
    }

    // 异步加载
    public void LoadAssetAsync(string abName, string resName, UnityAction<Object> callback) {
        StartCoroutine(ToLoadAssetAsync(abName, resName, callback));
    }

    private IEnumerator ToLoadAssetAsync(string abName, string resName, UnityAction<Object> callback) {
        LoadAb(abName);
        var abr = _abDictionary[abName].LoadAssetAsync(resName);
        yield return abr;
        callback(abr.asset);
    }

    public void LoadAssetAsync(string abName, string resName, Type type, UnityAction<Object> callback) {
        StartCoroutine(ToLoadAssetAsync(abName, resName, type, callback));
    }

    private IEnumerator ToLoadAssetAsync(string abName, string resName, Type type, UnityAction<Object> callback) {
        LoadAb(abName);
        var abr = _abDictionary[abName].LoadAssetAsync(resName, type);
        yield return abr;
        callback(abr.asset);
    }

    public void LoadAssetAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object {
        StartCoroutine(ToLoadAssetAsync(abName, resName, callback));
    }

    private IEnumerator ToLoadAssetAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object {
        LoadAb(abName);
        var abr = _abDictionary[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        callback(abr.asset as T);
    }



    // 卸载单个AB包
    public bool UnLoadAb(string abName) {
        if (_abDictionary.ContainsKey(abName)) {
            _abDictionary[abName].Unload(false);
            _abDictionary.Remove(abName);
            return true;
        }

        return false;
    }

    // 卸载所有AB包
    public void UnloadAllAb() {
        AssetBundle.UnloadAllAssetBundles(false);
        _abDictionary.Clear();
        _mainAb = null;
        _mainAbManifest = null;
    }
}
