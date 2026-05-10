using Unity.VisualScripting;
using UnityEngine;

public class PlayerEncounter : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    void OnCollisionEnter(Collision collision)
    {
        combatManager.SetEnemyToList(gameObject.GetComponent<BaseCharacter>());
        combatManager.StartCombat();
    }
}