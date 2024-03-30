using RiskRecreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingButton : MonoBehaviour
{
    public Country country;
    private void OnMouseDown()
    {
        GameObject.Find("MainGame").GetComponent<MainGame>().attackingCountry = country;
        GameObject.Find("MainGame").GetComponent<MainGame>().attackerChosen = true;
    }
}
