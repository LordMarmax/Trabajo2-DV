
using Godot;
using System;
using System.Collections.Generic;

public partial class CajaFusibles : Node3D
{
    // Señal cuando se instalan todos los fusibles.
    [Signal]
    public delegate void TodosFusiblesInstaladosEventHandler();

    // Señal cuando se instala un fusible individual.
    [Signal]
    public delegate void FusibleIndividualInstaladoEventHandler(int indice);

    // Lista de compartimientos detectados.
    private readonly List<CompFusible> compartimientos = new();

    // Delegados guardados para desconectarlos correctamente.
    private readonly Dictionary<
        CompFusible,
        CompFusible.FuseInstalledEventHandler
    > conexiones = new();

    // Referencia al Manager principal.
    public CentralElectricaManager Manager { get; private set; }

    public override void _Ready()
    {
        BuscarCompartimientos();
    }

    private void BuscarCompartimientos()
    {
        foreach (Node hijo in GetChildren())
        {
            if (hijo is not CompFusible compartimiento)
                continue;

            compartimientos.Add(compartimiento);

            // Índice correspondiente a este compartimiento.
            int indice = compartimientos.Count - 1;

            // Guardamos el delegado para poder desconectarlo.
            CompFusible.FuseInstalledEventHandler callback = () =>
             {
                 GD.Print(
                     $"CajaFusibles: Fusible {indice + 1} instalado."
                 );

                 // Avisar al Manager qué fusible se instaló.
                 EmitSignal(
                     SignalName.FusibleIndividualInstalado,
                     indice
                 );

                 // Comprobar si se completó el conjunto.
                 ComprobarFusibles();
             };

            compartimiento.FuseInstalled += callback;

            conexiones.Add(compartimiento, callback);

            GD.Print(
                $"CajaFusibles: Detectado {compartimiento.Name}"
            );
        }

        GD.Print(
            $"CajaFusibles: {compartimientos.Count} compartimientos encontrados."
        );
    }

    // Asignar el Manager a todos los compartimientos.
    public void ConfigurarManager(CentralElectricaManager manager)
    {
        Manager = manager;

        foreach (CompFusible compartimiento in compartimientos)
        {
            compartimiento.Manager = manager;

            GD.Print(
                $"CajaFusibles: Manager asignado a {compartimiento.Name}"
            );
        }
    }

    // Consultar si todos los fusibles están instalados.
    public bool EstanTodosInstalados()
    {
        if (compartimientos.Count == 0)
            return false;

        foreach (CompFusible compartimiento in compartimientos)
        {
            if (!compartimiento.EstaOcupado)
                return false;
        }

        return true;
    }

    // Comprobar si se completó el sistema de fusibles.
    private void ComprobarFusibles()
    {
        if (!EstanTodosInstalados())
            return;

        GD.Print("¡Todos los fusibles instalados!");

        EmitSignal(
            SignalName.TodosFusiblesInstalados
        );
    }

    // Desconectar señales al salir del árbol.
    public override void _ExitTree()
    {
        foreach (var conexion in conexiones)
        {
            CompFusible compartimiento = conexion.Key;

            if (GodotObject.IsInstanceValid(compartimiento))
            {
                compartimiento.FuseInstalled -= conexion.Value;
            }
        }

        conexiones.Clear();
        compartimientos.Clear();

        Manager = null;
    }
}
