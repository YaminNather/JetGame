using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlockComponentDamagePlayer : LevelComponent
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            //Debug.Log("Player has entered damage trigger");
            ph.player.HealthReduce_F(100);
        }
    }
}
