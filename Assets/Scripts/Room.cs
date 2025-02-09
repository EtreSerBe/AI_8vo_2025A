using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Room 
{
    // Usados para preguntarle al Owner qué hay alrededor de este cuarto.
    public int xPos;
    public int yPos;

    public bool occupied = false;

    public Room parent = null;
    public int iteration = 0;

    private TileMap _ownerTileMap;
    public void Setup(TileMap map, int x, int y) 
    { 
        _ownerTileMap = map; 
        xPos = x;
        yPos = y;
    }

    // Nuestro room va a tener entre 1 y 4 puertas (binding of isaac).
    List<Door> doors;

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
