using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class SelectionFX : MonoBehaviour
{
    public Color selectionColor = new Color(0f, 0.8310704f, 1f, 1f);
    public Color unselectedColor = Color.black;
    public Color ValidOperationColor = Color.green;
    public Color InvalidOperationColor = Color.red;

    MaterialPropertyBlock selfMatProp;
    Renderer selfRend;
    bool init = false;
    bool selected = false;
    public bool intersectionOperationCheck = false;
    public bool operationIsValid = true;
  

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (init && selected && intersectionOperationCheck)
        {
            RefreshOperationColor();
        }
    }

    public void Init()
    {
        if (init)
            return;

        selfRend = GetComponent<Renderer>();
        selfMatProp = new MaterialPropertyBlock();
        selfRend.GetPropertyBlock(selfMatProp);
        Deselect();
        init = true;
    }

    public void Select()
    {
        selfMatProp.SetFloat("_MaxDistance", 40f);
        SetColor(selectionColor);
        selected = true;
    }

    public void Deselect()
    {
        selfMatProp.SetFloat("_MaxDistance", 0f);
        SetColor(unselectedColor);
        selected = false;
    }

    public void RefreshOperationColor()
    {
        selfMatProp.SetFloat("_MaxDistance", 40f);
        SetColor(operationIsValid ? ValidOperationColor : InvalidOperationColor);
    }

    void SetColor(Color iColor)
    {
        selfMatProp.SetColor("_SelectionColor", iColor);
        
        selfRend.SetPropertyBlock(selfMatProp);
    }
}
