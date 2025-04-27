using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : BaseEnemy
{
    protected override void Start()
    {
        base.Start();
        // Configurations spécifiques à Enemy1
        attackRange = 0.7f;
        attackCooldown = 0.7f;
        navMeshAgent.speed = 0.8f;
        navMeshAgent.stoppingDistance = 0f;
    }
}