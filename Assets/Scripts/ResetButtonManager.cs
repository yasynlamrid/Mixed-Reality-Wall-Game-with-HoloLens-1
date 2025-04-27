using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class ResetButtonManager : MonoBehaviour
{
    [Header("Button References")]
    public GameObject buttonPrefab; // Le prefab du PressableButton à assigner dans Unity
    private PressableButton resetButton;

    [Header("Game Components")]
    private WorldCursor worldCursor;
    private AnimationPointSpawnManager spawnManager;

    [Header("Button Settings")]
    public Vector3 buttonPosition = new Vector3(0.3f, 0.2f, 0.5f); // Position par défaut
    public Vector3 buttonRotation = new Vector3(0, 0, 0);
    public Vector3 buttonScale = new Vector3(1f, 1f, 1f);

    void Start()
    {
        // Trouver les composants nécessaires
        worldCursor = FindObjectOfType<WorldCursor>();
        spawnManager = FindObjectOfType<AnimationPointSpawnManager>();

        // Créer le bouton
        CreateResetButton();
    }

    void CreateResetButton()
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab non assigné! Veuillez assigner un PressableButton prefab dans l'inspecteur.");
            return;
        }

        // Créer le bouton à partir du prefab
        GameObject buttonObj = Instantiate(buttonPrefab, buttonPosition, Quaternion.Euler(buttonRotation));
        buttonObj.transform.localScale = buttonScale;
        buttonObj.name = "ResetButton";

        // Configurer le bouton
        resetButton = buttonObj.GetComponent<PressableButton>();
        if (resetButton != null)
        {
            resetButton.ButtonPressed.AddListener(ResetGame);

            // Configurer le texte du bouton
            TextMeshPro buttonText = buttonObj.GetComponentInChildren<TextMeshPro>();
            if (buttonText != null)
            {
                buttonText.text = "Reset Game";
                buttonText.fontSize = 0.1f;
                buttonText.color = Color.white;
            }
        }

        // Rendre le bouton enfant de ce GameObject pour une meilleure organisation
        buttonObj.transform.parent = this.transform;
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game appelé!");

        if (worldCursor != null)
        {
            worldCursor.ResetCursors();
            worldCursor.ShowSpatialMesh();
            Debug.Log("WorldCursor réinitialisé");
        }
        else
        {
            Debug.LogWarning("WorldCursor non trouvé!");
        }

        if (spawnManager != null)
        {
            spawnManager.ResetSpawnManager();
            Debug.Log("SpawnManager réinitialisé");
        }
        else
        {
            Debug.LogWarning("SpawnManager non trouvé!");
        }

        if (AnimationPointsManager.Instance != null)
        {
            AnimationPointsManager.Instance.ResetManager();
            Debug.Log("AnimationPointsManager réinitialisé");
        }
        else
        {
            Debug.LogWarning("AnimationPointsManager non trouvé!");
        }
    }

    void OnDisable()
    {
        if (resetButton != null)
        {
            resetButton.ButtonPressed.RemoveListener(ResetGame);
        }
    }
}