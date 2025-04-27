using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour
{


   
    [Header("Projectile Settings")]
    public float speed = 1f;
    public float damage = 10f;
    public float lifetime = 3f;

    [Header("Audio Settings")]
    public AudioClip projectileSound;
    public AudioClip hitPlayerSound; // Nouveau son pour collision avec joueur
    public float maxSoundDistance = 20f;
    public float volumeVariation = 1f;

    private Vector3 direction;
    private AudioSource audioSource;
    private Transform mainCameraTransform;
    private bool hasHitPlayer = false;


    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        audioSource = gameObject.AddComponent<AudioSource>();
        SetupAudioSource();

        // Ajouter un BoxCollider à la caméra si nécessaire
        if (mainCameraTransform != null && mainCameraTransform.GetComponent<BoxCollider>() == null)
        {
            BoxCollider cameraCollider = mainCameraTransform.gameObject.AddComponent<BoxCollider>();
            cameraCollider.isTrigger = true;
            cameraCollider.size = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private void SetupAudioSource()
    {
        if (audioSource != null)
        {
            audioSource.clip = projectileSound;
            audioSource.spatialBlend = 1.0f;
            audioSource.spatialize = true;
            audioSource.volume = 1f;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("AudioSource couldn't be added to projectile");
        }
    }

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        if (audioSource != null && projectileSound != null)
        {
            PlaySpatialSound();
        }
        Destroy(gameObject, lifetime);
    }

    private void PlaySpatialSound()
    {
        Vector3 projectileToCamera = mainCameraTransform.position - transform.position;
        float angle = Vector3.SignedAngle(mainCameraTransform.forward, projectileToCamera, Vector3.up);
        float panValue = Mathf.Clamp(angle / 90f, -1f, 1f);

        float leftVolume = Mathf.Clamp01(1f - panValue);
        float rightVolume = Mathf.Clamp01(1f + panValue);
        leftVolume *= volumeVariation;
        rightVolume *= volumeVariation;

        audioSource.panStereo = panValue;
        audioSource.volume = Mathf.Max(leftVolume, rightVolume);
        audioSource.Play();
    }

    private void PlayHitPlayerSound()
    {
        if (hitPlayerSound != null && !hasHitPlayer)
        {
            hasHitPlayer = true;
            audioSource.clip = hitPlayerSound;
            audioSource.spatialBlend = 0f; // Son non spatial pour l'alerte
            audioSource.volume = 1f;
            audioSource.Play();
            StartCoroutine(DestroyAfterSound());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(hitPlayerSound != null ? hitPlayerSound.length : 0.1f);
        Destroy(gameObject);
    }

    void Update()
    {
        if (!hasHitPlayer)
        {
            transform.position += direction * speed * Time.deltaTime;
            if (audioSource != null && audioSource.isPlaying)
            {
                UpdateSpatialSound();
            }
        }
    }

    private void UpdateSpatialSound()
    {
        Vector3 projectileToCamera = mainCameraTransform.position - transform.position;
        float angle = Vector3.SignedAngle(mainCameraTransform.forward, projectileToCamera, Vector3.up);
        float panValue = Mathf.Clamp(angle / 90f, -1f, 1f);
        audioSource.panStereo = panValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mainCameraTransform.gameObject)
        {
            Debug.Log("Projectile hit player!");
            PlayHitPlayerSound();
        }
        else if (!hasHitPlayer)
        {
            Destroy(gameObject);
        }
    }
}