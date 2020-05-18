using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetTemp : MonoBehaviour {

    [SerializeField]
    Text mKey;
    [SerializeField]
    Text mValue;
    // Use this for initialization
    void Start () {
        transform.localScale = Vector3.one;
	}

    public void SetData(string key, string value)
    {
        mKey.text = key;
        mValue.text = value;
    }
}
