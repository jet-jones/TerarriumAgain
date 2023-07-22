using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject cursor;

    [SerializeField]
    private GameObject plantPrefab;

    [SerializeField]
    GameObject pebblePrefab;

    [SerializeField]
    private GameObject toolCursor;

    private void Start()
    {

    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        cursor.SetActive(false);

        if (Physics.Raycast(ray, out hit, 10000))
        {
            var interactable = hit.collider.GetComponent<IInteraction>();
            if (interactable != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    interactable.OnInteract();
                }

                interactable.Hovered = true;
            }
            else if (hit.collider.tag == "Ground")
            {
                cursor.SetActive(true);
                cursor.transform.position = hit.point - ray.direction * 0.2f;
                var worldCursorRotation = Quaternion.LookRotation(hit.normal);
                cursor.transform.rotation = worldCursorRotation * Quaternion.Euler(0, 0, 90);

                if (Input.GetMouseButtonDown(0))
                {

                    switch (toolCursor.GetComponent<ToolCursor>().currentTool)
                    {
                        case ToolCursor.ToolsType.Pebble:
                        {
                            Vector3 randScale = new Vector3(Random.Range(10f, 20f),
                                Random.Range(10f, 20f),
                                Random.Range(10f, 20f));
                            var pebble = Instantiate(pebblePrefab,
                                hit.point + new Vector3(0, 1.5f, 0),
                                Quaternion.Euler(-90, Random.Range(0, 360), 0));
                            pebble.transform.localScale = randScale;
                            break;
                        }

                        case ToolCursor.ToolsType.PebbleLarge:
                        {
                            Vector3 randScale = new Vector3(Random.Range(50f, 70f),
                                Random.Range(50f, 70f),
                                Random.Range(50f, 70f));
                            var pebble = Instantiate(pebblePrefab,
                                hit.point + new Vector3(0, 1.5f, 0),
                                Quaternion.Euler(-90, Random.Range(0, 360), 0));
                            pebble.transform.localScale = randScale;
                            break;
                        }
                        case ToolCursor.ToolsType.Plant1:
                        {
                            Instantiate(plantPrefab, hit.point, Quaternion.Euler(-90, Random.Range(0, 360), 0));
                            break;
                        }
                    }

                }
            }

        }

    }
}
