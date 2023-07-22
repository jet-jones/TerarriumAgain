using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lamp : MonoBehaviour {
    [SerializeField] private Light[] lights;
    [SerializeField] private Dial dial;
    private float[] _initialIntensity;
    
    private void Start() {
        _initialIntensity = new float[lights.Length];
        for (var i = 0; i < lights.Length; i++) {
            _initialIntensity[i] = lights[i].intensity;
        }
    }
    
    private void Update() {
        var dialT = dial.value / dial.maxValue;
        for (var i = 0; i < lights.Length; i++) {
            lights[i].intensity = Mathf.Lerp(0, _initialIntensity[i], dialT);
        }
    }
}