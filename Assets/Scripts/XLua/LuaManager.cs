using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XLua;

public class LuaManager : MonoBehaviour
{
    [SerializeField] private AssetReference luaAsset;
    [SerializeField] private TextMeshProUGUI damageText;

    private LuaEnv _luaEnv;

    private string _luaScript;

    [CSharpCallLua]
    public delegate float GetDamageFunc();

    private GetDamageFunc _damageFunc;

    private void Awake() {
        _luaEnv = new LuaEnv();
    }

    private void Start() {
        luaAsset.LoadAssetAsync<TextAsset>().Completed += handle => {
            print("Load Lua Done!");
            _luaScript = handle.Result.text;

            _luaEnv.DoString(_luaScript);

            _damageFunc = _luaEnv.Global.Get<GetDamageFunc>("getDamage");
        };
    }

    private void Update() {
        if (_damageFunc != null && Input.GetKeyDown(KeyCode.F)) {
            var damage = _damageFunc();
            damageText.text = $"Damaged {damage}";
        }
    }

}
