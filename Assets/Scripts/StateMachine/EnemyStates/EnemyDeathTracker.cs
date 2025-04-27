using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathTracker : MonoBehaviour
{
    public System.Action OnEnemyDestroyed;

    private void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke();
    }
}
