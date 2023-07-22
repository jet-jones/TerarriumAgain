using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private int nextFruitChanceMultiplier = 200;

    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private Transform[] fruitGrowPoints;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private float growTime;

    private float _growth;
    private MaterialPropertyBlock _mpb;
    public Fruit[] grownFruit;

    private void UpdateGrowMat() {
        float growT = _growth / growTime;
        _mpb.SetFloat("_GrowT", growT);
        mesh.SetPropertyBlock(_mpb);
    }
    
    private void Start() {
        grownFruit = new Fruit[fruitGrowPoints.Length];
        _mpb = new MaterialPropertyBlock();
        UpdateGrowMat();
    }

    private void Update() {
        if (_growth < growTime) {
            _growth += Time.deltaTime;

            if (_growth > growTime) _growth = growTime;
        } else {
            var fruitCount = 0;
            foreach (var fruit in grownFruit) {
                if (fruit) fruitCount++;
            }

            if (fruitCount < fruitGrowPoints.Length) {
                var chance = (fruitCount + 1) * nextFruitChanceMultiplier;
                var canGrow = Random.Range(0, chance) == 0;

                if (canGrow) {
                    var growPointIndex = -1;
                    for (var i = 0; i < grownFruit.Length; i++) {
                        if (grownFruit[i] == null) {
                            growPointIndex = i;
                            break;
                        }
                    }
                    
                    var randomPoint = fruitGrowPoints[growPointIndex].position;
                    if (growPointIndex < 0) Debug.Log("somethings fucked");
                    
                    var newFruit = Instantiate(fruitPrefab, randomPoint, Quaternion.identity).GetComponent<Fruit>();
                    newFruit.sourcePlant = this;
                    newFruit.growPointIndex = growPointIndex;

                    grownFruit[growPointIndex] = newFruit;
                }
            }
        }

        float growT = _growth / growTime;
        UpdateGrowMat();
        
        debugText.text = "growT = " + growT;
        //mesh.transform.localScale = new Vector3(1,1, Mathf.Clamp(growT, 0.1f, 1.0f));
    }
}
