
using Godot;

public partial class InterruptorElectrico : StaticBody3D
{
    [Export] public Node3D InterPivote3D;
    [Export] public MeshInstance3D Luz;

    [Export] public float AnguloActivado = 180f;

    [Export] public Color ColorApagado = Colors.Red;
    [Export] public Color ColorEncendido = Colors.Green;

    private bool encendido = false;

    private Vector3 rotacionInicial;
    private StandardMaterial3D materialLuz;

    public override void _Ready()
    {
        if (InterPivote3D == null || Luz == null)
        {
            GD.PushError(
                $"Interruptor {Name}: faltan referencias."
            );
            return;
        }

        // Guardar la rotación original del pivote.
        rotacionInicial = InterPivote3D.RotationDegrees;

        // Crear una copia independiente del material.
        Material materialOriginal =
            Luz.GetActiveMaterial(0);

        if (materialOriginal is StandardMaterial3D material)
        {
            materialLuz =
                (StandardMaterial3D)material.Duplicate();

            Luz.SetSurfaceOverrideMaterial(0, materialLuz);
        }
        else
        {
            GD.PushWarning(
                $"Interruptor {Name}: la luz necesita un StandardMaterial3D."
            );
        }

        ActualizarEstado();
    }

    public void Interactuar()
    {
        if (InterPivote3D == null || Luz == null)
            return;

        encendido = !encendido;

        ActualizarEstado();

        GD.Print(
            $"Interruptor {Name}: " +
            (encendido ? "ENCENDIDO" : "APAGADO")
        );
    }

    private void ActualizarEstado()
    {
        // Rotar instantáneamente el pivote en Z.
        Vector3 rotacion = rotacionInicial;

        if (encendido)
            rotacion.Z += AnguloActivado;

        InterPivote3D.RotationDegrees = rotacion;

        // Actualizar el color de la luz.
        if (materialLuz != null)
        {
            Color color = encendido
                ? ColorEncendido
                : ColorApagado;

            materialLuz.AlbedoColor = color;

            materialLuz.EmissionEnabled = true;
            materialLuz.Emission = color;
        }
    }

    public bool EstaEncendido()
    {
        return encendido;
    }
}
