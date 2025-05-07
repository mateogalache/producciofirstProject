using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AbsorbEffectController : MonoBehaviour
{
    private ParticleSystem ps;
    private bool hasExploded = false;

    public string nextSceneName = "Level2";
    public float delayBeforeExplosion = 0.1f;
    public float delayBeforeSceneChange = 0.005f;

    public void TriggerAbsorption()
    {
        if (!hasExploded)
            StartCoroutine(AbsorptionSequence());
    }

    IEnumerator AbsorptionSequence()
    {
        ps = GetComponent<ParticleSystem>();
        hasExploded = true;

        // Detener y limpiar partículas activas
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        yield return new WaitForSeconds(delayBeforeExplosion);

        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(1f, 3f); // Partículas más grandes
        main.startSpeed = new ParticleSystem.MinMaxCurve(50f, 80f); // Más alcance
        main.startLifetime = 1.5f;
        main.duration = 1f;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 3f; // Más dispersión inicial

        var emission = ps.emission;
        emission.enabled = true;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 20000) // Muchas partículas
        });

        ps.Play(true);

        yield return new WaitForSeconds(delayBeforeSceneChange);
        SceneManager.LoadScene(nextSceneName);
    }
}
