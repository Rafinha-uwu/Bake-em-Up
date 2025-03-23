using UnityEngine;
using System.Collections;

public class ZombieAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 2f;
    private bool isAttacking;

    public float attackRange = 2f;
    public bool roulotteInAttackRange;

    public LayerMask whatIsRoulotte;
    public int attackDamage = 10;

    private void Update()
    {
        roulotteInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsRoulotte);

        if (roulotteInAttackRange && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsRoulotte);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.name);
            HitEvent.GetHit(attackDamage, gameObject, hitCollider.gameObject);
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        isAttacking = false;
    }


}
