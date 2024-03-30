using RiskRecreation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ReinforcingStageHUD : MonoBehaviour
{
    public TextMeshPro player;
    public SpriteRenderer playerBackground;
    public TextMeshPro reinforcements;
    public TextMeshPro infantry;
    public TextMeshPro artillery;
    public TextMeshPro cavalry;
    
    
    public void UpdateVisuals(PlayerSlot slot, Vector3 cards, int reinforcements, Color color)
    {
        playerBackground.color = color;
        this.reinforcements.text = reinforcements.ToString();
        switch (slot)
        {
            case PlayerSlot.Player1:
                player.text = "Player 1";
                break;
            case PlayerSlot.Player2:
                player.text = "Player 2";
                break;
            case PlayerSlot.Player3:
                player.text = "Player 3";
                break;
            case PlayerSlot.Player4:
                player.text = "Player 4";
                break;
        }
        infantry.text = "Infantry: " + cards.x;
        infantry.text = "Artillery: " + cards.y;
        infantry.text = "Cavalry: " + cards.z;
    }
}
