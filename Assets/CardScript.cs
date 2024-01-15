using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    private bool isFlipped = false;
    public bool IsFlipped
    {
        get => isFlipped;
        set
        {
            isFlipped = value;
            transform.GetChild(0).gameObject.SetActive(!value);
            transform.GetChild(1).gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
