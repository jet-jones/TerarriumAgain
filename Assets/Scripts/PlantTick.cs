using System.Collections;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]
[RequireComponent(typeof(AdvancedPlant))]
public class PlantTick: MonoBehaviour
{
    [SerializeField]
    private int seed;

    AdvancedPlant plant;

    // private void OnDrawGizmosSelected()
    // {
    //     AdvancedPlant plant = GetComponent<AdvancedPlant>();
    //     plant.DrawDebug();
    // }

    private void Awake()
    {
        plant = GetComponent<AdvancedPlant>();
        StartCoroutine(SlowUpdate());

    }
    private IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.02f));
            plant.Generate(seed);
        }
    }
}
