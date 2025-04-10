using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyGenerationSystem : MonoBehaviour
{
    public int maxGreedySearchIterations = 50;
    public float greedySearchTolerance = 0.05f;
    public int rangeSteps = 10;
    
    [SerializeField]
    private PcgEnemy pcgEnemyPrefab;

    private List<PcgEnemy> _createdEnemies = new List<PcgEnemy>();
    
    public enum EnemyDifficulty
    {
        Easy, 
        Medium,
        Hard
    }
    
    [SerializeField]
    public float minHpRange= 1.0f;
    
    [SerializeField]
    public float maxHpRange = 20.0f;
    
    [SerializeField]
    public float minAttackDamageRange = 0.1f;
    
    [SerializeField]
    public float maxAttackDamageRange = 20.0f;
    
    [SerializeField]
    public float minAttackRateRange = 5.0f;

    [SerializeField]
    public float maxAttackRateRange = 0.5f;
    
    [SerializeField]
    public float minAttackRangeRange = 1.0f;
    
    [SerializeField]
    public float maxAttackRangeRange = 15.0f;

    [SerializeField]
    public float minMovementSpeedRange = 0.0f;

    [SerializeField]
    public float maxMovementSpeedRange = 10.0f;

    private float hpStepDistance;
    private float damageStepDistance;
    private float rateStepDistance;
    private float rangeStepDistance;
    private float movementSpeedStepDistance;
    
    
    protected static float maxDifficultyValue;
    protected static float minDifficultyValue;
    protected static float difficultyFunctionRange;  // el máximo menos el mínimo 

    public float DifficultyFunction(EnemyStats stats)
    {
        return DifficultyFunction(stats.maxHP, stats.attackDamage, stats.attackRate, stats.attackRange,
            stats.movementSpeed);
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

    public EnemyStats GreedySearch(PcgEnemy origin)
    {
        EnemyStats currentNode = origin.stats;

        PriorityQueue openList = new PriorityQueue(false);
        openList.Enqueue(currentNode, currentNode.difficultyValue);

        HashSet<EnemyStats> closedList = new HashSet<EnemyStats>();

        int currentIteration = 0;
        
        // Posibles condiciones de terminación:
        // 1) límite de iteraciones. Por ejemplo, 50 nodos visitados (en closed list). y después tomamos al mejor.
        // 2) ningún elemento en la lista abierta tiene un mejor valor de métrica que él.
        // ejemplo, mi currentNode tiene dificultad de .75, y el de hasta el frente de la openList tiene 0.73
        // 3) similar a la 2), pero con un "threshold" o umbral,
        // Por ejemplo: tengo un umbral de 0.05, currentNode tiene 0.75, y el del frente de la openList tiene 0.69
        // aquí, 0.75 - 0.05 es mayor que 0.69, entonces terminamos el algoritmo; pero uno de 0.71 sí haría que siga
        while (currentIteration < maxGreedySearchIterations)
        {
            currentIteration++; // para garantizar que salgamos de aquí.
            
            // sacar el elemento del frente de la lista abierta.
            currentNode = openList.Dequeue();
            // Después de sacarlo de la lista abierta, lo metemos a la lista cerrada.
            closedList.Add(currentNode);
            
            // checamos a sus vecinos.
            // Tenemos que checar el step distance.

            // 1, 2.9, 4.8, 6.7 ... 18.1, 20
            
            // 19.9
            
            if (currentNode.maxHP != maxHpRange)
            {
                // entonces sí podemos meter a ese vecino.
                EnemyStats newNode = new EnemyStats(currentNode);
                newNode.maxHP = math.min(currentNode.maxHP + hpStepDistance, maxHpRange);
                newNode.difficultyValue = DifficultyFunction(newNode); // Remplazarla por TotalScore
                openList.Enqueue(newNode, newNode.difficultyValue);
            }
            if (currentNode.maxHP != minHpRange)
            {
                // entonces sí podemos meter a ese vecino.
                EnemyStats newNode = new EnemyStats(currentNode);
                newNode.maxHP = math.max(currentNode.maxHP - hpStepDistance, maxHpRange);
                newNode.difficultyValue = DifficultyFunction(newNode); // Remplazarla por TotalScore
                openList.Enqueue(newNode, newNode.difficultyValue);
            }
            
            // AÑADIR LAS DEMÁS CARACTERÍSTICAS QUE TENGAN EN SU SISTEMA DE GENERACIÓN DE ENEMIGOS. COMO, DAMAGE, 
            // ATTACK RATE, ATTACK RANGE, ETC.
            
            
            

            // esto de aquí tiene que ser forzosamente después de checar a sus vecinos.
            if (openList.First().difficultyValue + greedySearchTolerance < currentNode.difficultyValue)
            {
                // terminamos el algoritmo porque el mejor de la lista abierta no le llega ni cerca al currentNode.
                break;
            }
            
        }

        // después del while ya tenemos al enemigo mejorcito que encontramos. Comparamos contra el que iniciamos.
        Debug.Log($"El enemigo original tenía: {origin.stats.PrintStats()} ; " +
                  $"el enemigo mejorado tiene: {currentNode.PrintStats()}");

        // finalmente, regresamos al enemigo obtenido.
        return currentNode;
    }
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        hpStepDistance = (maxHpRange - minHpRange) / rangeSteps;
        damageStepDistance = (maxAttackDamageRange - minAttackDamageRange) / rangeSteps;
        rateStepDistance = (maxAttackRateRange - minAttackRateRange) / rangeSteps;
        rangeStepDistance = (maxAttackRangeRange - minAttackRangeRange) / rangeSteps;
        movementSpeedStepDistance = (maxMovementSpeedRange - minMovementSpeedRange) / rangeSteps;

        
        
        PcgEnemy newEnemy = Instantiate(pcgEnemyPrefab);
        _createdEnemies.Add( newEnemy);
        
        newEnemy.GeneratePcgEnemy(this);
        
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

        if (newEnemy.stats.difficultyValue <= minDifficultyValue + GroupRange)
        {
            newEnemy.stats.difficultyCategory = EnemyDifficulty.Easy;
        }
        else if (newEnemy.stats.difficultyValue <= minDifficultyValue + GroupRange * 2)
        {
            newEnemy.stats.difficultyCategory = EnemyDifficulty.Medium;
        }
        else if (newEnemy.stats.difficultyValue <= minDifficultyValue + GroupRange * 3)
        {
            newEnemy.stats.difficultyCategory = EnemyDifficulty.Hard;
        }
        Debug.Log($"El enemigo {name} es de categoría de dificultad : {newEnemy.stats.difficultyCategory}");

        
        // lo cambiamos del rango de 300 a 6000, hacia el rango de 0 a 5700
        float movedDifficultyValue = newEnemy.stats.difficultyValue - minDifficultyValue;
        float dividedMovedDifficultyValue = movedDifficultyValue / GroupRange;
        newEnemy.stats.difficultyCategory = (EnemyDifficulty)(int)dividedMovedDifficultyValue;

        GreedySearch(newEnemy);

        Debug.Log($"El enemigo {name} es de categoría de dificultad en la versión matemática : {newEnemy.stats.difficultyCategory}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
