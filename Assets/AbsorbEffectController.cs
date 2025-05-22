using UnityEngine;
using System.Collections;

public class AbsorbEffectController : MonoBehaviour
{
    private ParticleSystem ps;
    private bool hasExploded = false;

    [Header("Transición")]
    public string nextSceneName = "Level2";
    public float delayBeforeExplosion = 0.5f;
    public float delayBeforeSceneChange = 1.5f;

    public void TriggerAbsorption()
    {
        if (!hasExploded)
        {
            Debug.Log("TriggerAbsorption llamado");
            StartCoroutine(AbsorptionSequence());
        }
    }

    IEnumerator AbsorptionSequence()
    {
        ps = GetComponent<ParticleSystem>();
        hasExploded = true;

        // Detener cualquier emisión actual
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        yield return new WaitForSeconds(delayBeforeExplosion);

        // Configurar nueva explosión
        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(50f, 80f);
        main.startLifetime = 1.5f;
        main.duration = 1f;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 3f;

        var emission = ps.emission;
        emission.enabled = true;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 20000)
        });

        ps.Play(true);

        // Esperar y luego cargar nueva escena con fade
        yield return new WaitForSeconds(delayBeforeSceneChange);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithFade(nextSceneName, 0f);
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager no encontrado.");
        }
    }
}
