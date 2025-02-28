using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField]
    protected float _movementSpeed = 5.0f;

    CharacterController characterController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!TryGetComponent<CharacterController>(out characterController))
        {
            Debug.LogError($"GameObject: {name} does not have a character controller component attached.");
            return; // salir para no ejecutar ningún otro código posterior a ésto.
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3( Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical") );

        characterController.Move(movement * Time.deltaTime * _movementSpeed);
    }
}
