using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private Transform[] fruitGrowPoints;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private float growTime;
    private float _growth;
    private MaterialPropertyBlock _mpb;
    private bool grownFruit;

    private void UpdateGrowMat() {
        float growT = _growth / growTime;
        _mpb.SetFloat("_GrowT", growT);
        mesh.SetPropertyBlock(_mpb);
    }
    
    private void Start() {
        _mpb = new MaterialPropertyBlock();
        UpdateGrowMat();
    }

    private void Update() {
        if (_growth < growTime) {
            _growth += Time.deltaTime;

            if (_growth > growTime) _growth = growTime;
        } else {
            if (!grownFruit) {
                var randomPoint = fruitGrowPoints[Random.Range(0,fruitGrowPoints.Length)].position;
                Instantiate(fruitPrefab, randomPoint, Quaternion.identity);
                grownFruit = true;
            }
        }

        float growT = _growth / growTime;
        UpdateGrowMat();
        
        debugText.text = "growT = " + growT;
        //mesh.transform.localScale = new Vector3(1,1, Mathf.Clamp(growT, 0.1f, 1.0f));
    }
}
