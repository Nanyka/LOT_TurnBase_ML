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
        [SerializeField] private Skill_SO enemySkill;
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
            if (beliefs.GetStates().Count == 0)
                DeteckPickUp(beliefs);
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
                int dummyJump = movement.jumpCount + m_Entity.GetJumpBoost();
                
                if (dummyJump > 0)
                {
                    var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i),
                        dummyJump);
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
                        var enemyInventory = SavingSystemManager.Instance.GetInventoryItemByName(enemy.EntityName);
                        // var enemySkill = (Skill_SO)AddressableManager.Instance.GetAddressableSO(
                        //     enemyInventory.skillsAddress[movement.jumpCount - 1]);

                        var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(i), enemySkill);
                        if (attackPoints == null)
                            continue;

                        foreach (var attackPoint in attackPoints)
                        {
                            if (Vector3.Distance(attackPoint, m_Transform.position) < 0.1f)
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
                        var lurePos = m_Transform.position + JIGeneralUtils.DirectionTo(i);
                        if (enemyHits.Contains(lurePos) == false && _envManager.FreeToMove(lurePos))
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

            MoveToTarget(supportPos);
            DecideAction(beliefs, movingIndex);
        }

        #endregion

        #region FIND OPPOTUNITY

        private void DetectOppotunity(WorldStates beliefs)
        {
            // Get object in range
            // Check any available position to execute successful attacks from the objects
            // Check jumping point for that attack is available or not
            // Check the movement that bring character as close to the intent position as possible
            movingIndex = 0;
            Vector3 potentialPos = Vector3.negativeInfinity;

            for (int i = -detectRange; i <= detectRange; i++)
            {
                for (int j = -detectRange; j <= detectRange; j++)
                {
                    var detectPos = new Vector3(i, 0f, j);
                    if (detectPos == Vector3.zero)
                        continue;

                    detectPos += m_Transform.position;

                    if (_envManager.FreeToMove(detectPos) || _envManager.CheckOutOfBoundary(detectPos))
                        continue;

                    for (int k = 1; k < 5; k++)
                    {
                        var jumpPos = detectPos + JIGeneralUtils.AdverseDirectionTo(k);
                        if (_envManager.FreeToMove(jumpPos))
                        {
                            var movement = _envManager.GetMovementInspector()
                                .MovingPath(jumpPos, k, 0, 0);
                            if (movement.jumpCount > 0)
                            {
                                movement.jumpCount += m_Entity.GetJumpBoost();
                                var attackPoints = AttackPoints(movement.returnPos, JIGeneralUtils.DirectionTo(k),
                                    movement.jumpCount);
                                if (attackPoints == null)
                                    continue;
                            
                                foreach (var attackPoint in attackPoints)
                                {
                                    if (_envManager.CheckEnemy(attackPoint, m_Entity.GetFaction()))
                                    {
                                        potentialPos = jumpPos;
                                        goto LoopEnd;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LoopEnd:
            {
                MoveToTarget(potentialPos);
                DecideAction(beliefs, movingIndex);
            }
        }

        #endregion

        #region PICK UP

        public void DeteckPickUp(WorldStates beliefs)
        {
            var pickUpList = SavingSystemManager.Instance.GetEnvironmentData().CollectableData;
            var pickUpPos = Vector3.negativeInfinity;
            movingIndex = 0;
            
            foreach (var pickUp in pickUpList)
            {
                // Test the first item
                pickUpPos = pickUp.Position;
                goto LoopEnd;
            }
            
            LoopEnd:
            {
                MoveToTarget(pickUpPos);

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

        private IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, Skill_SO skill)
        {
            return skill.CalculateSkillRange(targetPos, direction);
        }

        private void MoveToTarget(Vector3 target)
        {
            if (target.x.CompareTo(float.NegativeInfinity) == 1)
            {
                float minDistance = Mathf.Infinity;
                for (int i = 1; i < 5; i++)
                {
                    var curDistance = Vector3.Distance((m_Transform.position + JIGeneralUtils.DirectionTo(i)),
                        target);
                    if (curDistance < minDistance && _envManager.FreeToMove(target))
                    {
                        minDistance = curDistance;
                        movingIndex = i;
                    }
                }
            }
        }
    }
}