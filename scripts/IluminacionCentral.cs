
using Godot;
using System.Collections.Generic;

public partial class IluminacionCentral : Node
{
	[Export] public bool ApagarAlIniciar = true;

	[ExportGroup("Emisión de bombillas")]
	[Export] public Color ColorEmision = Colors.White;
	[Export] public float IntensidadEmision = 3.0f;

	private bool _encendida = false;

	private readonly List<Light3D> luces = new();
	private readonly List<StandardMaterial3D> materialesBombillas = new();

	public override void _Ready()
	{
		BuscarElementos(this);

		if (ApagarAlIniciar)
			ApagarLuces();
	}

	private void BuscarElementos(Node nodo)
	{
		foreach (Node hijo in nodo.GetChildren())
		{
			// Detectar luces reales.
			if (hijo is Light3D luz)
			{
				luces.Add(luz);
			}

			// Detectar la malla luminosa de las bombillas.
			if (hijo is MeshInstance3D mesh &&
				hijo.Name.ToString() == "bombilla")
			{
				ConfigurarBombilla(mesh);
			}

			// Buscar también dentro de nodos hijos.
			BuscarElementos(hijo);
		}
	}

	private void ConfigurarBombilla(MeshInstance3D mesh)
	{
		Material original = mesh.GetActiveMaterial(0);

		if (original is not StandardMaterial3D materialOriginal)
		{
			GD.PushWarning(
				$"Bombilla {mesh.GetPath()}: " +
                "No tiene un StandardMaterial3D."
			);
			return;
		}

		// Duplicar el material para no modificar otras mallas.
		StandardMaterial3D material =
			(StandardMaterial3D)materialOriginal.Duplicate();

		// Aplicar como Material Override.
		mesh.MaterialOverride = material;

		materialesBombillas.Add(material);

		GD.Print($"Bombilla detectada: {mesh.GetPath()}");
	}

	public void EncenderLuces()
	{
		if (_encendida)
			return;

		_encendida = true;

		foreach (Light3D luz in luces)
		{
			luz.Visible = true;
		}

		foreach (StandardMaterial3D material in materialesBombillas)
		{
			material.EmissionEnabled = true;
			material.Emission = ColorEmision;
			material.EmissionEnergyMultiplier = IntensidadEmision;
		}

		GD.Print(
            "Iluminación central: Luces y bombillas encendidas."
		);
	}

	public void ApagarLuces()
	{
		_encendida = false;

		foreach (Light3D luz in luces)
		{
			luz.Visible = false;
		}

		foreach (StandardMaterial3D material in materialesBombillas)
		{
			material.EmissionEnabled = false;
		}

		GD.Print(
            "Iluminación central: Luces y bombillas apagadas."
		);
	}
}
