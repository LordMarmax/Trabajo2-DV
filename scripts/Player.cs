using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public ObjectGrabber Grabber;
	[Export] public float Speed = 5.0f;
	[Export] public float MouseSensitivity = 0.003f;
	[Export] public float JumpVelocity = 4.5f;

	[Export] public ArrastrePalanca Arrastre;

	private RayCast3D rayCastAtaque;
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D _pivote;
	private Camera3D _camera;
	public override void _Ready()
	{
		// Capturar el ratón en el centro de la pantalla
		Input.MouseMode = Input.MouseModeEnum.Captured;

		// Buscamos los nodos correspondientes
		_pivote = GetNode<Node3D>("PivoteCamera");
		_camera = GetNode<Camera3D>("PivoteCamera/Camera3D");
		rayCastAtaque = GetNode<RayCast3D>("PivoteCamera/RayCastAtaque");
		_camera.MakeCurrent();
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// --- GRAVEDAD ---
		if (!IsOnFloor())
		{
			velocity.Y -= Gravity * (float)delta;
		}

		// 2. --- LÓGICA DE SALTO (NUEVA) ---
		// Si presionas la barra espaciadora ("ui_accept") y estás tocando el suelo, saltas
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}


		// --- MOVIMIENTO DE CÁMARA Y CUERPO POR RATÓN ---
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Vector2 mouseSpeed = Input.GetLastMouseVelocity();

			// 1. El mouse horizontal rota a TODO EL JUGADOR (Eje Y)
			Vector3 jugadorRot = Rotation;
			jugadorRot.Y -= mouseSpeed.X * MouseSensitivity * (float)delta;
			Rotation = jugadorRot;

			// 2. El mouse vertical rota SOLO AL PIVOTE (Eje X)
			Vector3 pivoteRot = _pivote.Rotation;
			pivoteRot.X -= mouseSpeed.Y * MouseSensitivity * (float)delta;
			// Limitamos la mirada para no dar la vuelta completa verticalmente
			//pivoteRot.X = Mathf.Clamp(pivoteRot.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
			pivoteRot.X = Mathf.Clamp(pivoteRot.X, Mathf.DegToRad(-60), Mathf.DegToRad(80));
			_pivote.Rotation = pivoteRot;
		}

		// --- MOVIMIENTO INTELIGENTE EN BASE A HACIA DÓNEC MIRAS ---
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		// 1. Calculamos la dirección cruda usando la orientación del cuerpo
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		// 2. ¡EL TRUCO!: Forzamos que la dirección vertical sea estrictamente 0
		// Esto evita que el personaje vuele o flote si miras hacia el techo/cielo
		direction.Y = 0;
		direction = direction.Normalized(); // Volvemos a normalizar el vector limpio

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}


		Velocity = velocity;
		MoveAndSlide();

	}



	public override void _Input(InputEvent @event)
	{
		// Iniciar o finalizar arrastre.
		if (@event is InputEventMouseButton boton &&
			boton.ButtonIndex == MouseButton.Left)
		{
			GD.Print("Se detecta click");
			if (boton.Pressed)
			{
				if (Arrastre != null &&
					Arrastre.IntentarIniciarArrastre())
				{
					return;
				}
			}
			else
			{
				Arrastre?.FinalizarArrastre();
			}
		}

		// Arrastrar la palanca.
		if (@event is InputEventMouseMotion movimiento &&
			Arrastre != null &&
			Arrastre.EstaArrastrando)
		{
			Arrastre.Arrastrar(movimiento.Relative);
		}

		// Interacción convencional.
		if (@event.IsActionPressed("interact") &&
			!@event.IsEcho())
		{
			// No interactuar con otros objetos durante el arrastre.
			if (Arrastre != null && Arrastre.EstaArrastrando)
				return;

			if (Grabber == null)
			{
				GD.PushError("ERROR: Grabber es NULL.");
				return;
			}

			Grabber.Interact();
		}
	}



}