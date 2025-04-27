using UnityEngine;
using UnityEngine.XR.WSA.Input;
using System.Collections;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;

public class WorldCursor : MonoBehaviour
{
    public GameObject floorCursorPrefab;
    public GameObject wallCursorPrefab;
    public GameObject otherCursorPrefab;
    public GameObject crackEffectPrefab;
    public GameObject postClickWallCursorPrefab;

    public GameObject laserEffectObject;

    public float spawnDistance = 0.001f;
    public float crackDuration = 0.5f;
    public float crackMaxSize = 0.5f;
    public float offsetDistance = 0.1f;

    private GameObject activeCursor;
    private GameObject currentWallCursor;
    private Vector3 lastHitNormal;
    private Vector3 lastHitPoint;
    private GestureRecognizer gestureRecognizer;
    private bool isAnimatingCrack = false;
    private LaserEffect laserEffect;
    private bool hasClicked = false;  // État global pour tous les curseurs
    private IMixedRealitySpatialAwarenessMeshObserver observer;
    public System.Action<Vector3, Vector3> OnWallClicked;


    void Start()
    {
        DisableAllCursors();
        observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();


        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.TappedEvent += OnAirTapped;
        gestureRecognizer.StartCapturingGestures();

        if (crackEffectPrefab == null)
        {
            Debug.LogWarning("Crack effect prefab non assigné - création d'un effet par défaut");
            CreateDefaultCrackEffect();
        }

        if (laserEffectObject != null)
        {
            laserEffect = laserEffectObject.GetComponent<LaserEffect>();
            if (laserEffect == null)
            {
                Debug.LogWarning("LaserEffect n'a pas été trouvé sur laserEffectObject.");
            }
        }

        if (postClickWallCursorPrefab == null)
        {
            Debug.LogWarning("postClickWallCursorPrefab n'est pas assigné. Les curseurs ne pourront pas être transformés.");
        }
    }




    private void HideSpatialMesh()
    {
        if (observer != null)
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
            Debug.Log("Maillage spatial masqué.");
        }
        else
        {
            Debug.LogWarning("Observer de maillage spatial non trouvé lors de la tentative de masquage.");
        }
    }

    public void ShowSpatialMesh()
    {
        if (observer != null)
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
            Debug.Log("Maillage spatial affiché.");
        }
    }


    private void OnAirTapped(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        
        if (currentWallCursor != null && currentWallCursor.activeInHierarchy && !isAnimatingCrack)
        {
            hasClicked = true;  
            CreateCrackEffect();
            TriggerWallClickEvent();
            HideSpatialMesh();
        }
    }

    void Update()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, Mathf.Infinity, 1 << 31))
        {
            lastHitNormal = hitInfo.normal;
            lastHitPoint = hitInfo.point;
            ActivateCursorForSurface(hitInfo);
        }
        else
        {
            DisableAllCursors();
        }
    }


    // active le curseur qui s'adapte à la suface pointée 
    private void ActivateCursorForSurface(RaycastHit hitInfo)
    {
        DisableAllCursors();
        Vector3 normal = hitInfo.normal;

        if (hasClicked && postClickWallCursorPrefab != null)
        {
            activeCursor = Instantiate(postClickWallCursorPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, normal));
            currentWallCursor = activeCursor;  
        }
        else
        {
            
            if (Vector3.Dot(normal, Vector3.up) > 0.9f)
            {
                activeCursor = Instantiate(floorCursorPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, normal));
            }
            else if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.1f)
            {
                activeCursor = Instantiate(wallCursorPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, normal));
                currentWallCursor = activeCursor;
            }
            else
            {
                activeCursor = Instantiate(otherCursorPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, normal));
            }
        }

        if (activeCursor != null)
        {
            activeCursor.SetActive(true);
        }
    }

    private void DisableAllCursors()
    {
        if (activeCursor != null)
        {
            Destroy(activeCursor);
        }
    }

   
    public void ResetCursors()
    {
        hasClicked = false;
        DisableAllCursors();
    }

    // Les autres méthodes (CreateCrackEffect, AnimateCrackEffect, etc.) restent inchangées
    private void CreateDefaultCrackEffect()
    {
        GameObject defaultEffect = new GameObject("DefaultCrackEffect");
        ParticleSystem particleSystem = defaultEffect.AddComponent<ParticleSystem>();

        particleSystem.Stop();

        var main = particleSystem.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 2f;
        main.startSpeed = 0.1f;
        main.startSize = 0.05f;
        main.maxParticles = 50;
        main.playOnAwake = false;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 30)
        });

        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;
        shape.radiusThickness = 1f;

        var renderer = defaultEffect.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.normalDirection = 1;

        defaultEffect.SetActive(false);
        crackEffectPrefab = defaultEffect;
    }

    private void CreateCrackEffect()
    {
        if (crackEffectPrefab == null)
        {
            Debug.LogError("crackEffectPrefab est null. Impossible de créer l'effet de fissure.");
            return;
        }

        
        Vector3 offset = GenerateOffset();
     
        Vector3 spawnPosition = lastHitPoint + offset + lastHitNormal * spawnDistance;

        // Enregistrez les points d'animation pour l'animation de fissure
        if (AnimationPointsManager.Instance != null)
        {
            // Le point de début de l'animation est celui où l'effet de fissure sera généré.
            Vector3 laserEnd = spawnPosition; 
            Vector3 laserStart = Camera.main.transform.position; 

            // Envoi des points d'animation (spawnPosition, laserStart, laserEnd, et lastHitNormal)
            AnimationPointsManager.Instance.RegisterAnimationPoints(
                spawnPosition,    // Position de spawn pour l'animation de fissure
                laserStart,       // Point de départ du laser
                laserEnd,         // Point de fin du laser (position de l'animation)
                lastHitNormal     // Normale de la surface
            );
        }

        
        GameObject crackEffect = Instantiate(crackEffectPrefab, spawnPosition, Quaternion.identity);
        crackEffect.SetActive(true);
        crackEffect.transform.rotation = Quaternion.LookRotation(lastHitNormal);

        
        StartCoroutine(AnimateCrackEffect(crackEffect));

       
        if (laserEffectObject != null && laserEffect != null)
        {
            laserEffectObject.SetActive(true);
            Vector3 direction = (lastHitPoint - Camera.main.transform.position).normalized;
            laserEffect.FireLaser(Camera.main.transform.position, direction);
        }
    }


    private Vector3 GenerateOffset()
    {
        Vector3 tangent1 = Vector3.Cross(lastHitNormal, Vector3.up).normalized;
        if (tangent1.magnitude < 0.01f)
        {
            tangent1 = Vector3.Cross(lastHitNormal, Vector3.right).normalized;
        }

        Vector3 tangent2 = Vector3.Cross(lastHitNormal, tangent1).normalized;
        Vector2 randomCircle = Random.insideUnitCircle * offsetDistance;

        return (tangent1 * randomCircle.x + tangent2 * randomCircle.y);
    }

    private IEnumerator AnimateCrackEffect(GameObject crack)
    {
        isAnimatingCrack = true;
        ParticleSystem particleSystem = crack.GetComponent<ParticleSystem>();
        float elapsedTime = 0f;

        crack.transform.localScale = Vector3.zero;

        while (elapsedTime < crackDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / crackDuration;
            float currentSize = Mathf.SmoothStep(0, crackMaxSize, progress);
            crack.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            yield return null;
        }

        if (particleSystem != null)
        {
            particleSystem.Play(true);

            while (particleSystem.IsAlive(true))
            {
                yield return null;
            }
        }

        Destroy(crack);
        isAnimatingCrack = false;
    }

    private void TriggerWallClickEvent()
    {
        Debug.Log("Effet de fissure créé sur le mur !");
        OnWallClicked?.Invoke(lastHitPoint, lastHitNormal);
    }

    void OnDestroy()
    {
        if (gestureRecognizer != null)
        {
            gestureRecognizer.TappedEvent -= OnAirTapped;
            gestureRecognizer.Dispose();
        }
    }
}