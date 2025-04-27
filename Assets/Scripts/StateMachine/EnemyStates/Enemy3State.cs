
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy3State : EnemyState
{
    public Enemy3State(BaseEnemy enemy) : base(enemy)
    {
        if (!(enemy is Enemy3))
        {
            Debug.LogError("État Enemy3 utilisé avec un mauvais type d'ennemi!");
        }
    }

    public abstract override void Enter();
    public abstract override void Update();
    public abstract override void Exit();
}