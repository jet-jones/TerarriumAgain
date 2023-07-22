using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private int minSupportedFruits = 1;
    [SerializeField] private int maxSupportedFruits = 4;

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
            int validFruitCount = 0;
            foreach (var fruit in grownFruit) {
                if (fruit) validFruitCount++;
            }

            if (validFruitCount < fruitGrowPoints.Length)
            {
                int chance = (validFruitCount + 1) * 100;
                bool canGrow = Random.Range(0, chance) == 0;

                if (canGrow) {
                    var randomPoint = fruitGrowPoints[Random.Range(0,fruitGrowPoints.Length)].position;
                    //for () {
                        
                    //}
                    
                    var newFruit = Instantiate(fruitPrefab, randomPoint, Quaternion.identity).GetComponent<Fruit>();
                    newFruit.sourcePlant = this;
                
                    //grownFruit.Add(newFruit);
                }
            }
        }

        float growT = _growth / growTime;
        UpdateGrowMat();
        
        debugText.text = "growT = " + growT;
        //mesh.transform.localScale = new Vector3(1,1, Mathf.Clamp(growT, 0.1f, 1.0f));
    }
}
