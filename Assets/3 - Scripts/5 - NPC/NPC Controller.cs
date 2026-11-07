using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public enum NPCState
    {
        Patrolling,
        Chasing,
        Attacking,
        Returning
    }

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public int patrolIndex;
    public NPCState State = NPCState.Patrolling;
    private Unit unit;
    private NavMeshAgent UnitAgent;
    private Unit target;
    private float nextAttackTime;

    void Awake()
    {
        unit = this.GetComponentInParent<Unit>();
        UnitAgent = this.GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        if (unit.PlayerControlled)
            return;
        if (unit.Harvestor)
            return;

        switch (State)
        {
            case NPCState.Patrolling:
                Patrol();
                break;
            case NPCState.Chasing:
                Chase();
                break;
            case NPCState.Attacking:
                Attack();
                break;
            case NPCState.Returning:
                ReturnToPatrol();
                break;
        }
    }

    private void Patrol()
    {
        Unit detectedUnit = FindHostileUnit();

        if (detectedUnit != null)
        {
            target = detectedUnit;
            State = NPCState.Chasing;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (UnitAgent.pathPending || 
            UnitAgent.remainingDistance > UnitAgent.stoppingDistance)
            return;

        UnitAgent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    private void Chase()
    {
        if (target == null)
        {
            State = NPCState.Returning;
            return;
        }

        float distance = Vector3.Distance(unit.transform.position, target.transform.position);

        if (distance <= unit.unitInfo.AttackRange)
        {
            UnitAgent.ResetPath();
            State = NPCState.Attacking;
            return;
        }

        if (distance > unit.unitInfo.DetectionRange * 1.5f)
        {
            target = null;
            State = NPCState.Returning;
            return;
        }

        UnitAgent.SetDestination(target.transform.position);
    }

    private void Attack()
    {
        if (target == null)
        {
            State = NPCState.Returning;
            return;
        }
        float distance = Vector3.Distance(unit.transform.position, target.transform.position);
        if (distance > unit.unitInfo.AttackRange)
        {
            State = NPCState.Chasing;
            return;
        }
        UnitAgent.ResetPath();
        if (Time.time >= nextAttackTime)
        {
            unit.Attack(target);
            nextAttackTime = Time.time + unit.unitInfo.AttackCooldown;
        }
    }

    private void ReturnToPatrol()
    {
        Unit detectedUnit = FindHostileUnit();

        if (detectedUnit != null)
        {
            target = detectedUnit;
            State = NPCState.Chasing;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            State = NPCState.Patrolling;
            return;
        }
        UnitAgent.SetDestination(patrolPoints[patrolIndex].position);
        if (!UnitAgent.pathPending && UnitAgent.remainingDistance <= UnitAgent.stoppingDistance)
        {
            State = NPCState.Patrolling;
        }
    }

    private Unit FindHostileUnit()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(
            unit.transform.position,
            unit.unitInfo.DetectionRange,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        Unit closestUnit = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Unit possibleUnit = nearbyCollider.GetComponentInParent<Unit>();
            if (possibleUnit == null || possibleUnit == unit || possibleUnit.CurrentHealth <= 0)
            {
                continue;
            }

            if (!FactionManager.Instance.AreHostile(
                unit.Faction,
                possibleUnit.Faction
            ))
            {
                continue;
            }

            float distance = Vector3.Distance(
                unit.transform.position,
                possibleUnit.transform.position
            );
        
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestUnit = possibleUnit;
            }

        }
        return closestUnit;
    }

    private void OnDrawGizmosSelected()
    {
        if (unit == null || unit.unitInfo == null)        
            return;
    
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            unit.transform.position,
            unit.unitInfo.DetectionRange
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            unit.transform.position,
            unit.unitInfo.AttackRange
        );
    }
}