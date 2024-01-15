using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlipCard : MonoBehaviour, IPointerClickHandler
{
    private CardScript cardScript;

    private void Awake()
    {
        cardScript = GetComponent<CardScript>();
    }
    public void Flip()
    {
        cardScript.IsFlipped = !cardScript.IsFlipped;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.locked && gameObject.activeSelf && !GameManager.Instance.wait)
            Flip();
    }
}
