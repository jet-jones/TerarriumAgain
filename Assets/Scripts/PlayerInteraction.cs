using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject plantPrefab;

    public GameObject toolCursor;
    
    private void Start()
    {
        
    }

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        cursor.SetActive(false);
        
        if (Physics.Raycast(ray, out hit, 10000)) {
            var interactable = hit.collider.GetComponent<IInteraction>();
            if (interactable != null) {
                if (Input.GetMouseButtonDown(0)) {
                    interactable.OnInteract();
                }
                
                interactable.Hovered = true;
            } else if (hit.collider.tag == "Ground"){
                cursor.SetActive(true);
                cursor.transform.position = hit.point - ray.direction * 0.2f;
                var worldCursorRotation = Quaternion.LookRotation(hit.normal);
                cursor.transform.rotation = worldCursorRotation * Quaternion.Euler(0, 0, 90);

                if (Input.GetMouseButtonDown(0)) {
                    
                    
                    Instantiate(plantPrefab, hit.point, Quaternion.Euler(-90, Random.Range(0, 360), 0));
                    
                }
            }

        }
        
    }
}
