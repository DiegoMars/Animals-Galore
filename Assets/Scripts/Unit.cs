using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        TopBarManager.Instance.UpdatePopulation();
    }

    // Update is called once per frame
    public void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }
}