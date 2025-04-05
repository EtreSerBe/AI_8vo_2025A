using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static TileMap;

public class Room 
{
    // Usados para preguntarle al Owner qué hay alrededor de este cuarto.
    public byte xPos;
    public byte yPos;

    public bool occupied = false;
    public bool walkable = true;

    public Room parent = null;

    public TileDirections parentDirection = TileDirections.None;
    public byte iteration = 0;

    byte _keyId = 0; // es el ID que nos dice qué llave es obtenible en ese room.

    // Nuestro room va a tener entre 1 y 4 puertas (binding of isaac).
    List<Door> doors = new List<Door>();

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }

    public Door GetDoorToRoom(Room otherRoom)
    {
        foreach (Door door in doors)
        {
            if (door.ConnectsToRoom(otherRoom))
                return door;
        }

        return null;
    }

    // función que nos regresa las puertas de un cuarto que todavía no tienen un cerrojo asignado.
    public List<Door> GetDoorsWithNoLockAssigned()
    {
        List<Door> result = new List<Door>();
        foreach (Door door in doors)
        {
            if (door._keyId == 0)
                result.Add(door);
        }

        return result;
    }

    public byte GetKeyTypeInsideRoom()
    {
        return _keyId;
    }

    public void SetObtainableKey(byte keyId)
    {
        _keyId = keyId;
    }

    // private TileMap _ownerTileMap;

    public void Setup(TileMap map, byte x, byte y) 
    { 
        // _ownerTileMap = map; 
        xPos = x;
        yPos = y;
    }



    public void Init(Room parentNeighbor, Door neighborDoor)
    {
        // Nos setea una puerta que es la de nuestro "padre" en el proceso de generación.
        doors.Add(neighborDoor);
    }

    public void GenerateDoors(/*int maxDoors*/int maxIterations, int currentIteration)
    {
        // añadimos un número entre 0 y 3 de puertas
        int numberOfDoors = Random.Range(0, 4);
        for (int i = 0; i < numberOfDoors; i++) 
        {
            // creamos un nuevo cuarto.
            Room newRoom = new Room();


            Door newDoor = new Door(this, newRoom);

            // Le decimos al cuarto nuevo que ya tiene una puerta y que lleva hacia este cuarto actual.
            newRoom.Init(this, newDoor);
        }
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
