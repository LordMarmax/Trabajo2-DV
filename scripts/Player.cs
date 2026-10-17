//Player.cs
using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public ObjectGrabber Grabber;
    [Export] public float Speed = 5.0f;
    [Export] public float MouseSensitivity = 0.003f;
    [Export] public float JumpVelocity = 4.5f;

    [Export] public ArrastrePalanca Arrastre;

    [Export] public SpotLight3D Linterna;
    [Export] public RunTimer Temporizador;
    [Export] public float DuracionDesvanecimiento = 2.5f;

    private Tween _tweenLinterna;
    private RayCast3D rayCastAtaque;

    public float Gravity = ProjectSettings
        .GetSetting("physics/3d/default_gravity")
        .AsSingle();

    private Node3D _pivote;
    private Camera3D _camera;

    // Minijuego de cables
    private PuzzleCables puzzleActual;

    private bool EstaEnPuzzle =>
        puzzleActual != null &&
        GodotObject.IsInstanceValid(puzzleActual) &&
        puzzleActual.EstaAbierto;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        _pivote = GetNode<Node3D>("PivoteCamera");
        _camera = GetNode<Camera3D>("PivoteCamera/Camera3D");

        rayCastAtaque = GetNode<RayCast3D>(
            "PivoteCamera/RayCastAtaque"
        );

        _camera.MakeCurrent();

        if (Linterna != null)
        {
            Linterna.Visible = true;
        }
        else
        {
            GD.PushError("Player: SpotLight3D no Asignado");
        }

        if (Temporizador != null)
        {
            Temporizador.BateriaAgotada += ApagarLinterna;
        }
        else
        {
            GD.PushError("Player: RunTimer no asignado");
        }
        if (Temporizador != null)
        {
            Temporizador.BateriaAgotada += ApagarLinterna;
            Temporizador.RunCompletada += DesvanecerLinterna;
        }

    }

    public override void _PhysicsProcess(double delta)
    {
        // Bloquear movimiento durante el minijuego.
        if (EstaEnPuzzle)
        {
            Velocity = Vector3.Zero;
            return;
        }

        Vector3 velocity = Velocity;

        // GRAVEDAD
        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }

        // SALTO
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // MOVIMIENTO DE CÁMARA
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            Vector2 mouseSpeed = Input.GetLastMouseVelocity();

            // Rotación horizontal del jugador.
            Vector3 jugadorRot = Rotation;

            jugadorRot.Y -= mouseSpeed.X *
                MouseSensitivity * (float)delta;

            Rotation = jugadorRot;

            // Rotación vertical de la cámara.
            Vector3 pivoteRot = _pivote.Rotation;

            pivoteRot.X -= mouseSpeed.Y *
                MouseSensitivity * (float)delta;

            pivoteRot.X = Mathf.Clamp(
                pivoteRot.X,
                Mathf.DegToRad(-60),
                Mathf.DegToRad(80)
            );

            _pivote.Rotation = pivoteRot;
        }

        // MOVIMIENTO DEL JUGADOR
        Vector2 inputDir = Input.GetVector(
            "ui_left",
            "ui_right",
            "ui_up",
            "ui_down"
        );

        Vector3 direction = (
            Transform.Basis *
            new Vector3(inputDir.X, 0, inputDir.Y)
        ).Normalized();

        direction.Y = 0;
        direction = direction.Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(
                Velocity.X, 0, Speed
            );

            velocity.Z = Mathf.MoveToward(
                Velocity.Z, 0, Speed
            );
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        // Mientras el puzzle esté abierto,
        // no procesamos controles normales.
        if (EstaEnPuzzle)
            return;

        // ARRASTRE DE PALANCA
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

        if (@event is InputEventMouseMotion movimiento &&
            Arrastre != null &&
            Arrastre.EstaArrastrando)
        {
            Arrastre.Arrastrar(movimiento.Relative);
        }

        // INTERACCIÓN CON E
        if (@event.IsActionPressed("interact") &&
            !@event.IsEcho())
        {
            // Evitar interacciones mientras se arrastra
            // una palanca.
            if (Arrastre != null &&
                Arrastre.EstaArrastrando)
            {
                return;
            }

            // Primero comprobar si apuntamos
            // a una caja de cables.
            if (IntentarInteractuarPuzzle())
                return;

            // Comprobar si apuntamos a un interruptor.
            if (IntentarInteractuarInterruptor())
                return;

            // Si no es una caja, mantener
            // las interacciones existentes.
            if (Grabber == null)
            {
                GD.PushError("ERROR: Grabber es NULL.");
                return;
            }

            Grabber.Interact();
        }
    }

    // DETECTAR CAJA DE CABLES
    private bool IntentarInteractuarPuzzle()
    {
        rayCastAtaque.ForceRaycastUpdate();

        if (!rayCastAtaque.IsColliding())
            return false;

        Node objeto = rayCastAtaque.GetCollider() as Node;

        if (objeto == null)
            return false;

        // Buscar PuzzleCables en el padre
        // del collider de interacción.
        PuzzleCables puzzle =
            objeto.GetParent() as PuzzleCables;

        if (puzzle == null)
            return false;

        puzzle.AbrirPuzzle();

        if (!puzzle.EstaAbierto)
            return false;

        puzzleActual = puzzle;

        GD.Print("Jugador interactuando con caja de cables.");

        return true;
    }
    private bool IntentarInteractuarInterruptor()
    {
        rayCastAtaque.ForceRaycastUpdate();

        if (!rayCastAtaque.IsColliding())
            return false;

        Node objeto = rayCastAtaque.GetCollider() as Node;

        if (objeto == null)
            return false;

        // El collider pertenece al StaticBody3D
        // que contiene InterruptorElectrico.
        if (objeto is InterruptorElectrico interruptor)
        {
            interruptor.Interactuar();

            GD.Print("Jugador pulsó un interruptor.");

            return true;
        }

        return false;
    }

    private void ApagarLinterna()
    {
        if (Linterna == null)
            return;

        _tweenLinterna?.Kill();

        Linterna.LightEnergy = 0.0f;
        Linterna.Visible = false;

        GD.Print("=== LINTERNA APAGADA ===");
        GD.Print("La batería se ha agotado.");
    }
    private void DesvanecerLinterna()
    {
        if (Linterna == null || !Linterna.Visible)
            return;

        GD.Print("=== DESVANECIENDO LINTERNA ===");

        _tweenLinterna?.Kill();

        _tweenLinterna = CreateTween();

        _tweenLinterna.TweenProperty(
            Linterna,
            "light_energy",
            0.0f,
            DuracionDesvanecimiento
        ).SetTrans(Tween.TransitionType.Sine)
        .SetEase(Tween.EaseType.InOut);

        _tweenLinterna.TweenCallback(
            Callable.From(() =>
            {
                Linterna.Visible = false;

                GD.Print("Linterna apagada suavemente.");
            })
        );
    }


}