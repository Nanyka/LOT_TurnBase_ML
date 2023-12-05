using System.Collections.Generic;
using System.Linq;
using JumpeeIsland;
using UnityEngine;

public class Teleport : SkillEffect
{
    public void TakeEffectOn(ISkillRelated attackEntity, IAttackRelated sufferEntity)
    {
        var envManager = GameFlowManager.Instance.GetEnvManager();

        var currentFaction = attackEntity.GetFaction();
        var enemies = SavingSystemManager.Instance.GetEnvironmentData().PlayerData;
        if (currentFaction == FactionType.Player)
            enemies = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;

        Vector3 selectedEnemyPos = Vector3.negativeInfinity;
        if (enemies.Count > 0)
        {
            var lowHealthEnemy = enemies.Aggregate((l, r) => l.CurrentHp <= r.CurrentHp ? l : r);
            selectedEnemyPos = lowHealthEnemy.Position;
        }
        else
        {
            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;
            buildings = buildings.FindAll(t => t.FactionType != currentFaction);
            selectedEnemyPos = buildings.Find(t => t.BuildingType == BuildingType.TOWER).Position;
        }

        if (selectedEnemyPos.x.CompareTo(float.NegativeInfinity) == 0)
            return;
        
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

        if (telePos.x.CompareTo(float.NegativeInfinity) != 0)
            attackEntity.UpdateTransform(telePos, JIGeneralUtils.AdverseRotateTo(teleIndex));
    }
}