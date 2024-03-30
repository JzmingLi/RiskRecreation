using RiskRecreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public int type; //0 = increment up, 1 = increment down, 2 = fortify, 3 = skip attack

    private void OnMouseDown()
    {
        switch (type)
        {
            case 0:
                GameObject.Find("MainGame").GetComponent<MainGame>().incrementUp = true;
                break;
            case 1:
                GameObject.Find("MainGame").GetComponent<MainGame>().incrementDown= true;
                break;
            case 2:
                GameObject.Find("MainGame").GetComponent<MainGame>().fortified = true;
                break;
            case 3:
                GameObject.Find("MainGame").GetComponent<MainGame>().skipAndFortify = true;
                break;


        }
    }
}
