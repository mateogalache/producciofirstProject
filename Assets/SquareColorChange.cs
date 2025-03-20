using System.Collections;
using UnityEngine;

public class SquareColorChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float changeInterval = 2f;
    public float whiteDuration = 0.1f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeColorRoutine());
    }

    private IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);

            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(whiteDuration);

            spriteRenderer.color = Color.white;
        }
    }
}
