using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    private bool navMeshBuilt = false;
    private WorldCursor worldCursor;

    void Start()
    {
        // Récupérer la référence du WorldCursor
        worldCursor = FindObjectOfType<WorldCursor>();
        if (worldCursor != null)
        {
            // S'abonner à l'événement OnWallClicked
            worldCursor.OnWallClicked += OnWallClicked;
        }
        else
        {
            Debug.LogError("WorldCursor non trouvé dans la scène!");
        }
    }

    private void OnWallClicked(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Construire le NavMesh uniquement s'il n'a pas déjà été construit
        if (!navMeshBuilt)
        {
            BuildNavMesh();
            navMeshBuilt = true;
        }
    }

    public void BuildNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.collectObjects = CollectObjects.All;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh construit après le clic sur le mur !");
        }
        else
        {
            Debug.LogError("NavMeshSurface n'est pas assigné !");
        }
    }

    void OnDestroy()
    {
        // Se désabonner de l'événement pour éviter les fuites de mémoire
        if (worldCursor != null)
        {
            worldCursor.OnWallClicked -= OnWallClicked;
        }
    }
}