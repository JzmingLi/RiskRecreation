using RiskRecreation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReinforceButton : MonoBehaviour
{
    public Country country;
    private void OnMouseDown()
    {
        GameObject.Find("MainGame").GetComponent<MainGame>().reinforcements = GameObject.Find("MainGame").GetComponent<MainGame>().reinforcements - 1;
        GameObject.Find("MainGame").GetComponent<MainGame>().reinforceCountry = country;
        GameObject.Find("MainGame").GetComponent<MainGame>().reinforced = true;

    }
}
