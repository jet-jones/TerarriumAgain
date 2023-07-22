using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Tool : MonoBehaviour, IInteraction {
    private MeshRenderer _renderer;
    private Material _defaultMat;
    
    private void Start() {
        _renderer = GetComponent<MeshRenderer>();
        _defaultMat = _renderer.material;
    }

    private void Update() {
        _renderer.material = Hovered ? Global.I.highlightMat : _defaultMat; 
        Hovered = false;
    }

    public bool Hovered { get; set; }
    public void OnInteract() {
        Destroy(gameObject);
    }
}
