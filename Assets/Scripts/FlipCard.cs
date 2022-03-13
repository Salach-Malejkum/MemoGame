using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlipCard : MonoBehaviour, IPointerClickHandler
{
    GameManager script;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!script.locked && gameObject.activeSelf && !script.wait)
            transform.GetChild(1).gameObject.SetActive(false);
    }
}
