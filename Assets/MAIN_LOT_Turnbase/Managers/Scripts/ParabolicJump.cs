using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class ParabolicJump : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        public List<Transform> tiles;
        public float jumpHeight = 2f; // Desired jump height
        public float jumpDuration = 1f; // Desired jump duration

        private Vector3 startPoint; // Starting point of the jump (Point A)
        private Vector3 endPoint; // Ending point of the jump (Point B)
        private bool isStartJumps;
        private int currentJump;
        private bool isJumping = false; // Flag to track if the object is jumping
        private float jumpStartTime; // Time when the jump started
        private Vector3 jumpDirection; // Direction vector from start to end point
        private float jumpDistance; // Distance between start and end point

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isStartJumps)
            {
                ResetJumps();
            }
            
            if (isStartJumps && !isJumping)
            {
                StartJump();
            }
            
            if (isJumping)
            {
                float timeSinceJumpStart = Time.time - jumpStartTime;
                float t = timeSinceJumpStart / jumpDuration;
            
                if (t < 1f)
                {
                    float verticalPosition = CalculateVerticalPosition(t);
                    float horizontalPosition = t * jumpDistance;
                    Vector3 newPosition = startPoint + jumpDirection * horizontalPosition;
                    newPosition.y = startPoint.y + verticalPosition;
                    transform.position = newPosition;
                }
                else
                {
                    EndJump();
                }
            }
        }

        void ResetJumps()
        {
            isStartJumps = true;
            currentJump = 0;
        }

        void StartJump()
        {
            startPoint = tiles[currentJump].position;
            endPoint = tiles[currentJump + 1].position;
            transform.position = startPoint;
            
            jumpDirection = endPoint - startPoint;
            jumpDistance = jumpDirection.magnitude;
            jumpDirection.Normalize();
            
            if (endPoint.y > startPoint.y)
            {
                jumpDuration = 1.2f;
                m_Animator.SetFloat("JumpSpeed",0.8f);
            }
            if (Math.Abs(endPoint.y - startPoint.y) < Mathf.Epsilon)
            {
                jumpDuration = 1f;
                m_Animator.SetFloat("JumpSpeed",1f);
            }
            if (endPoint.y < startPoint.y)
            {
                jumpDuration = 0.9f;
                m_Animator.SetFloat("JumpSpeed",1.1f);
            }
            
            isJumping = true;
            jumpStartTime = Time.time;
            m_Animator.SetBool("Jump",true);
        }

        float CalculateVerticalPosition(float t)
        {
            float verticalPosition = -4f * jumpHeight / (jumpDuration * jumpDuration) * (t - 0.5f * jumpDuration) * (t - 0.5f * jumpDuration) + jumpHeight;
            return verticalPosition;
        }

        void EndJump()
        {
            isJumping = false;
            transform.position = endPoint;
            m_Animator.SetBool("Jump",false);

            currentJump++;
            
            if (currentJump+1 >= tiles.Count)
            {
                Debug.Log("End jumps");
                isStartJumps = false;
            }
        }
    }
}