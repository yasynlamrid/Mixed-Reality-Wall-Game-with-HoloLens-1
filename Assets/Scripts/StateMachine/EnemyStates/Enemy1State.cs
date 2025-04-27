
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy1State : EnemyState
{
    public Enemy1State(BaseEnemy enemy) : base(enemy)
    {
        if (!(enemy is Enemy1))
        {
            Debug.LogError("État Enemy1 utilisé avec un mauvais type d'ennemi!");
        }
    }

    public abstract override void Enter();
    public abstract override void Update();
    public abstract override void Exit();
}