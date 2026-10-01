using UnityEngine;

public class AIEnemyInputProvider : MonoBehaviour, IInputProvider
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack Ranges")]
    [SerializeField] private float basicRange = 4f;
    [SerializeField] private float clawRange = 6f;
    [SerializeField] private float flameRange = 10f;
    [SerializeField] private float flyRange = 15f;

    [Header("Movement")]
    [SerializeField] private float preferredRange = 7f;
    [SerializeField] private float attackSpeedFactor = 0.6f;
    [SerializeField] private float attackChangeInterval = 1.5f;
    [SerializeField] private Vector3 arenaCenter = Vector3.zero;
    [SerializeField] private float arenaRadius = 18f;
    [SerializeField] private float edgeMargin = 3f;

    [Header("Decision Timing")]
    [SerializeField] private float decisionInterval = 0.3f;
    [SerializeField, Range(0f, 1f)] private float attackChancePerDecision = 0.65f;

    private Vector2 movementInput;
    private bool attack1Queued, attack2Queued, attack3Queued, attack4Queued;

    private float nextDecisionTime;
    private float nextAttackTime;
    private int strafeDir = 1;

    private DragonController dragon;
    private DragonController targetDragon;

    public Vector2 MovementInput => movementInput;

    public bool IsAttacking1
    {
        get { bool r = attack1Queued; attack1Queued = false; return r; }
    }
    public bool IsAttacking2
    {
        get { bool r = attack2Queued; attack2Queued = false; return r; }
    }
    public bool IsAttacking3
    {
        get { bool r = attack3Queued; attack3Queued = false; return r; }
    }
    public bool IsAttacking4
    {
        get { bool r = attack4Queued; attack4Queued = false; return r; }
    }

    private void Awake()
    {
        dragon = GetComponent<DragonController>();
    }

    private void Start()
    {
        if (target != null)
        {
            targetDragon = target.GetComponent<DragonController>();
        }
    }

    private void Update()
    {
        if (target == null)
        {
            movementInput = Vector2.zero;
            return;
        }

        if (targetDragon == null && target != null)
        {
            targetDragon = target.GetComponent<DragonController>();
        }

        UpdateMovement();

        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + decisionInterval;
            DecideAttack();
        }
    }

    private void UpdateMovement()
    {
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;
        
        //Debug.Log(dist);
        
        if (dist <= basicRange)
        {
            movementInput = Vector2.zero;
            return;
        }

        Vector3 dirToTarget = dist > 0.001f ? toTarget / dist : Vector3.forward;

        Vector3 approachComponent;
        if (dist > preferredRange + 1f) approachComponent = dirToTarget;
        else if (dist < preferredRange - 1f) approachComponent = -dirToTarget;
        else approachComponent = Vector3.zero;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackChangeInterval;
            strafeDir = Random.value > 0.5f ? 1 : -1;
        }
        Vector3 perpendicular = Vector3.Cross(Vector3.up, dirToTarget);
        Vector3 strafeComponent = perpendicular * strafeDir * attackSpeedFactor;

        Vector3 desired = approachComponent + strafeComponent;

        Vector3 posFlat = transform.position;
        posFlat.y = arenaCenter.y;
        float distFromCenter = Vector3.Distance(posFlat, arenaCenter);
        if (distFromCenter > arenaRadius - edgeMargin)
        {
            Vector3 towardCenter = (arenaCenter - posFlat).normalized;
            desired = Vector3.Lerp(desired.normalized, towardCenter, 0.7f);
        }

        if (desired.sqrMagnitude > 1f) desired.Normalize();
        movementInput = new Vector2(desired.x, desired.z);
    }

    private void DecideAttack()
    {
        if (attack1Queued || attack2Queued || attack3Queued || attack4Queued) return;
        if (dragon == null) return;

        if (targetDragon != null && (targetDragon.state == State.Attacking || targetDragon.state == State.Hit)) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (Random.value > attackChancePerDecision) return;

        if (dist <= basicRange)
        {
            bool basicReady = dragon.IsBasicReady;
            bool clawReady = dragon.IsClawReady;

            if (basicReady && clawReady)
            {
                if (Random.value > 0.5f) attack1Queued = true;
                else attack2Queued = true;
            }
            else if (basicReady) attack1Queued = true;
            else if (clawReady) attack2Queued = true;
        }
        else if (dist <= clawRange && dragon.IsClawReady)
        {
            attack2Queued = true;
        }
        else if (dist <= flameRange && dragon.IsFlameReady)
        {
            attack3Queued = true;
        }
        else if (dist <= flyRange && dragon.IsFlyReady)
        {
            attack4Queued = true;
        }
    }
}