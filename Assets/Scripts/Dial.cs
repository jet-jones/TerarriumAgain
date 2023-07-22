using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dial : MonoBehaviour, IInteraction {
    [SerializeField] private TextMeshProUGUI readoutText;
    [SerializeField] private MeshRenderer dialRenderer;
    [SerializeField] private Material dialMaterial;
    public float value;
    public float maxValue = 100;
    public bool Hovered { get; set; }
    public void OnInteract() {
        
    }
    
    private void Update() {
        if (Hovered) {
            value += Input.mouseScrollDelta.y * 3;
            value = Mathf.Clamp(value, 0, maxValue);
        }
        
        dialRenderer.material = Hovered ? Global.I.highlightMat : dialMaterial;
        readoutText.text = (int)value + "%";
        
        Hovered = false;
    }
}
