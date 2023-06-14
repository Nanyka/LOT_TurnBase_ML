using System.Collections;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;

public class SpawnTree : GAction
{
    public override bool PrePerform()
    {
        var availableTile = GameFlowManager.Instance.GetEnvManager().GetAvailableTile();
        if (availableTile == Vector3.negativeInfinity)
        {
            Debug.Log("No any available tile");
        }
        else
        {
            Debug.Log($"Start spawn tree");
            SavingSystemManager.Instance.OnSpawnResource(InventoryType.Resource,availableTile);
        }
        
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("End spawn tree");
        return true;
    }
}
