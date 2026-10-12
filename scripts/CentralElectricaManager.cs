
using Godot;

public partial class CentralElectricaManager : Node
{
    [Export] public ArrastreCables SistemaCables;

    public bool CablesConectados { get; private set; }

    [Signal]
    public delegate void EstadoElectricoCambiadoEventHandler();

    public override void _Ready()
    {
        if (SistemaCables == null)
        {
            GD.PushWarning(
                "Manager: Falta asignar SistemaCables."
            );
            return;
        }

        SistemaCables.EstadoCablesCambiado +=
            OnEstadoCablesCambiado;

        OnEstadoCablesCambiado(
            SistemaCables.TodosConectados()
        );
    }

    private void OnEstadoCablesCambiado(bool completos)
    {
        if (CablesConectados == completos)
            return;

        CablesConectados = completos;

        GD.Print(
            $"Manager: Cables conectados = {CablesConectados}"
        );

        ActualizarEstadoElectrico();
    }

    private void ActualizarEstadoElectrico()
    {
        // Próximamente:
        // 1. Comprobar fusibles.
        // 2. Actualizar indicadores.
        // 3. Comprobar interruptores.
        // 4. Habilitar alimentación final.

        EmitSignal(
            SignalName.EstadoElectricoCambiado
        );
    }

    public override void _ExitTree()
    {
        if (SistemaCables != null &&
            GodotObject.IsInstanceValid(SistemaCables))
        {
            SistemaCables.EstadoCablesCambiado -=
                OnEstadoCablesCambiado;
        }
    }
}
