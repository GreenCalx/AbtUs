using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine;
using UnityEngine.Events;
public struct BridgeBrick
{
    public Vector3 initPosition;
    public Vector3 position;
}
public class OrderBridge : MonoBehaviour
{
    public ComputeShader cs;
    public GameObject brickPrefab;
    public int xcount = 50;
    public int ycount = 50;
    public int brick_sizex = 2;
    public int brick_sizey = 2;
    private List<GameObject> objects;
    private BridgeBrick[] data;
    public float targetZ = 5f;
    public float animTime = 2f;
    float lerp = 0f;
    float orderHelpOffset = 0.5f;
    ComputeBuffer buff;
    Coroutine lerpCo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateBridge();
    }

    void Update()
    {
        if (OverWorldControl.Instance.OrderMagnitude > 0f)
        {
            if (lerpCo != null)
            {
                StopCoroutine(lerpCo);
                lerpCo = null;
            }
            lerpCo = StartCoroutine(LerpCo());
            // this.enabled = false;
        }
    }

    void OnDestroy()
    {
        buff.Release();
    }

    // Update is called once per frame
    void CreateBridge()
    {
        objects = new List<GameObject>();

        data = new BridgeBrick[xcount * ycount];
        for (int y = 0; y < ycount; y++)
        {
            for (int x = 0; x < xcount; x++)
            {
                CreateBrick(x, y, brick_sizex, brick_sizey);
            }
        }
    }

    void CreateBrick(int x, int y, int sizex, int sizey)
    {
        GameObject brick = Instantiate(brickPrefab);
        brick.transform.parent = transform;
        brick.transform.localPosition = new Vector3(x*sizex, y*sizey, UnityEngine.Random.Range(10f, 20f));
        brick.transform.localScale = new Vector3(sizex, 1f, sizey);

        objects.Add(brick);

        BridgeBrick brickData = new BridgeBrick();
        brickData.initPosition = brick.transform.localPosition;
        brickData.position = brick.transform.localPosition;
        data[x + y*xcount] = brickData;
    }

    void OnOrder()
    {
        int vec3Sz = sizeof(float) * 3;
        buff = new ComputeBuffer(data.Length, vec3Sz * 2);
        buff.SetData(data);

        cs.SetBuffer(0, "bricks", buff);
        cs.SetFloat("resolution", data.Length);
        cs.SetInt("width", xcount);
        cs.SetInt("height", ycount);
        cs.SetFloat("targetZ", targetZ);
        cs.SetFloat("lerpValue", lerp);

        cs.Dispatch(0, data.Length / 10, 1, 1);

        buff.GetData(data);
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject o = objects[i];
            BridgeBrick d = data[i];
            o.transform.localPosition = d.position;
        }
        

    }
    
    IEnumerator LerpCo()
    {
        while (OverWorldControl.Instance.OrderMagnitude > 0f)
        {
            lerp = Mathf.Clamp01(OverWorldControl.Instance.OrderMagnitude*2f);
            Debug.Log("lerp : " + lerp);
            OnOrder();
            yield return null;
        }
        lerp = 0f;
        OnOrder();
    }
}
