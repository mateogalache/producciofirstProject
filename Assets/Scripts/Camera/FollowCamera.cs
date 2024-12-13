using UnityEngine;
public class FollowCamera : MonoBehaviour
{
    // Referencia al GameObject que la c�mara debe seguir
    public Transform target;

    // Distancia de la c�mara respecto al objetivo
    public Vector3 offset = new Vector3(0, 5, -10);

    // Velocidad de seguimiento para suavizar el movimiento de la c�mara
    public float followSpeed = 5f;

    void Start()
    {
        // Si no se ha asignado un objetivo, busca uno con la etiqueta "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("No se encontr� un objeto con la etiqueta 'Player' para que la c�mara lo siga.");
            }
        }
    }

    void LateUpdate()
    {
        // Solo sigue el objetivo si se ha asignado
        if (target != null)
        {
            // Mantener las posiciones actuales en Y y Z
            Vector3 currentPosition = transform.position;

            // Calcular la posici�n objetivo en X, manteniendo Y y Z sin cambios
            float targetX = Mathf.Lerp(currentPosition.x, target.position.x + offset.x, followSpeed * Time.deltaTime);

            // Actualizar la posici�n de la c�mara
            transform.position = new Vector3(targetX, currentPosition.y, currentPosition.z);
        }
    }
}