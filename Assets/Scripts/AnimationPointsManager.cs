using UnityEngine;

public class AnimationPointsManager : MonoBehaviour
{
    public static AnimationPointsManager Instance { get; private set; }

    private Vector3 spawnPosition;
    private Vector3 laserStart;
    private Vector3 laserEnd;
    private Vector3 surfaceNormal;

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






    public void ResetManager()
    {
        // Réinitialiser toutes les variables à leurs valeurs par défaut
        spawnPosition = Vector3.zero;
        laserStart = Vector3.zero;
        laserEnd = Vector3.zero;
        surfaceNormal = Vector3.zero;
        Debug.Log("AnimationPointsManager réinitialisé");
    }







    public void RegisterAnimationPoints(Vector3 spawn, Vector3 lStart, Vector3 lEnd, Vector3 normal)
    {
        spawnPosition = spawn;
        laserStart = lStart;
        laserEnd = lEnd;
        surfaceNormal = normal;

        Debug.Log($"Animation points registered - Spawn: {spawn}, Normal: {normal}");
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

    public Vector3 GetLaserStart()
    {
        return laserStart;
    }

    public Vector3 GetLaserEnd()
    {
        return laserEnd;
    }

    public Vector3 GetSurfaceNormal()
    {
        return surfaceNormal;
    }
}