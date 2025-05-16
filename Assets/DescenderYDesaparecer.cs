using UnityEngine;

public class DescenderYDesaparecer : MonoBehaviour
{
    public float velocidadDescenso = 2f;
    public float distanciaADesaparecer = 5f;

    private bool debeBajar = false;
    private float alturaInicial;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            debeBajar = true;
            alturaInicial = transform.position.y;
        }
    }

    void Update()
    {
        if (debeBajar)
        {
            transform.position -= Vector3.up * velocidadDescenso * Time.deltaTime;

            if (alturaInicial - transform.position.y >= distanciaADesaparecer)
            {
                Destroy(gameObject); // Desaparece el objeto
            }
        }
    }
}
