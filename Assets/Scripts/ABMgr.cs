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
            // ͨ��������ȡ������Ϣ
            _mainAb = AssetBundle.LoadFromFile(Path + MainAbName);
            // ���������������ļ�
            _mainAbManifest = _mainAb.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
        }
        // ����������
        AssetBundle ab;
        // �������ļ��л�ȡ���е�������Ϣ
        var allDependencies = _mainAbManifest.GetAllDependencies(abName);
        // ����ÿ������AB��
        foreach (var dep in allDependencies) {
            if (!_abDictionary.ContainsKey(dep)) {
                Debug.Log("Loading AB Dependencies");
                ab = AssetBundle.LoadFromFile(Path + dep);
                _abDictionary.Add(dep, ab);
            }
        }
        // ������Դ��
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
            // ͨ��������ȡ������Ϣ
            var abcr = AssetBundle.LoadFromFileAsync(Path + MainAbName);
            yield return abcr;
            _mainAb = abcr.assetBundle;
            // ���������������ļ�
            var abr = _mainAb.LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest));
            yield return abr;
            _mainAbManifest = abr.asset as AssetBundleManifest;
        }
        // ����������
        AssetBundle ab;
        // �������ļ��л�ȡ���е�������Ϣ
        var allDependencies = _mainAbManifest.GetAllDependencies(abName);
        foreach (var dep in allDependencies) {
            if (!_abDictionary.ContainsKey(dep)) {
                var abcr = AssetBundle.LoadFromFileAsync(Path + dep);
                yield return abcr;
                _abDictionary.Add(dep, abcr.assetBundle);
            }
        }
        // ������Դ��
        if (!_abDictionary.ContainsKey(abName)) {
            var abcr = AssetBundle.LoadFromFileAsync(Path + abName);
            yield return abcr;
            _abDictionary.Add(abName, abcr.assetBundle);
        }
    }

    // ͬ������
    public Object LoadAsset(string abName, string resName) {
        // ����AB��
        // ��ȡAB����������Ϣ
        // ��������
        // ��ȡ�����������ļ�����ȡ������Ϣ
        LoadAb(abName);
        // ������Դ
        return _abDictionary[abName].LoadAsset(resName);
    }

    public Object LoadAsset(string abName, string resName, Type type) {
        // ����AB��
        // ��ȡAB����������Ϣ
        // ��������
        // ��ȡ�����������ļ�����ȡ������Ϣ
        LoadAb(abName);
        // ������Դ
        return _abDictionary[abName].LoadAsset(resName, type);
    }

    public T LoadAsset<T>(string abName, string resName) where T : Object {
        // ����AB��
        // ��ȡAB����������Ϣ
        // ��������
        // ��ȡ�����������ļ�����ȡ������Ϣ
        LoadAb(abName);
        // ������Դ
        return _abDictionary[abName].LoadAsset<T>(resName);
    }

    // �첽����
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



    // ж�ص���AB��
    public bool UnLoadAb(string abName) {
        if (_abDictionary.ContainsKey(abName)) {
            _abDictionary[abName].Unload(false);
            _abDictionary.Remove(abName);
            return true;
        }

        return false;
    }

    // ж������AB��
    public void UnloadAllAb() {
        AssetBundle.UnloadAllAssetBundles(false);
        _abDictionary.Clear();
        _mainAb = null;
        _mainAbManifest = null;
    }
}
