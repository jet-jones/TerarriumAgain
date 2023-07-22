using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolCursor : MonoBehaviour
{
    TextMeshProUGUI cursorText;
    public enum ToolsType
    {
        None,
        WateringCan,
        Pebble,
        Moss,
        SeedGrass
    }

    void Start()
    {
        cursorText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeTool(string toolsTypeString)
    {
        ToolsType tt = (ToolsType)Enum.Parse(typeof(ToolsType), toolsTypeString);
        cursorText.text = toolsTypeString;
        if (toolsTypeString == "None")
            cursorText.text = "";
    }
    void Update()
    {
        cursorText.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }
}