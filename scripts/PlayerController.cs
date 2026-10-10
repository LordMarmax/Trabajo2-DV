/*using Godot;
using System;
using System.Data.Common;

public partial class Jugador : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float MouseSensitivity = 0.003f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public int danio = 25;
	[Signal] public delegate void VidaCambiadaEventHandler(int vidaAcual);
	public int jugador_vida = 100;
	private bool puedeAtacar = true;
	private AnimationPlayer putazo;
	private float esperaGolpe = 0.1f;
	private float cooldownGolpe = 0.4f;
	
	private RayCast3D rayCastAtaque;
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D _pivote;
	private Camera3D _camera;
	private Camera3D _camera2;

	private bool usandoCamara1 = true;




	public override void _Ready()
	{
		// Capturar el ratón en el centro de la pantalla
		Input.MouseMode = Input.MouseModeEnum.Captured;

		// Buscamos los nodos correspondientes
		_pivote = GetNode<Node3D>("PivoteCamera");
		_camera = GetNode<Camera3D>("PivoteCamera/Camera3D");
		_camera2 = GetNode<Camera3D>("PivoteCamera/Camera3D2");
		rayCastAtaque =GetNode<RayCast3D>("PivoteCamera/RayCastAtaque");
		putazo = GetNode<AnimationPlayer>("AnimationPlayer");
		_camera.MakeCurrent();
		putazo.AnimationFinished += Finputazo;
	}

	public void recibir_Danio(int cantidad)
	{
		jugador_vida -= cantidad;
		
		EmitSignal(SignalName.VidaCambiada, jugador_vida);
	}
    public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("cambiar_camara"))
		{
			CambiarCamara();
		}
		if(@event.IsActionPressed("ataque"))
		{
			
			Atacar();
		}
	}
	private void CambiarCamara()
	{
		if (usandoCamara1)
			_camera2.MakeCurrent();
		else
			_camera.MakeCurrent();

		usandoCamara1 = !usandoCamara1;
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
			pivoteRot.X = Mathf.Clamp(pivoteRot.X, Mathf.DegToRad(-30), Mathf.DegToRad(50));
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


		// LÍNEA NUEVA: Muestra las coordenadas X, Y, Z redondeadas en la consola
		//GD.Print($"Posición Jugador -> X: {GlobalPosition.X:F2} | Y: {GlobalPosition.Y:F2} | Z: {GlobalPosition.Z:F2}");

		//// LÍNEA NUEVA: Muestra las coordenadas X, Y, Z reales de la cámara en la consola
		//if (_camera != null)
		//{
		//    GD.Print($"Posición Cámara  -> X: {_camera.GlobalPosition.X:F2} | Y: {_camera.GlobalPosition.Y:F2} | Z: {_camera.GlobalPosition.Z:F2}");
		//}

		// Liberar ratón con Escape
		
	}


	private void Finputazo (StringName animName)
	{
		if (animName == "golpe")
		{
			puedeAtacar = true;
		}
	}
	private async void Atacar()
	{
		if (!puedeAtacar)
		{
			return;
		}

		puedeAtacar = false;

		putazo.Play("golpe");

		if (!rayCastAtaque.IsColliding())
		{
			return;
		}
		GodotObject objetoGolpeado = rayCastAtaque.GetCollider();
		if(objetoGolpeado is Enemigo enemigo)
		{
			//putazo.Play("golpe");
			//await ToSignal(
            //GetTree().CreateTimer(esperaGolpe),SceneTreeTimer.SignalName.Timeout
        //);
			enemigo.RecibirDanio(danio);
		}
	}
}*/
