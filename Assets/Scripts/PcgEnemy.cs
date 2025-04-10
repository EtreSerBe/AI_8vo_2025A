using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;




public class PcgEnemy : BaseEnemy
{
    

    





    
    public float ComputeDifficultyValue(EnemyGenerationSystem egs)
    {
        return stats.difficultyValue = egs.DifficultyFunction(stats.maxHP, stats.attackDamage, stats.attackRate, 
            stats.attackRange, stats.movementSpeed);
    }

    public void GeneratePcgEnemy(EnemyGenerationSystem egs)
    { 
        stats.maxHP = Random.Range(egs.minHpRange, egs.maxHpRange); // 1 a 20
        stats.movementSpeed = Random.Range(egs.minMovementSpeedRange, egs.maxMovementSpeedRange); // 0 a 10
        stats.attackDamage = Random.Range(egs.minAttackDamageRange, egs.maxAttackDamageRange); // 0.1 a 20
        stats.attackRate = Random.Range(egs.minAttackRateRange, egs.maxAttackRateRange); // puede aportar de 0.2 a 5.0
        stats.attackRange = Random.Range(egs.minAttackRangeRange, egs.maxAttackRangeRange); // 1 a 15

        ComputeDifficultyValue(egs);
        
        Debug.LogWarning($"Se generó un enemigo: {gameObject.name} con dificultad: {stats.difficultyValue}");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
