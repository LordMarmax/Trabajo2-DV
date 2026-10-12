
using Godot;

public partial class PalancaEnergia : Node3D
{
    [Signal]
    public delegate void ActivadaEventHandler();

    [ExportGroup("Rotación")]
    [Export] public float AnguloInicial = 0f;
    [Export] public float AnguloFinal = 65f;

    private float _anguloActual;
    private bool _bloqueada = false;

    public bool EstaBloqueada => _bloqueada;

    public override void _Ready()
    {
        _anguloActual = AnguloInicial;
        RotationDegrees = new Vector3(
            _anguloActual,
            RotationDegrees.Y,
            RotationDegrees.Z
        );
    }

    public void Arrastrar(float desplazamientoVertical)
    {
        if (_bloqueada)
            return;

        _anguloActual += desplazamientoVertical;

        _anguloActual = Mathf.Clamp(
            _anguloActual,
            Mathf.Min(AnguloInicial, AnguloFinal),
            Mathf.Max(AnguloInicial, AnguloFinal)
        );

        RotationDegrees = new Vector3(
            _anguloActual,
            RotationDegrees.Y,
            RotationDegrees.Z
        );
    }

    public void BloquearYActivar()
    {
        if (_bloqueada)
            return;

        _bloqueada = true;
        EmitSignal(SignalName.Activada);
    }
}
