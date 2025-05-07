using UnityEngine;
using System.Collections;

public class MoveToTrigger : MonoBehaviour
{
    public float speed = 5f;
    public bool moveRight = true;

    private bool isMoving = true;
    private bool isBeingAbsorbed = false;
    private Transform mirrorTransform;

    private float spiralShrinkSpeed = 2f;
    private float scaleShrinkSpeed = 2f;
    private float selfRotationSpeed = 360f;
    private float orbitSpeed = 5f;

    private float orbitRadius = 1f;
    private float orbitAngle = 0f;

    private bool exploded = false;
    private GameObject explosionEffect;

    void Start()
    {
        explosionEffect = GameObject.Find("ExplosionMirror");
    }

    void Update()
    {
        if (isMoving)
        {
            Vector2 direction = moveRight ? Vector2.right : Vector2.left;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else if (isBeingAbsorbed && mirrorTransform != null)
        {
            transform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);

            orbitAngle += orbitSpeed * Time.deltaTime;
            orbitRadius = Mathf.Max(0f, orbitRadius - spiralShrinkSpeed * Time.deltaTime);

            float x = Mathf.Cos(orbitAngle) * orbitRadius;
            float y = Mathf.Sin(orbitAngle) * orbitRadius;

            transform.position = mirrorTransform.position + new Vector3(x, y, 0f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, scaleShrinkSpeed * Time.deltaTime);

            // Lanza la explosión antes de llegar al centro
            if (!exploded && orbitRadius <= 7f && transform.localScale.magnitude < 3f)
            {
                exploded = true;

                if (explosionEffect != null)
                {
                    explosionEffect.GetComponent<AbsorbEffectController>().TriggerAbsorption();
                }

                StartCoroutine(DestroyAfterDelay(0.1f)); // Esperar un poco para ver la explosión
            }

            if (orbitRadius <= 0.05f && transform.localScale.magnitude < 0.05f && !exploded)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mirror"))
        {
            isMoving = false;
            isBeingAbsorbed = true;
            mirrorTransform = other.transform;

            orbitRadius = Vector3.Distance(transform.position, mirrorTransform.position);
            Vector3 dir = transform.position - mirrorTransform.position;
            orbitAngle = Mathf.Atan2(dir.y, dir.x);

            Debug.Log("Absorción en espiral iniciada.");
        }
    }
}
