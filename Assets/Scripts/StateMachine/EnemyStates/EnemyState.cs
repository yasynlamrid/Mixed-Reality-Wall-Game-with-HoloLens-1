// EnemyState.cs
using UnityEngine;

public abstract class EnemyState
{
    protected BaseEnemy enemy;

    public EnemyState(BaseEnemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}