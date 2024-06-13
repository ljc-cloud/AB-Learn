using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterSpriteInstantiate : MonoBehaviour {
    [SerializeField] private AssetReference imageAsset;


    private void Start() {
        var asyncOperation = imageAsset.InstantiateAsync();
        asyncOperation.Completed += (handle) => Debug.Log("Done!");

    }
}
