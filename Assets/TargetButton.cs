using RiskRecreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    public Country country;
    private void OnMouseDown()
    {
        GameObject.Find("MainGame").GetComponent<MainGame>().defendingCountry = country;
        GameObject.Find("MainGame").GetComponent<MainGame>().defenderChosen = true;
    }
}
