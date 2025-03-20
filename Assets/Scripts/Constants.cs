using System.Collections.Generic;
using UnityEngine;

// Esta clase se usa para almacenar cosas que ser�n constantes para todo el proyecto
// y que debemos poder acceder desde distintos scripts.
public static class Constants
{
    // Estos enteros se usan para no tener que estar calculando el layer seg�n
    // un string m�s de una vez. As� �nicamente pides el int aqu�.
    // Esto ayuda a optimizar el chequeo de layers individuales por nombre.
    public static int PlayerLayer = LayerMask.NameToLayer("Player");
    public static int WallLayer = LayerMask.NameToLayer("Wall");
    public static int EnemyLayer = LayerMask.NameToLayer("Enemy");
    public static int BulletLayer = LayerMask.NameToLayer("Bullet");
    public static int ObstacleLayer = LayerMask.NameToLayer("Obstacle");
    public static int PickupObjectLayer = LayerMask.NameToLayer("PickupObject");

    public static readonly List<DungeonKeys> AllDungeonKeysList = new List<DungeonKeys>()
    {
        DungeonKeys.Bomb, DungeonKeys.SmallKey, DungeonKeys.BossKey
    };
}

// Enumeraci�n de tipos de da�o que puede haber en el juego, por ejemplo,
// da�o explosivo, da�o de fuego, de corte (slash), de golpe (bash), piercing, etc.
public enum DamageType
{
    Generic, // sin especificar, sin bonus de ning�n tipo. S� se ve reducido por armadura.
    Slash, // efectivo vs algunos enemigos sin armadura.
    Bash, // generalmente efectivo VS enemigos duros como esqueletos, acorazados menores, etc.
    Explosive, // efectivo vs acorazados y acorazados mayores.
    Fire, // efectivo vs algunos enemigos sin armadura, o de naturaleza vegetal.
    Almighty, // lo que sea.
}

public enum DungeonKeys : byte
{
    NONE = 0,
    Bomb, 
    SmallKey,
    BossKey,
    NUMBER_OF_KEYS
}

