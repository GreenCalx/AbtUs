using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

using static Constants;

[RequireComponent(typeof(RectTransform))]
public class UIActionBubbles : MonoBehaviour
{
    [Header("Prefab Refs")]
    public GameObject prefab_ActionBubble;

    [Header("Mand Refs")]
    public Image wheelImage;
    public Vector2 baseWheelImageSize = new Vector2(25f, 25f); // crosshair
    private Vector2 targetWheelImageSize;
    public UISelectedActionBubble selector;
    public List<UIActionBubble> actionBubbles;
    public Vector3 origin = Vector3.zero;
    public RectTransform self;
    private RectTransform selectorRect;

    [Header("Internals")]
    public float radius = 50f;
    public float imgScaleDuration = 0.5f;
    private Coroutine lerpImgCo;

    // void Start()
    // {
    //     origin = self.transform.position;
    //     selectorRect = selector.GetComponent<RectTransform>();
        
    //     targetWheelImageSize = new Vector2( self.rect.width, self.rect.height);
    // }

    IEnumerator LerpImgSizeCo()
    {
        float elapsed = 0f;
        while (elapsed < imgScaleDuration)
        {
            wheelImage.transform.localScale = Vector2.Lerp(baseWheelImageSize, targetWheelImageSize, elapsed / imgScaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        wheelImage.transform.localScale = targetWheelImageSize;
    }

    public void Init(PLAYER_ACTIONS[] iActions)
    {
        UIGame.Instance.ForceHideCursor();
        if (iActions.Length==4)
            wheelImage.sprite = UIGame.Instance.GetHResActionSprite(PLAYER_ACTIONS.WHEEL4);
        else if (iActions.Length==3)
            wheelImage.sprite = UIGame.Instance.GetHResActionSprite(PLAYER_ACTIONS.WHEEL3);
        else
            wheelImage.sprite = UIGame.Instance.GetHResActionSprite(PLAYER_ACTIONS.WHEEL2);

        origin = self.transform.position;
        selectorRect = selector.GetComponent<RectTransform>();
        
        targetWheelImageSize = new Vector2(
                radius / 2.5f,
                radius / 2.5f
            );

        if (lerpImgCo != null)
        {
            StopCoroutine(lerpImgCo);
            lerpImgCo = null;
        }
        lerpImgCo = StartCoroutine(LerpImgSizeCo());

        float angleStep = 360f / iActions.Length;
        for (int i = 0; i < iActions.Length; i++)
        {
            GameObject newBubble = Instantiate(prefab_ActionBubble);

            RectTransform as_rt = newBubble.GetComponent<RectTransform>();
            as_rt.SetParent(self);

            float angle = i * angleStep;
            as_rt.anchoredPosition =
                new Vector2(
                   (((as_rt.rect.size.x + as_rt.rect.size.y) / 2f) + radius) * Mathf.Cos((angle) * Mathf.PI / 180f),
                   (((as_rt.rect.size.x + as_rt.rect.size.y) / 2f) + radius) * Mathf.Sin((angle) * Mathf.PI / 180f)
                );
            UIActionBubble as_bubble = newBubble.GetComponent<UIActionBubble>();
            as_bubble.Init();
            as_bubble.associatedAction = iActions[i];
            as_bubble.selfImg.sprite = UIGame.Instance.GetActionSprite(iActions[i]);

            as_bubble.CollidedCB = new UnityEvent<UIActionBubble>();
            as_bubble.CollidedCB.AddListener(selector.ChangeSelectedAction);

            actionBubbles.Add(as_bubble);
        }

        selector.Reset();

    }

    public void Clear()
    {
        actionBubbles.ForEach(e => GameObject.Destroy(e.gameObject));
        actionBubbles.Clear();

        selector.Reset();
        wheelImage.transform.localScale = baseWheelImageSize;
    }

    void FixedUpdate()
    {
        //selectorRect.anchoredPosition = Vector3.ClampMagnitude(Input.mousePosition - origin, radius);
        var posFromAnchor = Vector3.ClampMagnitude(Input.mousePosition - origin, radius);
        selector.selfRB.MovePosition(origin + posFromAnchor);
    }

}
