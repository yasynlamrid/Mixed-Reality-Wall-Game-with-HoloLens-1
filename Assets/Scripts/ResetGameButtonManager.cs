using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using System.Linq;

public class ResetGameButtonManager : MonoBehaviour
{
    [Header("Button References")]
    public GameObject buttonPrefab;
    private PressableButton resetButton;

    [Header("Button Settings")]
    public Vector3 buttonPosition = new Vector3(0.3f, 0.2f, 0.5f);
    public Vector3 buttonRotation = new Vector3(0, 0, 0);
    public Vector3 buttonScale = new Vector3(1f, 1f, 1f);

    private WorldCursor worldCursor;
    private AnimationPointSpawnManager spawnManager;
    private AnimationPointsManager animationPointsManager;

    void Start()
    {
        FindGameComponents();
        CreateResetButton();
    }

    void FindGameComponents()
    {
        worldCursor = FindObjectOfType<WorldCursor>();
        if (worldCursor == null)
            Debug.LogError("WorldCursor non trouvé dans la scène!");

        spawnManager = FindObjectOfType<AnimationPointSpawnManager>();
        if (spawnManager == null)
            Debug.LogError("AnimationPointSpawnManager non trouvé dans la scène!");

        animationPointsManager = AnimationPointsManager.Instance;
        if (animationPointsManager == null)
            Debug.LogError("AnimationPointsManager non trouvé dans la scène!");
    }

    void CreateResetButton()
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab manquant!");
            return;
        }

        GameObject buttonObj = Instantiate(buttonPrefab, buttonPosition, Quaternion.Euler(buttonRotation));
        buttonObj.transform.localScale = buttonScale;
        buttonObj.name = "ResetButton";
        buttonObj.transform.parent = transform;

        resetButton = buttonObj.GetComponent<PressableButton>();
        if (resetButton != null)
        {
            resetButton.ButtonPressed.AddListener(RetournerASelectionMur);

            TextMeshPro buttonText = buttonObj.GetComponentInChildren<TextMeshPro>();
            if (buttonText != null)
            {
                buttonText.text = "Choisir Mur";
                buttonText.fontSize = 0.1f;
                buttonText.color = Color.white;
            }
        }
    }

    public void RetournerASelectionMur()
    {
        Debug.Log("Retour à la sélection du mur...");

        // Nettoyer toutes les animations et effets visuels
        CleanupAllEffects();

        // Réinitialiser le WorldCursor
        if (worldCursor != null)
        {
            worldCursor.ResetCursors();
            worldCursor.ShowSpatialMesh();

            // Réinitialiser les flags
            var hasClickedField = worldCursor.GetType().GetField("hasClicked",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (hasClickedField != null)
            {
                hasClickedField.SetValue(worldCursor, false);
            }

            var animatingField = worldCursor.GetType().GetField("isAnimatingCrack",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (animatingField != null)
            {
                animatingField.SetValue(worldCursor, false);
            }
        }

        // Nettoyer le SpawnManager
        if (spawnManager != null)
        {
            spawnManager.ResetSpawnManager();
        }

        // Réinitialiser l'AnimationPointsManager
        if (animationPointsManager != null)
        {
            animationPointsManager.ResetManager();
        }

        Debug.Log("Le jeu est prêt pour la sélection d'un nouveau mur!");
    }

    private void CleanupAllEffects()
    {
        // Nettoyer les effets de fissures
        var crackEffects = FindObjectsOfType<ParticleSystem>();
        foreach (var effect in crackEffects)
        {
            if (effect != null && effect.gameObject != null)
            {
                Destroy(effect.gameObject);
            }
        }

        // Nettoyer les effets de laser
        if (worldCursor != null && worldCursor.laserEffectObject != null)
        {
            worldCursor.laserEffectObject.SetActive(false);
        }

        // Chercher et détruire tous les objets avec "crack" ou "effect" dans leur nom
        var effectObjects = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(go => (go.name.ToLower().Contains("crack") ||
                         go.name.ToLower().Contains("effect")) &&
                         go.scene.isLoaded);

        foreach (var obj in effectObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    void OnDisable()
    {
        if (resetButton != null)
        {
            resetButton.ButtonPressed.RemoveListener(RetournerASelectionMur);
        }
    }
}