using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Constants;

public class UIActionBubble : MonoBehaviour
{
    public PLAYER_ACTIONS associatedAction;
    public Image selfImg;
    public Image shaderAnimatedImg;
    public UnityEvent<UIActionBubble> CollidedCB;
    private RectTransform rt;

    private Material localMat;

    private Coroutine RippleCo;

    private bool lockedInFX = false;

    public void Init()
    {
        rt = GetComponent<RectTransform>();
        if (shaderAnimatedImg != null)
        {
            localMat = Instantiate(shaderAnimatedImg.material);
            shaderAnimatedImg.material = localMat;

            var rect = shaderAnimatedImg.rectTransform.rect;
            Vector2 size = new Vector2(rect.width, rect.height);

            localMat.SetVector(uishad_centerOffset, -rt.anchoredPosition.normalized);
            localMat.SetFloat(uishad_rippleBlend, 0f);
            RefreshImg();
        }
    }

    void OnDestroy()
    {
        // clean localmat
    }

    private void RefreshImg() { shaderAnimatedImg.SetMaterialDirty(); }
    void OnCollisionEnter2D(Collision2D iCollider)
    {
        CollidedCB.Invoke(this);

        if (lockedInFX)
            return;

        ContactPoint2D c = iCollider.GetContact(0);
        Vector2 impactSpot = c.normal;
        //impactSpot += new Vector2(0.5f, 0.5f);
        localMat.SetVector(uishad_impact, -impactSpot);
        localMat.SetFloat(uishad_rippleBlend, 1f);
        RefreshImg();

        if (RippleCo != null)
        {
            StopCoroutine(RippleCo);
            RippleCo = null;
        }
        RippleCo = StartCoroutine(RippleFXCo());
    }

    IEnumerator RippleFXCo()
    {
        lockedInFX = true;

        float duration = 1f;

        float elapsedTime = 0f;

        localMat.SetFloat(uishad_rippleBlend, 1f);
        RefreshImg();

        float blendVal = 0f;
        float frac = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;

            frac = elapsedTime / duration;
            blendVal = Utils.Lerp(1f, 0f, frac);
            localMat.SetFloat(uishad_rippleBlend, blendVal);
            RefreshImg();
            yield return null;
        }

        localMat.SetFloat(uishad_rippleBlend, 0f);
        RefreshImg();

        lockedInFX = false;
    }

}
