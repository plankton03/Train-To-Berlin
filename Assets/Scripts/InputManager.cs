using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CarController carController;

    private float _verticalInput;
    private float _horizontalInput;

    public bool touch;



    void Update()
    {
        CheckInput();
        UpdateCarControllerInputs();
    }


    private void CheckInput()
    {
        if (touch == true)
        {
            foreach (var touch in Input.touches)
            {
                
                if (Input.touchCount == 1 )
                {
                    if (touch.position.x > Screen.width - Screen.width / 3)
                        _horizontalInput = 1;
                    else if (touch.position.x < Screen.width*1.0f / 3)
                        _horizontalInput = -1;
                    _verticalInput = 0;
                }else if (Input.touchCount == 2 )
                {
                    _horizontalInput = 0;
                    _verticalInput = -1;
                }
                else
                {
                    _horizontalInput = 0;
                    _verticalInput = 0;
                }
            }
        }
        else
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
        }
    }

    private void UpdateCarControllerInputs()
    {
        carController.steerInput = _horizontalInput;
        carController.verticalInput = _verticalInput;
    }
}