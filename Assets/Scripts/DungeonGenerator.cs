
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // Va a generar X cuartos, con entre 1 y Z puertas cada uno.
    [SerializeField]
    int numberOfIterations;

    List<Room> allRooms;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Podemos crear el primero antes de entrar al ciclo incluso.
        Room room = new Room();
        // room.

        // allRooms.Add(new Room());


        for (int i = 0; i < numberOfIterations; i++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
