using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SelectionFX : MonoBehaviour
{
    public Color selectionColor = new Color(1f, 0.5f, 0f, 1f);
    public Color unselectedColor = Color.black;

    MaterialPropertyBlock selfMatProp;
    Renderer selfRend;
    bool init = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
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

    }

    public void Deselect()
    {
        selfMatProp.SetFloat("_MaxDistance", 0f);
        SetColor(unselectedColor);
        
    }

    void SetColor(Color iColor)
    {
        selfMatProp.SetColor("_SelectionColor", iColor);
        
        selfRend.SetPropertyBlock(selfMatProp);
    }
}
