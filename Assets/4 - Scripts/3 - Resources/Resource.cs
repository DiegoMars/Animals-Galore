using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] public Animator berryAnimator;
    
    internal void TriggerCollection()
    {
        Debug.Log("[Resource] Triggering Animation");
        berryAnimator.SetTrigger("Collecting");
    }
}
