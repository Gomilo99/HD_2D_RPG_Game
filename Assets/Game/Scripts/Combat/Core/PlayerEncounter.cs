using Unity.VisualScripting;
using UnityEngine;

public class PlayerEncounter : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            combatManager.SetEnemyToList(collision.gameObject.GetComponent<BaseCharacter>());
            combatManager.StartCombat();
        }
        
    }
}