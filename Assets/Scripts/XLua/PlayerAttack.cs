using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    // ��ͨ�����˺�
    [SerializeField] private float atk;
    // ħ���˺�
    [SerializeField] private float matk;

    public static PlayerAttack Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public float Atk {
        get => atk;
        set {
            atk = value;
        }
    }
    public float Matk {
        get => matk;
        set {
            matk = value;
        }
    }

}
