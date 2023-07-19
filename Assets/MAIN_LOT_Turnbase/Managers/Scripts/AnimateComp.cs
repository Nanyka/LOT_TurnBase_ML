using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AnimateComp : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        
        private List<Vector3> tiles = new();
        private Transform m_Transform;
        private ICreatureMove m_Creature;
        private Vector3 direction;
        private Vector3 destination; // Ending point of the jump (Point B)
        private bool isStartMoves; // Flag to track if the object is in a move loop
        private bool isMoving; // Flag to track if the object is jumping
        private int moveIndex;

        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int JumpUp = Animator.StringToHash("JumpUp");
        private static readonly int JumpDown = Animator.StringToHash("JumpDown");
        private static readonly int JumpUp1 = Animator.StringToHash("JumpUp1");
        private static readonly int JumpDown1 = Animator.StringToHash("JumpDown1");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Jump = Animator.StringToHash("Jump");

        private void Start()
        {
            m_Transform = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isStartMoves)
                ResetMoves();

            if (isStartMoves && !isMoving)
                StartMove();

            if (isMoving)
            {
                if (Vector3.Distance(destination, transform.position) < 0.1f)
                {
                    EndMove();
                }
            }
        }

        public void MoveToTarget(Vector3 currPos, int moveDir, ICreatureMove creature)
        {
            m_Creature ??= creature;
            tiles.Clear();

            // currPos = new Vector3(Mathf.RoundToInt(currPos.x), Mathf.RoundToInt(currPos.y),
            //     Mathf.RoundToInt(currPos.z));
            // m_Transform.position = currPos;
            GameFlowManager.Instance.GetEnvManager().GetMovementInspector().MovingPath(currPos, moveDir, tiles);
            // Debug.Log($"{gameObject} move to {moveDir} from {currPos} over {tiles.Count} tiles");

            ResetMoves();
        }

        private void ResetMoves()
        {
            moveIndex = 0;
            isStartMoves = true;
        }

        private void StartMove()
        {
            if (tiles.Count == 0)
            {
                EndMove();
                return;
            }

            destination = tiles[moveIndex];
            direction = new Vector3(destination.x, m_Transform.position.y, destination.z);
            m_Transform.LookAt(direction);

            if (Mathf.Abs(destination.y - transform.position.y) < 0.1f)
            {
                if (Mathf.Abs(Vector3.Distance(destination, transform.position)) < 1.5f)
                    m_Animator.SetBool(Walk, true);
                else
                    m_Animator.SetTrigger(Jump);
            }
            else if (destination.y > transform.position.y)
                m_Animator.SetTrigger(Mathf.Abs(Vector3.Distance(destination, transform.position)) < 1.5f
                    ? JumpUp1
                    : JumpUp);
            else if (destination.y < transform.position.y)
                m_Animator.SetTrigger(Mathf.Abs(Vector3.Distance(destination, transform.position)) < 1.5f
                    ? JumpDown1
                    : JumpDown);

            isMoving = true;
        }

        private void EndMove()
        {
            m_Animator.SetBool(Walk, false);

            isMoving = false;
            moveIndex++;

            if (moveIndex >= tiles.Count)
            {
                // Debug.Log($"End moves at {transform.position}");
                isStartMoves = false;
                m_Creature.CreatureEndMove();
            }
        }

        public void SetAnimation(AnimateType animate)
        {
            switch (animate)
            {
                case AnimateType.Attack:
                    m_Animator.SetTrigger(Attack);
                    break;
                case AnimateType.Die:
                    m_Animator.SetTrigger(Die);
                    break;
            }
        }

        public void SetAnimator(Animator animator)
        {
            m_Animator = animator;
        }
    }
}