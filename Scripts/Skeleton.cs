using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Skeleton : MonoBehaviour
    {
        [SerializeField] private int m_health = 100;//wrong naming, no prefix('m_') needed
        [SerializeField] private int m_moveRange = 20;//wrong naming, no prefix('m_') needed
        [SerializeField] private int m_attackRange = 5;//wrong naming, no prefix('m_') needed

        [Range(0, .3f)] [SerializeField] private float m_movementSmoothing = .05f;//wrong naming, no prefix('m_') needed
        [SerializeField] private float m_walkSpeed = 10f;//wrong naming, no prefix('m_') needed
        public float m_moveCoordinates;//wrong naming, no prefix('m_') needed
        [SerializeField] private Rigidbody2D m_rigidBody;//wrong naming, no prefix('m_') needed
        [SerializeField] private Animator m_animator;//wrong naming, no prefix('m_') needed
        [SerializeField] private GameObject m_projectile;//wrong naming, no prefix('m_') needed
        [SerializeField] private Transform m_firepointRight; //wrong naming, no prefix('m_') needed
        //use for shoot the player and also for raycast
        [SerializeField] private Transform m_firepointLeft; //wrong naming, no prefix('m_') needed
        //use only for raycast - may be deleted - fix later
        [SerializeField] private Transform m_firepoint;//wrong naming, no prefix('m_') needed
        //because of the fucking Quaternion in firepoint right
        //DONT CHANGE FIREPOINTS POSITIONS

        private Vector3 m_velocity = Vector3.zero; //wrong naming, no prefix('m_') needed

        private bool m_facingRight = true;//wrong naming, no prefix('m_') needed
        private bool m_moving;//wrong naming, no prefix('m_') needed
        private bool m_started = true; //wrong naming, no prefix('m_') needed

        private enum State//wrong naming, should be like AnimationState
        {
            Idle,
            Walk,
            Attack,
            Die
        };

        private State m_state = State.Idle;//wrong naming, no prefix('m_') needed

        public void TakeDamage(int damage)
        {
            m_health -= damage;
            if (m_health > 0) return;
            if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonDie")) Die();
        }

        private void Die()
        {
            m_animator.SetTrigger("Die");
            Destroy(gameObject, 1);
        }

        private void Shoot()//wrong naming, should be ShootProjectile
                            //Used as event-method in animation SkeletonAttack
        {
            Instantiate(m_projectile, m_firepoint.position, m_firepoint.rotation);
        }

        private void Move(float move)
        {
            Vector3 targetVelocity = new Vector2(move * 10f, m_rigidBody.velocity.y);
            // And then smoothing it out and applying it to the character
            m_rigidBody.velocity =
                Vector3.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref m_velocity, m_movementSmoothing);
        }

        private void UpdateState()
        {
            AnimatorClipInfo[] currentState = m_animator.GetCurrentAnimatorClipInfo(0);
            string state = currentState[0].clip.name;
            switch (state)
            {
                case "SkeletonWalk":
                    m_state = State.Walk;
                    break;
                case "SkeletonDie":
                    m_state = State.Die;
                    break;
                case "SkeletonAttack":
                    m_state = State.Attack;
                    break;
                default:
                    m_state = State.Idle;
                    break;
            }
        }

        private void Attack()
        {
            if (Math.Abs(Math.Abs(m_moveCoordinates) - Math.Abs(transform.position.x)) < m_attackRange)
            {
                if (m_state != State.Die || m_state != State.Attack)
                {
                    m_animator.SetTrigger("Attack");
                }
            }
        }

        private void Eye()//inapropriate naming, should be named FindPlayer; also can me replaced by integrated method moveTowards
        {
            var angle = Mathf.Sin(Time.time * 100) * 360; //tweak this to change frequency
            RaycastHit2D hitRight = Physics2D.Raycast(m_firepointRight.position, m_firepointRight.right, m_moveRange);
            m_firepointRight.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            RaycastHit2D hitLeft = Physics2D.Raycast(m_firepointLeft.position, m_firepointLeft.right, m_moveRange);
            m_firepointLeft.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (hitRight)
                if (hitRight.collider.CompareTag("Player"))
                {
                    m_moveCoordinates = hitRight.collider.transform.position.x;
                    m_started = false;
                    return;
                }

            if (hitLeft)
            {
                if (hitLeft.collider.CompareTag("Player"))
                {
                    m_moveCoordinates = hitLeft.collider.transform.position.x;
                    float distance = transform.position.x - m_moveCoordinates;

                    if (!m_facingRight && distance < 0)
                    {
                        Flip();
                    }

                    if (m_facingRight && distance > 0)
                    {
                        Flip();
                    }
                    m_started = false;
                }
            }
        }
        
        private void Update()
        {
            Eye();
            UpdateState();
            RaycastHit2D hitOnFace =
                Physics2D.Raycast(m_firepointRight.position, Vector3.right * (m_facingRight ? 1 : -1));
            RaycastHit2D hitNotOnFace =
                Physics2D.Raycast(m_firepointLeft.position, Vector3.left * (m_facingRight ? 1 : -1));

            if (hitOnFace)
            {
                if (hitOnFace.collider.CompareTag("Player") && m_state != State.Attack)
                {
                    Attack();
                }
            }

            if (hitNotOnFace)
            {
                if (hitNotOnFace.collider.CompareTag("Player") && m_state != State.Attack)
                {
                    Attack();
                }
            }
            float distance = Math.Abs(Math.Abs(m_moveCoordinates) - Math.Abs(transform.position.x));
            if (!m_started)
            {
                if (distance < 0.1f && m_moving)
                {
                    if (m_state != State.Die && m_state != State.Attack)
                    {
                        m_animator.SetBool("Walk", false);
                        m_moving = false;
                    }
                }

                if (distance > 0.1f && !m_moving)
                {
                    if (m_state != State.Die && m_state != State.Attack && distance > m_attackRange)
                    {
                        m_animator.SetBool("Walk", true);
                        m_moving = true;
                    }
                }
            }

            if (m_moving && m_state != State.Die && m_state != State.Attack && distance > m_attackRange)
            {
                Move(m_walkSpeed * Time.fixedDeltaTime * (m_facingRight ? 1 : -1));
            }

        }

        private void Flip()
        {
            transform.Rotate(0f, 180f, 0f); //flipping
            m_facingRight = !m_facingRight;
        }

        public void setMaterial(PhysicsMaterial2D material)//wrong naming, should be SetMaterial
        {
            m_rigidBody.sharedMaterial = material;
        }

    }
}