
using Godot;

public partial class InterruptorElectrico : StaticBody3D
{
    [Export] public Node3D InterPivote3D;

    [Export] public LucesInterruptores SistemaLuces;

    // Índice de la luz correspondiente.
    [Export] public int IndiceLuz = 0;

    [Export] public float AnguloActivado = 180f;

    private bool encendido = false;
    private Vector3 rotacionInicial;

    public override void _Ready()
    {
        if (InterPivote3D == null)
        {
            GD.PushError(
                $"Interruptor {Name}: falta InterPivote3D."
            );
            return;
        }

        rotacionInicial = InterPivote3D.RotationDegrees;
        ActualizarRotacion();
    }

    public void ConfigurarLuces(LucesInterruptores sistema)
    {
        SistemaLuces = sistema;
    }

    public void Interactuar()
    {
        if (InterPivote3D == null || SistemaLuces == null)
            return;

        // No permitir activar sin su fusible.
        if (!SistemaLuces.EstaPreparada(IndiceLuz))
        {
            GD.Print(
                $"Interruptor {Name}: falta instalar su fusible."
            );
            return;
        }

        encendido = !encendido;

        ActualizarRotacion();

        SistemaLuces.CambiarLuzVerde(
            IndiceLuz,
            encendido
        );

        GD.Print(
            $"Interruptor {Name}: " +
            (encendido ? "ENCENDIDO" : "APAGADO")
        );
    }

    private void ActualizarRotacion()
    {
        Vector3 rotacion = rotacionInicial;

        if (encendido)
            rotacion.Z += AnguloActivado;

        InterPivote3D.RotationDegrees = rotacion;
    }

    public bool EstaEncendido()
    {
        return encendido;
    }
}
