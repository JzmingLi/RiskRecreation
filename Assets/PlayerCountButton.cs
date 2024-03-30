using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCountButton : MonoBehaviour
{
    public int playerCount;
    private void OnMouseDown()
    {
        GameObject.Find("MainGame").GetComponent<MainGame>().playerCount = playerCount;
    }
}
