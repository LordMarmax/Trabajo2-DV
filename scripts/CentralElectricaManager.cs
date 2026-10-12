
using Godot;

public partial class CentralElectricaManager : Node
{
    public enum EtapaCentral
    {
        Cables,
        Fusibles,
        Interruptores,
        Palanca,
        Completado
    }

    [Signal]
    public delegate void EtapaCambiadaEventHandler(int nuevaEtapa);

    [Export]
    public EtapaCentral EtapaActual =
        EtapaCentral.Cables;

    public bool CablesListos =>
        EtapaActual >= EtapaCentral.Fusibles;

    public bool FusiblesListos =>
        EtapaActual >= EtapaCentral.Interruptores;

    public bool InterruptoresListos =>
        EtapaActual >= EtapaCentral.Palanca;

    public bool CentralRestaurada =>
        EtapaActual == EtapaCentral.Completado;

    public void CompletarCables()
    {
        Avanzar(
            EtapaCentral.Cables,
            EtapaCentral.Fusibles
        );
    }

    public void CompletarFusibles()
    {
        Avanzar(
            EtapaCentral.Fusibles,
            EtapaCentral.Interruptores
        );
    }

    public void CompletarInterruptores()
    {
        Avanzar(
            EtapaCentral.Interruptores,
            EtapaCentral.Palanca
        );
    }

    public void ActivarPalanca()
    {
        Avanzar(
            EtapaCentral.Palanca,
            EtapaCentral.Completado
        );
    }

    private void Avanzar(
        EtapaCentral requerida,
        EtapaCentral siguiente
    )
    {
        if (EtapaActual != requerida)
            return;

        EtapaActual = siguiente;

        GD.Print($"Central: etapa {EtapaActual}");

        EmitSignal(
            SignalName.EtapaCambiada,
            (int)EtapaActual
        );
    }
}
