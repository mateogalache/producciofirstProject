using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Tooltip("Altura a la que se abrirá la puerta")]
    public float openHeight = 2f;
    [Tooltip("Velocidad de desplazamiento (unidades/segundo)")]
    public float moveSpeed = 1f;

    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 openPos;
    private Coroutine movingCoroutine;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        movingCoroutine = StartCoroutine(MoveDoor(transform.position, openPos, () =>
        {
            isOpen = true;
        }));
    }

    // Ya no hacemos nada aquí: la puerta nunca vuelve a cerrarse
    public void CloseDoor()
    {
        // Intencionalmente vacío: una vez abierta, no se cierra
    }

    private IEnumerator MoveDoor(Vector3 from, Vector3 to, System.Action onComplete)
    {
        float dist = Vector3.Distance(from, to);
        float traveled = 0f;

        while (traveled < dist)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, to, step);
            traveled += step;
            yield return null;
        }

        transform.position = to;
        movingCoroutine = null;
        onComplete?.Invoke();
    }
}
