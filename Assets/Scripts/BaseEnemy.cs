using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyStats
{
    public EnemyStats()
    {
        // constructor vacío.
    }
    
    public EnemyStats(EnemyStats value)
    {
        this.maxHP = value.maxHP;
        this.attackDamage = value.attackDamage;
        this.attackRange = value.attackRange;
        this.attackRate = value.attackRate;
        this.movementSpeed = value.movementSpeed;
    }
    
    [SerializeField]
    public float maxHP;
    
    [SerializeField] public float movementSpeed;
    
    [SerializeField]
    public float attackDamage = 1.0f;
    
    [SerializeField]
    public float attackRate = 1.0f;
    
    [SerializeField]
    public float attackRange = 1.0f;
    
    public float difficultyValue = 0.0f;
    
    public EnemyGenerationSystem.EnemyDifficulty difficultyCategory = EnemyGenerationSystem.EnemyDifficulty.Easy;

    public string PrintStats()
    {
        return $"DifficultyValue = {difficultyValue}; Category: {difficultyCategory}; HP: {maxHP}; " +
               $"Damage: {attackDamage}; Attack Rate: {attackRate}; Range: {attackRange}; " +
               $"Movement Speed: {movementSpeed}";
    }
    
}

public class BaseEnemy : MonoBehaviour
{
    protected float currentHP;

   
    // Radio de detecci�n 
    [SerializeField]
    protected Senses detectionSenses;


    [SerializeField]
    protected NavMeshAgent navMeshAgent;

    // MeshRenderer meshRenderer;
    public EnemyStats stats = new EnemyStats();
   
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = stats.maxHP;
    }

    private void FixedUpdate()
    {
        // Si el script de Senses ya detect� a alguien.
        // if(detectionSenses.IsEnemyDetected())
        {
            // entonces podemos setearlo en el script de steering behaviors.
            // steeringBehaviors.SetEnemyReference(detectionSenses.GetDetectedEnemyRef());
            // steeringBehaviors.obstacleList = detectionSenses.GetDetectedObstacles();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // si contra quien choc� este enemigo es algo de la capa de Balas del jugador, este enemigo debe de tomar da�o.
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            // obtenemos el script de bullet de ese gameobject que nos choc�, 
            Bullet collidingBullet = other.GetComponent<Bullet>();
            if (collidingBullet == null)
            {
                // si no tiene un script de Bullet, entonces no hay nada que hacer,
                // probablemente a ese "other" le falta que se le asigne el script de bullet.
                Debug.LogError("error, alguien en la capa PlayerBullet no tiene script de Bullet.");
                return;
            }

            // y nos restamos la vida en la cantidad que Bullet nos diga.
            currentHP -= collidingBullet.GetDamage();


            Debug.Log($"perd� {collidingBullet.GetDamage()} de vida, mi vida ahora es: {currentHP}");


            // si tu vida llega a 0 o menos, te mueres.
            if (currentHP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}