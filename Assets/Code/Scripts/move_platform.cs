using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_platform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2.0f;

    private Vector3 targetPosition;

    private void Start()
    {
        // Inicializa la posición inicial como el primer objetivo
        targetPosition = endPoint.position;
    }

    private void Update()
    {
        // Mueve la plataforma hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Si la plataforma llega al objetivo, cambia el objetivo
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeTarget();
        }
    }

    private void ChangeTarget()
    {
        // Cambia el objetivo entre el punto de inicio y el punto final
        targetPosition = (targetPosition == startPoint.position) ? endPoint.position : startPoint.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si el jugador colisiona con la plataforma, haz que el jugador sea hijo de la plataforma
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
            collision.transform.gameObject.GetComponent<DashToGoalAgent>().isInPlatform = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Si el jugador deja de colisionar con la plataforma, deja de ser hijo de la plataforma
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform.parent.parent.parent;
            collision.transform.gameObject.GetComponent<DashToGoalAgent>().isInPlatform = false;

        }
    }
}
