using System.Collections;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;

    private bool canMove = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // Delay.
        StartCoroutine(DelayMovement());
    }

    private IEnumerator DelayMovement()
    {
        yield return new WaitForSeconds(4f);
        canMove = true;
    }

    void Update()
    {
        if (!canMove || target == null) return;

        // Relates movement to deltaTime, enables pausing of slime movement.
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}