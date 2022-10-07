using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    BattleManager battleManager;
    public bool OnBattle = false;
	private float firstSpeed;
    public float firstAcceleration;

    private Animator anim;
	private Camera cam;
	private CharacterController controller;

	private Vector3 desiredMoveDirection;
	private Vector3 moveVector;

	public Vector2 moveAxis;
	private float verticalVel;

	[Header("Settings")]
	public bool allowMovement=true;
	[SerializeField] float movementSpeed;
	[SerializeField] float rotationSpeed = 0.1f;
	[SerializeField] float fallSpeed = .2f;
	public float acceleration = 1;

	[Header("Booleans")]
	public	bool blockRotationPlayer;
	private bool isGrounded;

	private JammoActions _jummoActions;
    private void Awake()
    {
		firstSpeed = movementSpeed;
		firstAcceleration = acceleration;
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();

        _jummoActions = new JammoActions();
        _jummoActions.Player.Enable();
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        battleManager.OnBattle += OnBattleBehaviour;
        battleManager.OnBattleFinished += OnBattleFinished;
     }
    private void OnDisable()
    {
        battleManager.OnBattle -= OnBattleBehaviour;
        battleManager.OnBattleFinished -= OnBattleFinished;

        anim.SetFloat("InputMagnitude", 0);
    }
    void OnBattleBehaviour()
    {
        OnBattle = true;
        anim.SetFloat("InputMagnitude", 0);
     }
	void OnBattleFinished()
    {
		Debug.LogError("battle end for movement");
        OnBattle = false;
 		acceleration = firstAcceleration;
    }
    

	void Update()
	{
        if (Input.GetMouseButton(0))
        {
            GetInput2();
			PlayerMoveAndRotation();
        }
        else
        {
			moveAxis.x = 0;
			moveAxis.y = 0;
			blockRotationPlayer = true;
            PlayerMoveAndRotation();
            blockRotationPlayer = false;

        }
	
        InputMagnitude();

		isGrounded = controller.isGrounded;

		if (isGrounded)
			verticalVel -= 0;
		else
			verticalVel -= 1;

		moveVector = new Vector3(0, verticalVel * fallSpeed * Time.deltaTime, 0);

		if (!OnBattle)
			controller.Move(moveVector);
	}

	void PlayerMoveAndRotation()
	{
		if (!allowMovement)
			return;
		if(anim.speed!=1)
        anim.speed = 1;

        var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize();
		right.Normalize();

		desiredMoveDirection = forward * moveAxis.y + right * moveAxis.x;

		if (blockRotationPlayer == false)
		{
			//Camera
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed * acceleration * Time.deltaTime);
			controller.Move(desiredMoveDirection * Time.deltaTime * (movementSpeed * acceleration));
		}
		else
		{
			//Strafe
			controller.Move((transform.forward * moveAxis.y + transform.right * moveAxis.y) * Time.deltaTime * (movementSpeed * acceleration));
		}
	}

	public void LookAt(Vector3 pos)
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), rotationSpeed);
	}

	public void RotateToCamera(Transform t)
	{
		/*
		var forward = cam.transform.forward;

		desiredMoveDirection = forward;
		Quaternion lookAtRotation = Quaternion.LookRotation(desiredMoveDirection);
		Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

		t.rotation = Quaternion.Slerp(transform.rotation, lookAtRotationOnly_Y, rotationSpeed);
		*/
	}

	void InputMagnitude()
	{
 		//Calculate the Input Magnitude
		float inputMagnitude = new Vector2(moveAxis.x, moveAxis.y).sqrMagnitude;

		//Physically move player
		if (inputMagnitude > 0.1f)
		{
			anim.SetFloat("InputMagnitude", inputMagnitude * acceleration, .1f, Time.deltaTime);
			PlayerMoveAndRotation();
		}
		else
		{
			anim.SetFloat("InputMagnitude", inputMagnitude * acceleration, .1f,Time.deltaTime);
		}
	}
	void GetInput2()
    {
        var inputValues = _jummoActions.Player.Move.ReadValue<Vector2>();
        moveAxis.x = inputValues.x;
        moveAxis.y = inputValues.y;
    }
	#region Input

	public void OnMove(InputValue value)
	{
		/*
		//Debug.Log("Movement");
		var inputValues = _jummoActions.Player.Move.ReadValue<Vector2>();
		moveAxis.x = inputValues.x;
		moveAxis.y = inputValues.y;
		*/

    }

	private void GetInput()
    {
		return;
        var inputValues = _jummoActions.Player.Move.ReadValue<Vector2>();
        moveAxis.x = inputValues.x;
        moveAxis.y = inputValues.y;
      //  Debug.Log("x " + inputValues.x);
      //  Debug.Log("y " + inputValues.y);
    }		

	#endregion

 
}
