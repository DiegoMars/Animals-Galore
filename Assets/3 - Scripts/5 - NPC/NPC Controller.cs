using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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

        float distance = Vector3.Distance( transform.position, target.transform.position);

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
        float distance = Vector3.Distance( transform.position, target.transform.position);
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
        throw new NotImplementedException();
    }
}
