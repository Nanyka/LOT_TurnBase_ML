using System;
using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class Boss1Sensor : GOAPCreatureSensor
    {
        [SerializeField] private CreatureEntity m_Entity;
        [SerializeField] private SkillComp m_Skills;
        [SerializeField] private int detectRange;
        private EnvironmentData _envData;
        private EnvironmentManager _envManager;
        private CreatureData m_CreatureData;
        private Transform m_Transform;
        private int movingIndex;
        private List<Vector3> enemyHits = new();

        public override void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            m_Transform = transform;
            _envData = SavingSystemManager.Instance.GetEnvironmentData();
            _envManager = GameFlowManager.Instance.GetEnvManager();
        }

        // TODO: Check conditions based on goals
        public override void DetectEnvironment(WorldStates beliefs)
        {
            beliefs.ClearStates();
            DetectAttack(beliefs);
            if (beliefs.GetStates().Count == 0)
                DetectDefend(beliefs);
            if (beliefs.GetStates().Count == 0)
                DetectSupport(beliefs);
            if (beliefs.GetStates().Count == 0)
                DetectOppotunity(beliefs);
        }

        #region ATTACK

        private void DetectAttack(WorldStates beliefs)
        {
            // Get all adjacent tiles that is able to move
            // Check if any successful attack can be executed there?
            // Record this state into beliefs

            movingIndex = 0;
            var maxHit = 0;
            for (int i = 1; i < 5; i++)
            {
                var movement = _envManager.GetMovementInspector().MovingPath(m_Transform.position, i, 0, 0);

                if (movement.jumpCount > 0)
                {
                    var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i),
                        movement.jumpCount);
                    if (attackPoints == null)
                        continue;

                    int curHit = 0;
                    foreach (var attackPoint in attackPoints)
                        if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                            curHit++;

                    if (curHit > maxHit)
                    {
                        maxHit = curHit;
                        movingIndex = i;
                    }
                }
            }

            DecideAction(beliefs, movingIndex);
        }

        #endregion

        #region DEFEND

        private void DetectDefend(WorldStates beliefs)
        {
            // Get enemy list
            // Get each enemy potential attack points
            // Check if any attack hit me
            // Check actions to avoid

            enemyHits.Clear();
            movingIndex = 0;
            var enemies = _envData.PlayerData;
            foreach (var enemy in enemies)
            {
                for (int i = 1; i < 5; i++)
                {
                    var movement = _envManager.GetMovementInspector()
                        .MovingPath(enemy.Position, i, 0, 0);
                    if (movement.jumpCount > 0)
                    {
                        var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i),
                            movement.jumpCount);
                        if (attackPoints == null)
                            continue;

                        foreach (var attackPoint in attackPoints)
                        {
                            if (attackPoint == m_Transform.position)
                            {
                                enemyHits = (List<Vector3>)attackPoints;
                                goto LoopEnd;
                            }
                        }
                    }
                }
            }

            LoopEnd:
            {
                if (enemyHits.Count > 0)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        if (enemyHits.Contains(m_Transform.position + JIGeneralUtils.DirectionTo(i)) == false)
                        {
                            movingIndex = i;
                            break;
                        }
                    }
                }

                DecideAction(beliefs, movingIndex);
            }
        }

        #endregion

        #region SUPPORT

        private void DetectSupport(WorldStates beliefs)
        {
            // Get cohorts
            // Check each direction
            // How many jump cohort can execute in each direction if it is supported
            // Check curHit of the max jumping path
            // If curHit higher than maxHit, record support position

            var cohorts = _envData.EnemyData;
            Vector3 supportPos = Vector3.negativeInfinity;
            var maxHit = 0;

            foreach (var cohort in cohorts)
            {
                if (cohort == m_CreatureData)
                    continue;

                for (int i = 1; i < 5; i++)
                {
                    var jumpingPathScore = 0; // How the jumping path complete their shape
                    var checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 1;
                    }

                    checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 3;
                    }

                    checkFreePos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 5;
                    if (_envManager.FreeToMove(checkFreePos) == false &&
                        _envManager.CheckOutOfBoundary(checkFreePos) == false)
                    {
                        jumpingPathScore += 5;
                    }

                    Vector3 tempPos = Vector3.negativeInfinity;
                    Vector3 checkPos = Vector3.negativeInfinity;
                    var jumpCount = 0;
                    switch (jumpingPathScore)
                    {
                        case 0:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 2;
                            jumpCount = 1;
                            break;
                        }
                        case 1:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 4;
                            jumpCount = 2;
                            break;
                        }
                        case 3:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 4;
                            jumpCount = 2;
                            break;
                        }
                        case 4:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 5;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                        case 6:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 3;
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                        case 8:
                        {
                            tempPos = cohort.Position + JIGeneralUtils.DirectionTo(i);
                            checkPos = cohort.Position + JIGeneralUtils.DirectionTo(i) * 6;
                            jumpCount = 3;
                            break;
                        }
                    }

                    if (_envManager.CheckOutOfBoundary(checkPos) == false)
                    {
                        var attackPoints = AttackPoints(checkPos, JIGeneralUtils.DirectionTo(i), jumpCount);
                        var curHit = 0;
                        foreach (var attackPoint in attackPoints)
                            if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                                curHit++;

                        if (curHit > maxHit)
                        {
                            maxHit = curHit;
                            supportPos = tempPos;
                        }
                    }
                }
            }

            if (supportPos != Vector3.negativeInfinity)
            {
                float minDistance = Mathf.Infinity;
                for (int i = 1; i < 5; i++)
                {
                    var curDistance = Vector3.Distance((m_Transform.position + JIGeneralUtils.DirectionTo(i)),
                        supportPos);
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        movingIndex = i;
                    }
                }

                DecideAction(beliefs, movingIndex);
            }
        }

        #endregion

        #region FIND OPPOTUNITY

        private void DetectOppotunity(WorldStates beliefs)
        {
            // Get positions in range
            // Check available movements in each tiles
            // If any position provide successful attack, get it
            // Check the movement that bring character as close to the intent position as possible

            movingIndex = 0;
            Vector3 potentilPos = Vector3.negativeInfinity;

            for (int i = -detectRange; i <= detectRange; i++)
            {
                for (int j = -detectRange; j <= detectRange; j++)
                {
                    var detectPos = new Vector3(i, 0f, j);
                    detectPos += m_Transform.position;

                    if (_envManager.FreeToMove(detectPos) == false)
                        continue;

                    for (int k = 1; k < 5; k++)
                    {
                        var movement = _envManager.GetMovementInspector()
                            .MovingPath(detectPos, k, 0, 0);
                        if (movement.jumpCount > 0)
                        {
                            var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(k),
                                movement.jumpCount);
                            if (attackPoints == null)
                                continue;

                            foreach (var attackPoint in attackPoints)
                            {
                                if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                                {
                                    potentilPos = detectPos;
                                    goto LoopEnd;
                                }
                            }
                        }
                    }
                }
            }

            LoopEnd:
            {
                if (potentilPos != Vector3.negativeInfinity)
                {
                    float minDistance = Mathf.Infinity;
                    for (int i = 1; i < 5; i++)
                    {
                        var curDistance = Vector3.Distance((m_Transform.position + JIGeneralUtils.DirectionTo(i)),
                            potentilPos);
                        if (curDistance < minDistance)
                        {
                            minDistance = curDistance;
                            movingIndex = i;
                        }
                    }
                }

                DecideAction(beliefs, movingIndex);
            }
        }

        #endregion

        private static void DecideAction(WorldStates beliefs, int movingIndex)
        {
            // Select state and save it to belief
            switch (movingIndex)
            {
                case 1:
                    beliefs.ModifyState("GOLEFT", 1);
                    break;
                case 2:
                    beliefs.ModifyState("GORIGHT", 1);
                    break;
                case 3:
                    beliefs.ModifyState("GOBACKWARD", 1);
                    break;
                case 4:
                    beliefs.ModifyState("GOFORWARD", 1);
                    break;
            }
        }

        private IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
        {
            if (m_Skills.GetSkillAmount() < jumpStep || m_Skills.GetSkillByIndex(jumpStep - 1) == null)
                return null;
            return m_Skills.GetSkillByIndex(jumpStep - 1).CalculateSkillRange(targetPos, direction);
        }
    }
}