using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator : MonoBehaviour
{
    [SerializeField] float speed;
    Vector2 startPoint;
    [SerializeField] Vector2 endPoint;
    int activations = 0;

    private void Awake()
    {
        startPoint = transform.position;
    }

    private void Update()
    {
        if (activations > 0)
            transform.position = Vector3.MoveTowards(transform.position, startPoint + endPoint, speed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, startPoint, speed * Time.deltaTime);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)endPoint);
    }

    public void AddActivation()
    {
        activations++;
    }
    public void RemoveActivation()
    {
        activations--;
    }
}
