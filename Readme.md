## Script Unity

Este script de Unity, llamado `Realtime`, permite interactuar con una base de datos en tiempo real utilizando Firebase. Proporciona funcionalidades como la:
- Recogida de las posiciones iniciales de los prefabs.
- Actualización de datos del jugador.
- Actualizar en tiempo real lo que ocurre en el juego.

## Descripción del Código

### Inicialización y Configuración de Firebase

En el método `Start()`, se inicializa Firebase y se establece una referencia a la base de datos:

```csharp
void Start()
    {
        // Inicializa Firebase
        referenciaBaseDatos = FirebaseDatabase.DefaultInstance.RootReference;

        // Genera un ID de jugador único, por ejemplo, el ID de usuario de Firebase Authentication
        idJugador = "ID_DEL_JUGADOR";

        // Recoge las posiciones de los elementos al inicio del juego
        RecogerPosicionesIniciales();
    }
```
### Recogida de Posiciones Iniciales
El método `RetrieveInitialPositions()` se encarga de suscribir un listener a la referencia de la base de datos que almacena las posiciones iniciales de los elementos:

```csharp
void RecogerPosicionesIniciales()
    {
        // Accede a la referencia de la base de datos que almacena las posiciones iniciales de los elementos
        DatabaseReference referenciaPosiciones = referenciaBaseDatos.Child("posicionesElementos");

        // Suscribe un listener a la referencia de la base de datos para escuchar cambios en las posiciones
        referenciaPosiciones.ValueChanged += ManejarCambiosPosicionesIniciales;
    }
```
### Manejo de Cambios en las Posiciones Iniciales
El método `HandleInitialPositionsChanged` se ejecuta cada vez que cambian las posiciones iniciales en la base de datos.
Recupera los datos de la base de datos y posiciona los elementos en la escena de Unity:

```csharp
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
```

### Actualización de Datos del Jugador
El método `UpdatePlayerData()` se encarga de actualizar los datos del jugador en mi caso `puntos` y `vidas` en la base de datos:

```csharp
void ActualizarDatosJugador(int puntos, int vidas)
    {
        // Accede a la referencia del jugador en la base de datos
        DatabaseReference referenciaJugador = referenciaBaseDatos.Child("jugadores").Child(idJugador);

        // Actualiza los datos del jugador en la base de datos
        referenciaJugador.Child("puntos").SetValueAsync(puntos);
        referenciaJugador.Child("vidas").SetValueAsync(vidas);
    }
```

### Movimiento de Objetos en la Escena
El método `MoveObject()` se encarga de actualizar la posicion de un objeto en la escena y actualizar su posición en la base de datos:

```csharp
void MoverObjeto(Vector3 nuevaPosicion)
    {
        // Actualiza la posición del objeto en la base de datos
        DatabaseReference referenciaObjeto = referenciaBaseDatos.Child("objetos").Child("nombre_del_objeto");
        referenciaObjeto.Child("posicion").SetValueAsync(nuevaPosicion);

        // Mueve el objeto en la escena
        // Aquí debes implementar la lógica para mover el objeto en la escena de Unity
    }
```
