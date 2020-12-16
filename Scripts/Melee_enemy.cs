using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_enemy : MonoBehaviour////wrong naming, should be MeleeEnemy
{
    #region wrong naming, no prefix ('m_') needed

    [SerializeField] private int m_health = 100;
    [SerializeField] private int m_moveRange = 20;
    [SerializeField] private float m_attackRange = 5;
    [SerializeField] private int m_damage = 5;
    [Range(0, .3f)] [SerializeField] private float m_movementSmoothing = .05f;
    [SerializeField] private float m_walkSpeed = 10f;
    public float m_moveCoordinates;
    [SerializeField] private Rigidbody2D m_rigidBody;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_firepointRight;
    [SerializeField] private Transform m_firepointLeft;
    [SerializeField] private LayerMask m_layersToHit;
    private Vector3 m_velocity = Vector3.zero;

    private bool m_facingRight = true;
    private bool m_moving;
    private bool m_started = true;

    #endregion 


    private enum State//wrong naming, could be like AnimationState
    {
        Idle,
        Walk,
        Attack,
        Hurt,
        Die
    };

    private State m_state = State.Idle;//wrong naming, no prefix ('m_') needed

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (m_health < 0 && m_state != State.Die)
        {
            m_walkSpeed = 0;
            m_animator.SetTrigger("Die");
        }
    }

    private void Move(float move)
    {
        Vector3 targetVelocity = new Vector2(move * 10f, m_rigidBody.velocity.y);//Should be Vector2 instead Vector3
        // And then smoothing it out and applying it to the character
        m_rigidBody.velocity =
            Vector3.SmoothDamp(m_rigidBody.velocity, targetVelocity, ref m_velocity, m_movementSmoothing);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void UpdateState()
    {
        AnimatorClipInfo[] currentState = m_animator.GetCurrentAnimatorClipInfo(0);
        string state = currentState[0].clip.name;
        switch (state)
        {
            case "run"://capitalize case, should be "Run"
                m_state = State.Walk;
                break;
            case "death"://capitalize case, should be "Death"
                m_state = State.Die;
                break;
            case "attack"://capitalize case, should be "Attack"
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

    public void HitCall()
    {
        StartCoroutine(Hit());
    }

    private IEnumerator Hit()
    {
        while (m_state == State.Attack)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_firepointRight.position, m_attackRange, m_layersToHit);

            foreach (Collider2D collider in colliders)
            {
                PlayerControl player = collider.GetComponent<PlayerControl>();
                player.TakeDamage(m_damage);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void setMaterial(PhysicsMaterial2D material)//wrong naming, should be SetMaterial
    {
        m_rigidBody.sharedMaterial = material;
    }
}