using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;

public class Realtime : MonoBehaviour
{
    DatabaseReference referenciaBaseDatos;

    // Identificador único del jugador
    string idJugador;

    void Start()
    {
        // Inicializa Firebase
        referenciaBaseDatos = FirebaseDatabase.DefaultInstance.RootReference;

        // Genera un ID de jugador único, por ejemplo, el ID de usuario de Firebase Authentication
        idJugador = "ID_DEL_JUGADOR";

        // Recoge las posiciones de los elementos al inicio del juego
        RecogerPosicionesIniciales();
    }

    // Método para recoger las posiciones iniciales de los elementos desde la base de datos
    void RecogerPosicionesIniciales()
    {
        // Accede a la referencia de la base de datos que almacena las posiciones iniciales de los elementos
        DatabaseReference referenciaPosiciones = referenciaBaseDatos.Child("posicionesElementos");

        // Suscribe un listener a la referencia de la base de datos para escuchar cambios en las posiciones
        referenciaPosiciones.ValueChanged += ManejarCambiosPosicionesIniciales;
    }

    // Maneja los cambios en las posiciones iniciales de los elementos
	void ManejarCambiosPosicionesIniciales(object sender, ValueChangedEventArgs args)
	{
    	if (args.DatabaseError != null)
    	{
        	Debug.LogError("Error al obtener posiciones iniciales: " + args.DatabaseError.Message);
        	return;
    	}

   		// Parsea los datos recibidos desde la base de datos
    	// Por ejemplo, asumiendo que los datos son un diccionario donde la clave es el nombre del objeto y el valor es la posición
    	Dictionary<string, object> datosPosiciones = (Dictionary<string, object>)args.Snapshot.Value;

    	// Itera sobre los datos y posiciona los elementos en la escena de Unity
    	foreach (var entradaPosicion in datosPosiciones)
    	{
        	string nombreObjeto = entradaPosicion.Key;
        	Vector3 posicion = ParsearPosicion(entradaPosicion.Value);

        	// Verifica si el prefab correspondiente existe
        	GameObject prefab = Resources.Load<GameObject>(nombreObjeto);
        	if (prefab == null)
        	{
            	Debug.LogError("Prefab '" + nombreObjeto + "' no encontrado en Resources.");
            	continue;
        	}

        	// Instancia el prefab correspondiente y lo posiciona en la escena
        	GameObject instanciaPrefab = Instantiate(prefab, posicion, Quaternion.identity);
    	}
	}

    // Método para parsear la posición desde los datos de la base de datos
    Vector3 ParsearPosicion(object datosPosicion)
    {
        // Aquí debes implementar la lógica de cómo se almacenan las posiciones en tu base de datos
        // Por ejemplo, si las posiciones se almacenan como un diccionario con claves x, y, z
        Dictionary<string, object> diccionarioPosicion = (Dictionary<string, object>)datosPosicion;
        float x = float.Parse(diccionarioPosicion["x"].ToString());
        float y = float.Parse(diccionarioPosicion["y"].ToString());
        float z = float.Parse(diccionarioPosicion["z"].ToString());
        return new Vector3(x, y, z);
    }

    // Método para actualizar los datos del jugador en la base de datos
    void ActualizarDatosJugador(int puntos, int vidas)
    {
        // Accede a la referencia del jugador en la base de datos
        DatabaseReference referenciaJugador = referenciaBaseDatos.Child("jugadores").Child(idJugador);

        // Actualiza los datos del jugador en la base de datos
        referenciaJugador.Child("puntos").SetValueAsync(puntos);
        referenciaJugador.Child("vidas").SetValueAsync(vidas);
    }

    // Método para mover un objeto en la escena y actualizar su posición en la base de datos
    void MoverObjeto(Vector3 nuevaPosicion)
    {
        // Actualiza la posición del objeto en la base de datos
        DatabaseReference referenciaObjeto = referenciaBaseDatos.Child("objetos").Child("nombre_del_objeto");
        referenciaObjeto.Child("posicion").SetValueAsync(nuevaPosicion);

        // Mueve el objeto en la escena
        // Aquí debes implementar la lógica para mover el objeto en la escena de Unity
    }
}
