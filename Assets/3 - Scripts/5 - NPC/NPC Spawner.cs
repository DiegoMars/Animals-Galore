using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject SpawnNPC(UnitSO UnitSpObj, FactionManager.Factions faction,
                               Vector3 Pos)
    {
        GameObject npcUnit = Instantiate(
            UnitSpObj.prefab,
            Pos,
            Quaternion.identity
        );
        return npcUnit;
    }
}