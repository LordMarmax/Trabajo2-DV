using Godot;

public partial class CableFlexible : Node3D
{
	[Export] public Marker3D Anclaje;
	[Export] public MeshInstance3D Cilindro;
	[Export] public StaticBody3D Punta;
	[Export] public Marker3D Union;

	private Transform3D transformInicialCilindro;
	private Transform3D transformInicialPunta;

	private Vector3 direccionInicial;
	private float distanciaInicial;

	private Basis orientacionInicial;
	private Vector3 escalaInicial;

	private Vector3 centroGeometricoLocal;
	private float longitudGeometrica;

	[Export] public ConectorCable.ColorCable ColorCable;

	public bool EstaConectado { get; private set; } = false;

	public override void _Ready()
	{
		// Guardar las transformaciones originales.
		transformInicialCilindro = Cilindro.GlobalTransform;
		transformInicialPunta = Punta.GlobalTransform;

		Basis basisInicial = Cilindro.GlobalBasis;

		escalaInicial = basisInicial.Scale;
		orientacionInicial = basisInicial.Orthonormalized();

		Vector3 inicio = Anclaje.GlobalPosition;
		Vector3 fin = Union.GlobalPosition;

		direccionInicial = (fin - inicio).Normalized();
		distanciaInicial = inicio.DistanceTo(fin);

		// El eje longitudinal del cilindro es Z.
		Aabb dimensiones = Cilindro.Mesh.GetAabb();

		centroGeometricoLocal = dimensiones.GetCenter();

		longitudGeometrica =
			dimensiones.Size.Z * Mathf.Abs(escalaInicial.Z);

		GD.Print(
			$"Cable {Name}: longitud inicial = {distanciaInicial}, " +
			$"longitud geométrica = {longitudGeometrica}"
		);
	}

	public void MoverPunta(Vector3 destinoUnion)
	{
		Vector3 inicio = Anclaje.GlobalPosition;
		Vector3 direccion = destinoUnion - inicio;

		float distancia = direccion.Length();

		if (distancia < 0.001f ||
			longitudGeometrica < 0.001f)
			return;

		// Calcular el giro respecto a la dirección inicial.
		Quaternion giro = new Quaternion(
			direccionInicial,
			direccion.Normalized()
		);

		Basis basisGiro = new Basis(giro);

		// ----------------------------------
		// ROTACIÓN Y MOVIMIENTO DE LA PUNTA
		// ----------------------------------

		// Conservar su escala y orientación originales.
		Basis nuevaBasisPunta =
			basisGiro * transformInicialPunta.Basis;

		Punta.GlobalBasis = nuevaBasisPunta;

		// Mover la punta DESPUÉS de rotarla.
		// Union debe coincidir exactamente con destinoUnion.
		Punta.GlobalPosition +=
			destinoUnion - Union.GlobalPosition;

		// ----------------------------------
		// ROTACIÓN Y ESCALA DEL CILINDRO
		// ----------------------------------

		Basis nuevaOrientacion =
			basisGiro * orientacionInicial;

		Vector3 nuevaEscala = escalaInicial;

		// Estirar únicamente sobre el eje Z local.
		nuevaEscala.Z *= distancia / longitudGeometrica;

		Basis nuevaBasis =
			nuevaOrientacion.ScaledLocal(nuevaEscala);

		// Centro entre el anclaje y la unión.
		Vector3 centroDeseado =
			(inicio + destinoUnion) * 0.5f;

		// Compensar el centro geométrico de la malla.
		Vector3 posicionOrigen =
			centroDeseado -
			nuevaBasis * centroGeometricoLocal;

		Cilindro.GlobalTransform = new Transform3D(
			nuevaBasis,
			posicionOrigen
		);
	}

	public void RestaurarPunta()
	{
		Punta.GlobalTransform = transformInicialPunta;
		Cilindro.GlobalTransform = transformInicialCilindro;
	}

	public void Conectar(ConectorCable conector)
	{
		if (EstaConectado)
			return;

		Vector3 destino =
			conector.ObtenerPosicionConexion();

		// Mover y orientar el cable hasta el conector.
		MoverPunta(destino);

		// Bloquear el cable.
		EstaConectado = true;

		GD.Print(
			$"Cable {ColorCable} conectado correctamente."
		);
	}
	public void Desconectar()
	{
		EstaConectado = false;
		RestaurarPunta();
	}
}
