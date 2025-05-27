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
    public LayerMask optIntersecMask;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (init && selected && intersectionOperationCheck)
        {
            IntersectionColoring();
        }
    }

    void IntersectionColoring()
    {
        Bounds b = selfRend.bounds;

        // is under map
        float height = Terrain.activeTerrain.SampleHeight(b.center);
        if (height > transform.position.y)
        {
            MakeValidOperation(false);
            Debug.Log("under map !");
            return;
        }
        
        // intersects other colliders
        List<Collider> cols = Physics.OverlapBox(b.center, b.extents / 2f, Quaternion.identity, optIntersecMask, QueryTriggerInteraction.Ignore).ToList();
        int n = cols.Where(e => e.gameObject != gameObject).ToArray().Length;
        MakeValidOperation(n == 0);
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

    public void MakeValidOperation(bool iState)
    {
        selfMatProp.SetFloat("_MaxDistance", 40f);
        SetColor(iState ? ValidOperationColor : InvalidOperationColor);
    }

    void SetColor(Color iColor)
    {
        selfMatProp.SetColor("_SelectionColor", iColor);
        
        selfRend.SetPropertyBlock(selfMatProp);
    }
}
