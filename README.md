

# AB-Learn
AB包、XLua热更新学习


### Scene1：AssetBundle

1. 测试使用AssetBundle加载资源
2. 实现ABMgr脚本，同步加载资源和异步加载资源
##### 重要
1. 加载AB包首先需要加载这个AB包所依赖的其他AB包，因此需要加载主包manifest，获取依赖信息
``` C#
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
        // 缓存依赖包
        _abDictionary.Add(dep, ab);
    }
}
```
3. 可以使用字典缓存AB包，后面需要加载这个AB包时，直接使用缓存
```C#
private Dictionary<string, AssetBundle> _abDictionary;
```
