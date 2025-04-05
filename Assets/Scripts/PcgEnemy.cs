using UnityEditor;
using UnityEngine;

public class PcgEnemy : BaseEnemy
{
    [SerializeField]
    protected float minHpRange= 1.0f;
    
    [SerializeField]
    protected float maxHpRange = 20.0f;
    
    [SerializeField]
    protected float minAttackDamageRange = 0.1f;
    
    [SerializeField]
    protected float maxAttackDamageRange = 20.0f;
    
    [SerializeField]
    protected float minAttackRateRange = 5.0f;

    [SerializeField]
    protected float maxAttackRateRange = 0.5f;
    
    [SerializeField]
    protected float minAttackRangeRange = 1.0f;
    
    [SerializeField]
    protected float maxAttackRangeRange = 15.0f;

    [SerializeField]
    protected float minMovementSpeedRange = 0.0f;

    [SerializeField]
    protected float maxMovementSpeedRange = 10.0f;


    public enum EnemyDifficulty
    {
        Easy, 
        Medium,
        Hard
    }


    protected float difficultyValue = 0.0f;
    protected EnemyDifficulty difficultyCategory = EnemyDifficulty.Easy;

    protected static float maxDifficultyValue;
    protected static float minDifficultyValue;
    protected static float difficultyFunctionRange;  // el máximo menos el mínimo 

    
    public float ComputeDifficultyValue()
    {
        return difficultyValue = DifficultyFunction(maxHP, attackDamage, attackRate, attackRange, movementSpeed);
    }

    public float DifficultyFunction(float HP, float AttackDamage, float AttackRate, float AttackRange,
        float MovementSpeed)
    {
        // a la hora de calcular la dificultad, estén normalizados.
        // restarle el mínimo del rango (minRange), y dividirlo entre el rango total esa característica (maxRange - minRange)
        float normalizedHp = (HP - minHpRange) / (maxHpRange - minHpRange);
        float normalizedAttackDamage = (AttackDamage - minAttackDamageRange) / (maxAttackDamageRange - minAttackDamageRange);
        // float normalizedAttackRate = (AttackRate - minAttackRateRange) / (maxAttackRateRange - minAttackRateRange);
        float normalizedAttackRange = (AttackRange - minAttackRangeRange) / (maxAttackRangeRange - minAttackRangeRange);
        float normalizedMovementSpeed = (MovementSpeed - minMovementSpeedRange) / (maxMovementSpeedRange - minMovementSpeedRange);

        
        // HP, movement speed, attack rate, damage y range.
        // float attackPower = AttackDamage * (1.0f / AttackRate) * AttackRange;
        // float finalvalue = (HP * 0.3f) * attackPower + (MovementSpeed * 0.2f) * attackPower + attackPower * 0.5f;
        float finalValue = normalizedHp  + normalizedAttackDamage + /*(1.0f / AttackRate) +*/ normalizedAttackRange + normalizedMovementSpeed;
        finalValue /= 4.0f;
        return finalValue;
    }

    public void GeneratePcgEnemy()
    {
        maxHP = Random.Range(minHpRange, maxHpRange); // 1 a 20
        movementSpeed = Random.Range(minMovementSpeedRange, maxMovementSpeedRange); // 0 a 10
        attackDamage = Random.Range(minAttackDamageRange, maxAttackDamageRange); // 0.1 a 20
        attackRate = Random.Range(minAttackRateRange, maxAttackRateRange); // puede aportar de 0.2 a 5.0
        attackRange = Random.Range(minAttackRangeRange, maxAttackRangeRange); // 1 a 15

        ComputeDifficultyValue();
        
        
        Debug.LogWarning($"Se generó un enemigo: {gameObject.name} con dificultad: {difficultyValue}");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GeneratePcgEnemy();
        
        maxDifficultyValue = DifficultyFunction(maxHpRange, maxAttackDamageRange, maxAttackRateRange,
                maxAttackRangeRange, maxMovementSpeedRange);
        Debug.Log($"El valor máximo de la función de dificultad es: {maxDifficultyValue}");
        
        minDifficultyValue = DifficultyFunction(minHpRange, minAttackDamageRange, minAttackRateRange,
            minAttackRangeRange, minMovementSpeedRange);
        Debug.Log($"El valor mínimo de la función de dificultad es: {minDifficultyValue}");


        difficultyFunctionRange = maxDifficultyValue - minDifficultyValue;
        Debug.Log($"El rango de la función de dificultad es: {difficultyFunctionRange}");

        // Tomamos el rango de la función y lo dividimos en tres partes iguales (una para fácil, otra para medio y otra difícil). 
        float GroupRange = difficultyFunctionRange / 3;
        Debug.Log($"El rango por grupo es de : {GroupRange}");

        if (difficultyValue <= minDifficultyValue + GroupRange)
        {
            difficultyCategory = EnemyDifficulty.Easy;
        }
        else if (difficultyValue <= minDifficultyValue + GroupRange * 2)
        {
            difficultyCategory = EnemyDifficulty.Medium;
        }
        else if (difficultyValue <= minDifficultyValue + GroupRange * 3)
        {
            difficultyCategory = EnemyDifficulty.Hard;
        }
        Debug.Log($"El enemigo {name} es de categoría de dificultad : {difficultyCategory}");

        
        // lo cambiamos del rango de 300 a 6000, hacia el rango de 0 a 5700
        float movedDifficultyValue = difficultyValue - minDifficultyValue;
        float dividedMovedDifficultyValue = movedDifficultyValue / GroupRange;
        difficultyCategory = (EnemyDifficulty)(int)(dividedMovedDifficultyValue);

        Debug.Log($"El enemigo {name} es de categoría de dificultad en la versión matemática : {difficultyCategory}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
