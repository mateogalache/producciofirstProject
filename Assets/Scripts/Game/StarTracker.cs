using UnityEngine;
using System.Collections.Generic;

public class StarTracker : MonoBehaviour
{
    public static StarTracker Instance;

    [Header("Progreso de estrellas")]
    public int collectedStars = 0;
    public int maxStars = 14;

    private HashSet<string> collectedStarIDs = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddStar(string starID)
    {
        if (!collectedStarIDs.Contains(starID))
        {
            collectedStarIDs.Add(starID);
            collectedStars++;
            Debug.Log($"[StarTracker] AÑADIDA: {starID} Total recogidas: {collectedStars}");
            return true;
        }

        Debug.Log($"[StarTracker] YA RECOGIDA: {starID}");
        return false;
    }

    public bool HasCollected(string starID)
    {
        return collectedStarIDs.Contains(starID);
    }

    public void ResetStars()
    {
        collectedStars = 0;
        collectedStarIDs.Clear();
        Debug.Log("[StarTracker] Progreso reiniciado.");
    }
}
