using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject.Find("MainGame").GetComponent<MainGame>().exchanged = true;
        Destroy(gameObject);
    }
}
