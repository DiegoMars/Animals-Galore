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

    public bool IsPlayer(Factions faction)
    {
        return faction == PlayerFaction;
    }
}