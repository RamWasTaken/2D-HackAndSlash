using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Heal : PowerUp
{
    [Header("Settings")]
    public float amount = 5f;
    public override void Activate()
    {
        Player.CurrentHealth += amount;
        Debug.Log("Player :" + Player.CurrentHealth);
        base.Activate();
    }
}
