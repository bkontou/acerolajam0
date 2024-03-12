using Godot;
using System;

public class PC : KinematicBody
{
	//Mouse variables
	[Export] float mouseSensitivity = 0.3f;



	//Move variables
	[Export] float gravity = -9.8f;
	[Export] float gravityMultiplier = 3f;
	[Export] float moveSpeed = 12f;
	[Export] float moveWalkSpeed = 6f;
	[Export] float moveAcceleration = 7f;
	[Export] float moveDeceleration = 9f;
	[Export] bool canMove = true;
	bool isWalking = false;
	bool forcedWalk = false;

	Vector3 velocity;

	//Jump variables
	[Export] float jumpSpeed = 10f;
	int airJumps = 1;
	int airJumpsLeft;
	bool canJump = false;
	bool alowJumpInput = true;
	bool hasFloorContact = false;


	//View variables
	[Export] float maxAngleView = 90f;
	[Export] float minAngleView = -90f;
	float cameraAngle = 0f;
	float headRelativeAngle = 0f;

	//Slope variables
	[Export] float maxSlopeAngle = 35f;
	
	[Signal]
	public delegate void npc_talked_to(AberrantNPC npc);
	[Signal]
	public delegate void evidence_found(Evidence evidence);
	
	// info
	public float scope_battery = 100;
	public bool scope_up = false;
	
	uint special_camera_mask = 1021;
	uint normal_camera_mask = 2043;

	public Camera camera;
	public Camera special_camera;
	Spatial head;
	RayCast floorChecker;
	RayCast interaction_cast;
	AnimationPlayer scope_animation;
	TextureRect scope_overlay;
	Spatial scope;
	Vector3 direction;
	
	Panel npc_info;

	AberrantNPC current_viewed_npc = null;
	
	public bool controllable = true;

	public override void _Ready()
	{
		GetNodes();
		gravity = gravity * gravityMultiplier;
		airJumpsLeft = airJumps;
		
		special_camera_mask = special_camera.CullMask;
		normal_camera_mask = camera.CullMask;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!controllable) {
			return;
		}
		
		RotateView();
		MoveCharacter(delta);
		CalculateMoveToFloor(delta);

	}

	public override void _Process(float delta)
	{
		if (!controllable) {
			return;
		}
		
		ResetJumpCount();
		UpdateMovementInput();
		UpdateActionInput();
		UpdateScope(delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (!controllable)
		{
			return;
		}
		
		UpdateCameraInput(@event);
	}
	
	void UpdateScope(float delta)
	{
		((TextureProgress)scope_overlay.GetNode("ScopeBattery")).Value = scope_battery;
		
		if (special_camera.Current) {
			scope_battery = Mathf.Max(scope_battery - 15f * delta, 0);
		}
		
		if (scope_up && scope_battery <= 0) {
			special_camera.CullMask = normal_camera_mask;
		}
		else {
			special_camera.CullMask = special_camera_mask;
		}
	}

	void UpdateActionInput()
	{
		if (Input.IsActionJustPressed("ui_rmb"))
		{
			scope_animation.Play("scope_up");
		}
		if (Input.IsActionJustReleased("ui_rmb"))
		{
			BringScopeDown();
		}
		
		if (Input.IsActionJustPressed("ui_select"))
		{
			var collider = interaction_cast.GetCollider();
			
			if (collider == null) {
				return;
			}
			
			if ((( (StaticBody) collider).CollisionMask & 4) != 0) {
				EmitSignal(nameof(npc_talked_to), (AberrantNPC) collider);
				GD.Print("start talking");
			}
			else if ((( (StaticBody) collider).CollisionMask & 8) != 0)
			{
				GD.Print("this is an evidence");
				EmitSignal(nameof(evidence_found), (Evidence) collider);
				((Node)collider).QueueFree();
			}
			else if  ((( (StaticBody) collider).CollisionMask & 16) != 0)
			{
				GD.Print("Picked up battery!");
				scope_battery = 100f;
				((Node)collider).QueueFree();
			}
			
		}
	}

	void UpdateMovementInput()
	{
		direction = Vector3.Zero;

		Basis headDirection = head.GetGlobalTransform().basis;
		Basis cameraDirection = camera.GetGlobalTransform().basis;


		if (canMove) CalculateMoveDirection(cameraDirection, headDirection);


		direction = direction.Normalized();

		isWalking = direction != Vector3.Zero;

		if (Input.IsActionJustPressed("ui_accept") && alowJumpInput)
		{
			canJump = true;
		}
		else
		{
			canJump = false;
		}
	}

	void MoveCharacter(float delta)
	{
		Vector3 tempVelocity = velocity;
		tempVelocity.y = 0f;

		float speed;
		speed = isWalking || forcedWalk ? moveWalkSpeed : moveSpeed;

		Vector3 target = direction * speed;

		float acceleration;
		if (direction.Dot(tempVelocity) > 0)
		{
			acceleration = moveAcceleration;
		}
		else
		{
			acceleration = moveDeceleration;
		}

		tempVelocity = tempVelocity.LinearInterpolate(target, acceleration * delta);

		velocity.x = tempVelocity.x;
		velocity.z = tempVelocity.z;

		if (canJump)
		{
			velocity.y += jumpSpeed;
			hasFloorContact = false;
		}

		velocity = MoveAndSlide(velocity, Vector3.Up);
	}


	void UpdateCameraInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			headRelativeAngle = Mathf.Deg2Rad(-mouseMotion.Relative.x * mouseSensitivity);

			cameraAngle = Mathf.Clamp(cameraAngle - mouseMotion.Relative.y * mouseSensitivity, minAngleView, maxAngleView);
		}
	}

	void GetNodes()
	{
		head = (Spatial)FindNode("Head");
		camera = (Camera)FindNode("MainCamera");
		special_camera = (Camera)FindNode("SpecialCamera");
		floorChecker = (RayCast)FindNode("FloorChecker");
		interaction_cast = (RayCast)FindNode("InteractionCast");
		scope_animation = (AnimationPlayer)FindNode("AnimationPlayer");
		scope_overlay = (TextureRect)FindNode("ScopeOverlay");
		scope = (Spatial)FindNode("MeshInstance");
		npc_info = (Panel)FindNode("NPCInfo");
		
		((NPCInfo)FindNode("NPCInfo")).pc = this;
	}

	void RotateView()
	{
		head.RotateY(headRelativeAngle);
		camera.SetRotationDegrees(new Vector3(cameraAngle, 0f, 0f));
		special_camera.GlobalTransform = camera.GetGlobalTransform();
		headRelativeAngle = 0f;
	}

	void CalculateMoveDirection(Basis cameraDirection, Basis headDirection)
	{
		if (Input.IsActionPressed("ui_up")) direction -= headDirection.z;
		if (Input.IsActionPressed("ui_down")) direction += headDirection.z;
		if (Input.IsActionPressed("ui_left")) direction -= cameraDirection.x;
		if (Input.IsActionPressed("ui_right")) direction += cameraDirection.x;
	}

	void CalculateMoveToFloor(float delta)
	{
		if (IsOnFloor())
		{
			hasFloorContact = true;
			forcedWalk = false;
			alowJumpInput = true;

			Vector3 floorNormal = floorChecker.GetCollisionNormal();
			float floorAngle = Mathf.Rad2Deg(Mathf.Acos(floorNormal.Dot(Vector3.Up)));

			if (floorAngle > maxSlopeAngle)
			{
				forcedWalk = true;
				airJumpsLeft = 0;
				Fall(delta);
			}
		}
		else
		{
			if (!floorChecker.IsColliding())
			{
				hasFloorContact = false;
				Fall(delta);
			}
		}
		if (hasFloorContact && !IsOnFloor()) MoveAndCollide(Vector3.Down);
	}

	void Fall(float delta)
	{
		velocity.y += gravity * delta;
		alowJumpInput = false;
	}

	void ResetJumpCount()
	{
		if (alowJumpInput && airJumpsLeft == 0)
		{
			airJumpsLeft = airJumps;
		}
	}
	
	void DisplayNPCInfo()
	{
		npc_info.Visible = true;
		((Label)npc_info.FindNode("NameLabel")).Text = current_viewed_npc.name;
	}
	
	
	public void BringScopeDown()
	{
		camera.Current = true;
		scope_overlay.Visible = false;
		scope.Visible = true;
		scope_up = false;
		scope_animation.Play("scope_down");
	}

	private void _on_AnimationPlayer_animation_finished(String anim_name)
	{
		if (anim_name == "scope_up")
		{
			special_camera.Current = true;
			
			scope_overlay.Visible = true;
			scope.Visible = false;
			scope_up = true;
		}
	}

	private void _on_LookArea_body_entered(object body)
	{
		GD.Print("view entered");
		current_viewed_npc = (AberrantNPC) body;
		((NPCInfo)FindNode("NPCInfo")).current_viewed_npc = current_viewed_npc;
		//NPCInfo viewed_npc_info = (NPCInfo) ((StaticBody) body).Get("info");
		DisplayNPCInfo();
	}

	private void _on_LookArea_body_exited(object body)
	{
		GD.Print("view exited");
		current_viewed_npc = null;
		((NPCInfo)FindNode("NPCInfo")).current_viewed_npc = null;
		npc_info.Visible = false;
	}
}



