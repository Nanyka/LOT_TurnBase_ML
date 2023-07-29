using System.Collections.Generic;
using System.Linq;
using JumpeeIsland;
using UnityEngine;

public class Teleport : SkillEffect
{
    public void TakeEffectOn(Entity attackEntity, Entity sufferEntity)
    {
        var envManager = GameFlowManager.Instance.GetEnvManager();
        
        var currentEntity = attackEntity.GetFaction();
        var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
        if (currentEntity == FactionType.Player)
            enemies = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;

        var lowHealthEnemy = enemies.Aggregate((l, r) => l.CurrentHp < r.CurrentHp ? l : r);
        var selectedEnemyPos = lowHealthEnemy.Position;
        var telePos = Vector3.negativeInfinity;
        int teleIndex = 0;
        for (int i = 1; i < 5; i++)
        {
            teleIndex = i;
            telePos = selectedEnemyPos + JIGeneralUtils.DirectionTo(teleIndex);
            telePos = envManager.GetTilePosByGeoPos(telePos);
            if (envManager.FreeToMove(telePos) 
                && envManager.CheckHigherTile(selectedEnemyPos, telePos) == false)
                break;
        }
        
        if (telePos != Vector3.negativeInfinity)
        {
            attackEntity.UpdateTransform(telePos, JIGeneralUtils.AdverseRotateTo(teleIndex));
        }
    }
}