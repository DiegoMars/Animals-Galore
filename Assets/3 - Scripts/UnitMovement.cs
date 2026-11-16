using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    public NavMeshAgent agent;
    public LayerMask ground;
    public LayerMask clickable;
    public bool Selected = false;

    private Unit ParentUnit;
    private bool harvestor;
    private bool IsPlayer;

    private Unit attackUnitTarget;
    private Building attackBuildingTarget;
    private float nextAttackTime;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        ParentUnit = this.GetComponentInParent<Unit>();
        harvestor = ParentUnit.Harvestor;
        IsPlayer = FactionManager.Instance.IsPlayer(this.GetComponentInParent<Unit>().Faction);
    }

    private void Update()
    {
        if (!IsPlayer)
            return;
        if (Selected && Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
        UpdateAttack();
    }

    private void HandleRightClick()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            ResourceProducer resource = hit.collider.GetComponentInParent<ResourceProducer>();
            if (harvestor && resource != null)
            {
                if (ResourceManager.Instance.IsResourceDiscovered(ParentUnit.Faction, resource))
                {
                    ClearAttackTarget();
                    ParentUnit.StartCollection(resource);
                }
                return;
            }

            Unit unitTarget = hit.collider.GetComponentInParent<Unit>();
            if (unitTarget != null && unitTarget.Faction != ParentUnit.Faction)
            {
                SetAttackTarget(unitTarget);
                return;
            }

            Building buildingTarget = hit.collider.GetComponentInParent<Building>();
            if (buildingTarget != null && buildingTarget.Faction != ParentUnit.Faction)
            {
                SetAttackTarget(buildingTarget);
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
        {
            ClearAttackTarget();
            agent.SetDestination(hit.point);
        }
    }

    private void UpdateAttack()
    {
        if (attackUnitTarget == null && attackBuildingTarget == null)
        {
            return;
        }

        Transform targetTransform;

        if (attackUnitTarget != null)
        {
            targetTransform = attackUnitTarget.transform;
        }
        else
        {
            targetTransform = attackBuildingTarget.transform;
        }

        float distance = Vector3.Distance(
            ParentUnit.transform.position,
            targetTransform.position
        );
        if (distance > ParentUnit.unitInfo.AttackRange)
        {
            agent.SetDestination(targetTransform.position);
            return;
        }
        agent.ResetPath();

        if (Time.time < nextAttackTime)
            return;
        if (attackUnitTarget != null)
        {
            ParentUnit.Attack(attackUnitTarget);
        }
        else
        {
            ParentUnit.Attack(attackBuildingTarget);
        }
        nextAttackTime = Time.time + ParentUnit.unitInfo.AttackCooldown;
    }

    private void ClearAttackTarget()
    {
        if (attackUnitTarget != null)
            SetAttackIndicator(attackUnitTarget.gameObject, false);
        if (attackBuildingTarget != null)
            SetAttackIndicator(attackBuildingTarget.gameObject, false);
        attackUnitTarget = null;
        attackBuildingTarget = null;
        nextAttackTime = 0f;
    }

    private void SetAttackTarget(Unit target)
    {
        ClearAttackTarget();
        attackUnitTarget = target;
        UpdateAttackIndicator();

        agent.SetDestination(target.transform.position);
    }
    private void SetAttackTarget(Building target)
    {
        ClearAttackTarget();
        attackBuildingTarget = target;
        UpdateAttackIndicator();

        agent.SetDestination(target.transform.position);
    }
    private void SetAttackIndicator(GameObject target, bool value)
    {
        if (target == null)
            return;
        Transform indicator = target.transform.Find("AttackTargetIndicator");
        if (indicator != null)
            indicator.gameObject.SetActive(value);
    }
    public void UpdateAttackIndicator()
    {
        if (attackUnitTarget != null)
        {
            SetAttackIndicator(attackUnitTarget.gameObject, Selected);
        }
        else if (attackBuildingTarget != null)
        {
            SetAttackIndicator(attackBuildingTarget.gameObject, Selected);
        }
    }
}