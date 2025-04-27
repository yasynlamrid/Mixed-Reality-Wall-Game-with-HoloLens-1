using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class LaserEffect : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject laserPrefab; // Prefab du laser
    private GameObject activeLaser; // Référence au laser actif
    public float laserDuration = 0.2f;
    public AudioSource laserAudio;
    public GameObject impactEffectPrefab;

 

    void Start()
    {
        if (laserAudio == null)
        {
            Debug.LogError("Aucun AudioSource assigné pour le laser !");
        }
    }

    void OnEnable()
    {
        // S'inscrire aux événements d'entrée MRTK
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    void OnDisable()
    {
        // Se désinscrire des événements d'entrée MRTK
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        // Réagir à un air tap (clic)
        Debug.Log("Air tap détecté !");
        FireLaser(Camera.main.transform.position, CoreServices.InputSystem.GazeProvider.HitPosition);

        // Consommer l'événement (facultatif)
        eventData.Use();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { } // Pas utilisé ici
    public void OnPointerUp(MixedRealityPointerEventData eventData) { } // Pas utilisé ici
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { } // Pas utilisé ici

    public void FireLaser(Vector3 start, Vector3 end)
    {
        // Déterminer une position légèrement en dessous et à droite de la caméra
        Vector3 offset = new Vector3(0.1f, -0.1f, 0); 
        Vector3 adjustedStart = Camera.main.transform.position + Camera.main.transform.TransformDirection(offset);

        // Effectuer un raycast pour obtenir la normale de la surface à l'impact
        RaycastHit hit;
        Vector3 direction = end - adjustedStart;

        if (Physics.Raycast(adjustedStart, direction, out hit))
        {
            // Obtenez la normale de la surface à l'impact
            Vector3 surfaceNormal = hit.normal;

            // Vérifier si le raycast touche un mur (via Spatial Mapping)
            BaseEnemy enemy = hit.collider.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // Appeler la méthode TakeDamage sur l'ennemi
            }

            // Instancier le prefab du laser
            if (laserPrefab != null)
            {
                if (activeLaser != null)
                {
                    Destroy(activeLaser);
                }

                activeLaser = Instantiate(laserPrefab, adjustedStart, Quaternion.LookRotation(direction));
                activeLaser.transform.localScale = new Vector3(0.05f, 0.05f, Vector3.Distance(adjustedStart, hit.point)); // Ajuster la taille du laser
                Destroy(activeLaser, laserDuration);
            }

            // Lecture de l'audio du laser
            if (laserAudio != null)
            {
                laserAudio.Play();
            }

            // Instancier l'effet d'impact
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(surfaceNormal));
                float offsetDistance = 0.01f;
                impactEffect.transform.position += surfaceNormal * offsetDistance;
                Destroy(impactEffect, 1f);
            }


        }
        else
        {
            // Si le raycast ne touche rien, désactiver le laser actif
            if (activeLaser != null)
            {
                Destroy(activeLaser);
            }
        }
    }

 


   
}
