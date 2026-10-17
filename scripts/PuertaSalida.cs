
using Godot;

public partial class PuertaSalida : Node3D
{
	[Export] public CentralElectricaManager Manager;
	[Export] public TransicionPantalla Transicion;

	private bool _salidaActivada = false;

	public void Interactuar()
	{
		if (_salidaActivada)
			return;

		if (Manager == null)
		{
			GD.PushError("Puerta: Manager no asignado.");
			return;
		}

		if (!Manager.CentralCompletada)
		{
			GD.Print("Puerta: No hay energía. Completá la central eléctrica.");
			Manager.MostrarFeedback("¡Debes completar el trabajo!");
			return;
		}

		if (Transicion == null)
		{
			GD.PushError("Puerta: TransicionPantalla no asignada.");
			return;
		}

		_salidaActivada = true;

		GD.Print("Puerta: Salida habilitada.");
		Manager.MostrarFeedback("Finalizando...");
		Transicion.DesvanecerANegro();
	}
}
