using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CarController carController;

    private float _verticalInput;
    private float _horizontalInput;


    void Update()
    {
        CheckInput();
        UpdateCarControllerInputs();
    }


    private void CheckInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void UpdateCarControllerInputs()
    {
        carController.steerInput = _horizontalInput;
        carController.verticalInput = _verticalInput;
    }
}