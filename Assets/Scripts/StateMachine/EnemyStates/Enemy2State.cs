// BaseEnemy.cs
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy2State : EnemyState
{
    public Enemy2State(BaseEnemy enemy) : base(enemy)
    {
        if (!(enemy is Enemy2))
        {
            Debug.LogError("État Enemy2 utilisé avec un mauvais type d'ennemi!");
        }
    }

    public abstract override void Enter();
    public abstract override void Update();
    public abstract override void Exit();
}