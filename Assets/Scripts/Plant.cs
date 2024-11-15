using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private int nextFruitChanceMultiplier = 200;
    [SerializeField] private int minFruitSpawns = 1;
    [SerializeField] private int maxFruitSpawns = 5;
    
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private float growTime;
    public AdvancedPlant plant;

    private float _growth;
    private MaterialPropertyBlock _mpb;
    public List<Fruit> grownFruit;
    private int _fruitSpawnCount;
    private int _growFruitIndex;

    private void UpdateGrowMat() {
        float growT = _growth / growTime;
        _mpb.SetFloat("_GrowT", growT);
        mesh.SetPropertyBlock(_mpb);
    }
    
    private void Start() {
        _fruitSpawnCount = Random.Range(minFruitSpawns, maxFruitSpawns);
        plant.Generate(Random.Range(0, 10000));
        grownFruit = new List<Fruit>();
        _mpb = new MaterialPropertyBlock();
        UpdateGrowMat();
    }

    private void Update() {
        plant.UpdateMesh();
        
        if (_growth < growTime) {
            _growth += Time.deltaTime;

            if (_growth > growTime) _growth = growTime;
        } else {
            if (grownFruit.Count < _fruitSpawnCount) {
                var chance = (grownFruit.Count + 1) * nextFruitChanceMultiplier;
                var canGrow = Random.Range(0, chance) == 0;

                if (canGrow) {
                    var advancedPlant = mesh.GetComponent<AdvancedPlant>();
                    
                    var newFruit = Instantiate(fruitPrefab, advancedPlant.GetGrowPoint(_growFruitIndex), Quaternion.identity).GetComponent<Fruit>();
                    newFruit.branchIndex = _growFruitIndex;
                    _growFruitIndex++;
                    newFruit.sourcePlant = this;

                    grownFruit.Add(newFruit);
                }
            }
        }

        float growT = _growth / growTime;
        UpdateGrowMat();
        
        debugText.text = "growT = " + growT;
        //mesh.transform.localScale = new Vector3(1,1, Mathf.Clamp(growT, 0.1f, 1.0f));
    }
}
