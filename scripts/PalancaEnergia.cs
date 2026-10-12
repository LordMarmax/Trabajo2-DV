/* Este script tendrá la responsabilidad de controlar la rotación
    y su estado de bloqueo.
    
*/
using Godot;

public partial class PalancaEnergia : Node3D
{
    [Signal]
    public delegate void ActivadaEventHandler();

    [ExportGroup("Rotación")]
    [Export] public float AnguloInicial = 0f;
    [Export] public float AnguloFinal = 65f;
    [Export] public float Sensibilidad = 0.25f;

    private float _anguloActual;
    private bool _bloqueada = false;

    public override void _Ready()
    {
        _anguloActual = AnguloInicial;
        RotationDegrees = new Vector3(
            _anguloActual, RotationDegrees.Y, RotationDegrees.Z
        );
    }

    public void Arrastrar(float desplazamientoMouse)
    {
        if (_bloqueada)
            return;

        _anguloActual += desplazamientoMouse * Sensibilidad;

        _anguloActual = Mathf.Clamp(
            _anguloActual,
            Mathf.Min(AnguloInicial, AnguloFinal),
            Mathf.Max(AnguloInicial, AnguloFinal)
        );

        RotationDegrees = new Vector3(
            _anguloActual, RotationDegrees.Y, RotationDegrees.Z
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
