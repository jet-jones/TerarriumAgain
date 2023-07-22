using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolCursor : MonoBehaviour
{
    TextMeshProUGUI cursorText;
    public ToolsType currentTool;
    public enum ToolsType
    {
        None,
        Plant1,
        WateringCan,
        Pebble,
        Moss,
        Vines,
        PebbleLarge,
    }

    public GameObject pebblePrefab;
    
    void Start()
    {
        cursorText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeTool(string toolsTypeString)
    {
        ToolsType tt = (ToolsType)Enum.Parse(typeof(ToolsType), toolsTypeString);
        cursorText.text = toolsTypeString;
        currentTool = tt;
        if (toolsTypeString == "None")
            cursorText.text = "";
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("mousedown:" + currentTool);

            switch (currentTool)
            {
                case ToolsType.WateringCan:
                {
                    break;
                }
                case ToolsType.Pebble:
                {
                     break;
                }
            }
        }
        cursorText.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }
}