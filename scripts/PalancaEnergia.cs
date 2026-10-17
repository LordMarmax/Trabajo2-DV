
using Godot;

public partial class PalancaEnergia : Node3D
{
    [Signal]
    public delegate void ActivadaEventHandler();

    [ExportGroup("Rotación")]
    [Export] public float AnguloInicial = 0f;
    [Export] public float AnguloFinal = 65f;

    [Export] public CentralElectricaManager Manager;

    [Export(PropertyHint.Range, "0.01,1.0,0.01")]
    public float SensibilidadArrastre = 0.15f;


    private float _anguloActual;

    private bool _bloqueada = true;
    private bool _activada = false;


    public bool EstaBloqueada => _bloqueada;
    public bool EstaActivada => _activada;

    public override void _Ready()
    {
        _anguloActual = AnguloInicial;

        ActualizarRotacion();
    }

    // El Manager habilita la palanca.
    public void Desbloquear()
    {
        if (_activada)
            return;

        _bloqueada = false;

        GD.Print("Palanca: Desbloqueada.");
    }

    // El Manager puede bloquearla nuevamente.
    public void Bloquear()
    {
        if (_activada)
            return;

        _bloqueada = true;

        GD.Print("Palanca: Bloqueada.");
    }

    public void Arrastrar(float desplazamientoVertical)
    {

        if (_bloqueada || _activada)
            return;

        _anguloActual +=
            desplazamientoVertical * SensibilidadArrastre;

        _anguloActual = Mathf.Clamp(
            _anguloActual,
            Mathf.Min(AnguloInicial, AnguloFinal),
            Mathf.Max(AnguloInicial, AnguloFinal)
        );

        ActualizarRotacion();

        // Detectar si llegó al ángulo final.
        if (Mathf.IsEqualApprox(_anguloActual, AnguloFinal))
        {
            BloquearYActivar();
        }
    }

    private void ActualizarRotacion()
    {
        RotationDegrees = new Vector3(
            _anguloActual,
            RotationDegrees.Y,
            RotationDegrees.Z
        );
    }

    public void BloquearYActivar()
    {
        if (_activada || _bloqueada)
            return;

        _activada = true;
        _bloqueada = true;

        EmitSignal(SignalName.Activada);
    }
}
