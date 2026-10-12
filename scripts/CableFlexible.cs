using Godot;

public partial class CableFlexible : Node3D
{
	[Export] public Marker3D Anclaje;
	[Export] public MeshInstance3D Cilindro;
	[Export] public Node3D Punta;

	[Export] public float LongitudOriginal = 1.0f;

	private Vector3 escalaOriginal;

	public override void _Ready()
	{
		escalaOriginal = Cilindro.Scale;
	}

	public void MoverPunta(Vector3 destinoGlobal)
	{
		Vector3 inicio = Anclaje.GlobalPosition;
		Vector3 direccion = destinoGlobal - inicio;

		float longitud = direccion.Length();

		if (longitud < 0.001f)
			return;

		// La punta sigue el destino.
		Punta.GlobalPosition = destinoGlobal;

		// El cilindro se coloca entre ambos extremos.
		Cilindro.GlobalPosition =
			(inicio + destinoGlobal) / 2.0f;

		// Orientamos el eje Y del cilindro hacia la punta.
		Vector3 ejeY = direccion.Normalized();

		Vector3 referencia = Mathf.Abs(ejeY.Dot(Vector3.Up)) > 0.99f
			? Vector3.Forward
			: Vector3.Up;

		Vector3 ejeX = referencia.Cross(ejeY).Normalized();
		Vector3 ejeZ = ejeX.Cross(ejeY).Normalized();

		Cilindro.GlobalBasis = new Basis(
			ejeX,
			ejeY,
			ejeZ
		).Orthonormalized();

		// Solo se estira el cilindro en su eje longitudinal.
		Cilindro.Scale = new Vector3(
			escalaOriginal.X,
			escalaOriginal.Y * longitud / LongitudOriginal,
			escalaOriginal.Z
		);
	}
}
