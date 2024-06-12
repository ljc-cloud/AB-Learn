using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ABTest : MonoBehaviour
{
    [SerializeField] private Image image;
    // Start is called before the first frame update
    void Start()
    {
        var go1 = ABMgr.Instance.LoadAsset("model", "Cube") as GameObject;
        var go2 = ABMgr.Instance.LoadAsset("model", "Cube", typeof(GameObject)) as GameObject;
        var go3 = ABMgr.Instance.LoadAsset<GameObject>("model", "Cube");
        ABMgr.Instance.LoadAssetAsync("model", "Cube", obj =>
        {
            Instantiate(obj);
        });
        ABMgr.Instance.LoadAssetAsync("model", "Cube", typeof(GameObject),obj =>
        {
            Instantiate(obj);
        });
        ABMgr.Instance.LoadAssetAsync<GameObject>("model", "Cube", obj =>
        {
            Instantiate(obj);
        });
        Instantiate(go1);
        Instantiate(go2);
        Instantiate(go3);
        // AB�������ظ�����
        //var abFile2 = AssetBundle.LoadFromFile($"{Application.streamingAssetsPath}/model");
        // Э�� �첽������Դ
        //StartCoroutine(LoadAssetAsync("switch", "btn_switch_auto"));

        // ж�����м��ص�AB��
        //AssetBundle.UnloadAllAssetBundles(false);
    }

    IEnumerator LoadAssetAsync(string abName, string resName)
    {
        var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync($"{Application.streamingAssetsPath}/{abName}");
        yield return assetBundleCreateRequest;
        var assetBundleRequest = assetBundleCreateRequest.assetBundle.LoadAssetAsync(resName, typeof(Sprite));
        yield return assetBundleRequest;
        image.sprite = assetBundleRequest.asset as Sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }
    }
}
