using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour, IInteraction
{
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.4f;

    [HideInInspector] public Plant sourcePlant;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private float growTimeMin = 2;
    [SerializeField] private float growTimeMax = 4;
    
    private float _growth;
    public bool Hovered { get; set; }
    private Material fruitMat;
    private float growTime;
    private float scale;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        fruitMat = mesh.material;
        scale = Random.Range(minScale, maxScale);
        growTime = Random.Range(growTimeMin, growTimeMax);
    }
    
    private void Update() {
        mesh.material = Hovered ? Global.I.highlightMat : fruitMat;
        if (_growth < growTime) {
            _growth += Time.deltaTime;
            if (_growth > growTime) {
                
            }
        }

        var growT = _growth / growTime;
        transform.localScale = scale * growT * Vector3.one; 
        
        Hovered = false;
    }
    
    public void OnInteract() {
        sourcePlant.grownFruit.Remove(this);
        Destroy(gameObject);
    }
}
