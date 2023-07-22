using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour, IInteraction {
    [SerializeField] private MeshRenderer handleMesh;
    [SerializeField] private Transform drawerTransform;
    [SerializeField] private Vector3 openOffset;
    private Material _handleMat;
    private Vector3 _drawClosedPos;
    private bool _open;
    
    private void Start() {
        _open = false;
        _handleMat = handleMesh.material;
        _drawClosedPos = drawerTransform.position;
    }

    private void Update() {
        handleMesh.material = Hovered ? Global.I.highlightMat : _handleMat;
        var targetDrawerPos = _open ? _drawClosedPos + openOffset : _drawClosedPos;
        drawerTransform.position = Vector3.Lerp(drawerTransform.position, targetDrawerPos, 6.0f * Time.deltaTime);
        Hovered = false;
    }

    public bool Hovered { get; set; }
    public void OnInteract() {
        _open = !_open;
    }
}
