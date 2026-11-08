using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance {get; set;}

    public enum Factions
    {
        BUGS,
        BIRDS,
        REPTILES,
        MAMMALS
    }

    public Factions PlayerFaction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // NEED TO CHANGE HOW THIS WORKS SOMEHOW
        InitFactionInManagers(Factions.BUGS);
        InitFactionInManagers(Factions.REPTILES);
    }

    private void InitFactionInManagers(Factions faction)
    {
        ResourceManager.Instance.InitFactionResourceState(faction);
        BuildManager.Instance.InitFactionBuildingList(faction);
    }

    public bool IsPlayer(Factions faction)
    {
        return faction == PlayerFaction;
    }

    public bool AreHostile(Factions first, Factions second)
    {
        return first != second;
    }
}