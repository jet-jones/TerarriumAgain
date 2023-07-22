using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(AdvancedPlant))]
public class PlantEditor : MonoBehaviour
{
    [SerializeField] private int seed;
    
    private void OnDrawGizmosSelected() {
        AdvancedPlant plant = GetComponent<AdvancedPlant>();
        plant.DrawDebug();
    }

    private void Awake() {
        Debug.Log("start");
    }
    
    private void Update() {
        AdvancedPlant plant = GetComponent<AdvancedPlant>();
        plant.Generate(seed);
    }
}
