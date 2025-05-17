using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibretaContactos
{
    class Contacto
    {
        private const int LimiteCaracteres = 16;

        private string _nombre;
        private string _apellido;
        private string _telefono;
        private string _email;

        public string Nombre
        {
            get => _nombre;
            set => _nombre = value.Length <= LimiteCaracteres ? value : value.Substring(0, LimiteCaracteres);
        }

        public string Apellido
        {
            get => _apellido;
            set => _apellido = value.Length <= LimiteCaracteres ? value : value.Substring(0, LimiteCaracteres);
        }

        public string Telefono
        {
            get => _telefono;
            set => _telefono = value;
        }

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public override string ToString()
        {
            return $"{Nombre} {Apellido} | Tel: {Telefono} | Email: {Email}";
        }

        public string ToCSV()
        {
            return $"{Nombre},{Apellido},{Telefono},{Email}";
        }

        public static Contacto FromCSV(string csv)
        {
            string[] parts = csv.Split(',');
            if (parts.Length < 4) return null;

            return new Contacto
            {
                Nombre = parts[0],
                Apellido = parts[1],
                Telefono = parts[2],
                Email = parts[3]
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is Contacto otroContacto)
            {
                return Nombre.Equals(otroContacto.Nombre, StringComparison.OrdinalIgnoreCase) &&
                       Apellido.Equals(otroContacto.Apellido, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Nombre?.GetHashCode() ?? 0) ^ (Apellido?.GetHashCode() ?? 0);
        }
    }

    class LibretaDeContactos
    {
        private List<Contacto> contactos = new List<Contacto>();
        private const string ArchivoContactos = "contactos.csv";
        private bool cambiosSinGuardar = false;

        public void CargarContactos()
        {
            Console.WriteLine("\n===== CARGAR CONTACTOS =====");
            Console.Write("Ingrese el nombre del archivo (deje vacío para cancelar): ");
            string nombreArchivo = Console.ReadLine()?.Trim();

            // Verificar si el nombre de archivo está vacío
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                Console.WriteLine("Operación cancelada. No se especificó un archivo.");
                return;
            }

            try
            {
                // Verificar si el archivo existe
                if (File.Exists(nombreArchivo))
                {
                    contactos.Clear();
                    string[] lineas = File.ReadAllLines(nombreArchivo);
                    foreach (string linea in lineas)
                    {
                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            Contacto contacto = Contacto.FromCSV(linea);
                            if (contacto != null)
                            {
                                contactos.Add(contacto);
                            }
                        }
                    }
                    Console.WriteLine($"¡Éxito! Se cargaron {contactos.Count} contactos del archivo '{nombreArchivo}'.");
                }
                else
                {
                    Console.WriteLine($"Error: El archivo '{nombreArchivo}' no existe.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo: {ex.Message}");
            }
        }

        public void MostrarContactos()
        {
            if (contactos.Count == 0)
            {
                Console.WriteLine("No hay contactos para mostrar.");
                return;
            }

            // Crear una copia de la lista para poder ordenarla
            List<Contacto> contactosMostrados = new List<Contacto>(contactos);

            // Solicitar criterio de ordenamiento
            Console.WriteLine("\n===== ORDENAR CONTACTOS POR =====");
            Console.WriteLine("1. Nombre");
            Console.WriteLine("2. Apellido");
            Console.WriteLine("3. Teléfono");
            Console.WriteLine("4. Email");
            Console.Write("\nSeleccione una opción (o presione Enter para no ordenar): ");
            string opcionOrden = Console.ReadLine();

            // Ordenar según la opción seleccionada
            switch (opcionOrden)
            {
                case "1":
                    contactosMostrados = contactosMostrados.OrderBy(c => c.Nombre).ToList();
                    Console.WriteLine("Contactos ordenados por Nombre.");
                    break;
                case "2":
                    contactosMostrados = contactosMostrados.OrderBy(c => c.Apellido).ToList();
                    Console.WriteLine("Contactos ordenados por Apellido.");
                    break;
                case "3":
                    contactosMostrados = contactosMostrados.OrderBy(c => c.Telefono).ToList();
                    Console.WriteLine("Contactos ordenados por Teléfono.");
                    break;
                case "4":
                    contactosMostrados = contactosMostrados.OrderBy(c => c.Email).ToList();
                    Console.WriteLine("Contactos ordenados por Email.");
                    break;
            }

            const int contactosPorPagina = 10;
            int totalPaginas = (int)Math.Ceiling(contactosMostrados.Count / (double)contactosPorPagina);
            int paginaActual = 1;
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine($"\n===== LISTA DE CONTACTOS (Página {paginaActual} de {totalPaginas}) =====");
                Console.WriteLine("ID | Nombre y Apellido | Teléfono | Email");
                Console.WriteLine("--------------------------------------");

                // Calcular índices para la página actual
                int inicio = (paginaActual - 1) * contactosPorPagina;
                int fin = Math.Min(inicio + contactosPorPagina, contactosMostrados.Count);

                // Mostrar contactos de la página actual
                for (int i = inicio; i < fin; i++)
                {
                    Console.WriteLine($"{i + 1}. {contactosMostrados[i]}");
                }

                Console.WriteLine($"\nMostrando {fin - inicio} de {contactosMostrados.Count} contactos totales");

                // Menú de navegación
                Console.WriteLine("\n===== OPCIONES =====");
                Console.WriteLine("M. Volver al menú principal");

                if (paginaActual > 1)
                    Console.WriteLine("P. Página anterior");

                if (paginaActual < totalPaginas)
                    Console.WriteLine("N. Página siguiente");

                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine()?.ToUpper();

                switch (opcion)
                {
                    case "M":
                        salir = true;
                        break;
                    case "P":
                        if (paginaActual > 1)
                            paginaActual--;
                        break;
                    case "N":
                        if (paginaActual < totalPaginas)
                            paginaActual++;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public bool AñadirContacto()
        {
            Console.Clear();
            Console.WriteLine("\n===== AÑADIR NUEVO CONTACTO =====");

            Contacto nuevoContacto = new Contacto();

            // Solicitar Nombre (obligatorio, máximo 16 caracteres)
            while (true)
            {
                Console.Write("Nombre (máximo 16 caracteres): ");
                string nombre = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("Error: El nombre es obligatorio.");
                }
                else if (nombre.Length > 16)
                {
                    Console.WriteLine("Error: El nombre no puede tener más de 16 caracteres.");
                }
                else
                {
                    nuevoContacto.Nombre = nombre;
                    break;
                }
            }

            // Solicitar Apellido (obligatorio, máximo 16 caracteres)
            while (true)
            {
                Console.Write("Apellido (máximo 16 caracteres): ");
                string apellido = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(apellido))
                {
                    Console.WriteLine("Error: El apellido es obligatorio.");
                }
                else if (apellido.Length > 16)
                {
                    Console.WriteLine("Error: El apellido no puede tener más de 16 caracteres.");
                }
                else
                {
                    nuevoContacto.Apellido = apellido;
                    break;
                }
            }

            // Solicitar Teléfono
            Console.Write("Teléfono: ");
            nuevoContacto.Telefono = Console.ReadLine()?.Trim() ?? "";

            // Solicitar Email
            Console.Write("Email: ");
            nuevoContacto.Email = Console.ReadLine()?.Trim() ?? "";

            // Validar que teléfono y email no estén ambos en blanco
            if (string.IsNullOrWhiteSpace(nuevoContacto.Telefono) && string.IsNullOrWhiteSpace(nuevoContacto.Email))
            {
                Console.WriteLine("Error: El teléfono y email no pueden estar ambos en blanco.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return false;
            }

            // Mostrar información del contacto y confirmar
            Console.WriteLine("\nInformación del nuevo contacto:");
            Console.WriteLine(nuevoContacto);
            Console.Write("\n¿Desea añadir este contacto? (S/N): ");
            string confirmacion = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (confirmacion == "S")
            {
                contactos.Add(nuevoContacto);
                Console.WriteLine("\n¡Éxito! El contacto ha sido añadido correctamente.");
                cambiosSinGuardar = true;
                return true;
            }
            else
            {
                Console.WriteLine("\nOperación cancelada. El contacto no ha sido añadido.");
                return false;
            }
        }


        public bool EditarContacto()
        {
            if (contactos.Count == 0)
            {
                Console.WriteLine("No hay contactos para editar.");
                return false;
            }

            Console.Clear();
            Console.WriteLine("\n===== EDITAR CONTACTO =====");
            Console.WriteLine("Ingrese el índice o campo para buscar el contacto (deje vacío para cancelar)");
            Console.Write("Búsqueda: ");
            string busqueda = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(busqueda))
            {
                Console.WriteLine("\nOperación cancelada.");
                return false;
            }

            // Buscar contactos que coincidan
            List<Contacto> coincidencias = new List<Contacto>();
            List<int> indicesCoincidencias = new List<int>();

            // Intentar interpretar la búsqueda como un índice numérico
            if (int.TryParse(busqueda, out int indice) && indice > 0 && indice <= contactos.Count)
            {
                coincidencias.Add(contactos[indice - 1]);
                indicesCoincidencias.Add(indice - 1);
            }
            else
            {
                // Buscar coincidencias en todos los campos
                for (int i = 0; i < contactos.Count; i++)
                {
                    Contacto c = contactos[i];
                    if (c.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                        c.Apellido.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                        c.Telefono.Contains(busqueda) ||
                        c.Email.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
                    {
                        coincidencias.Add(c);
                        indicesCoincidencias.Add(i);
                    }
                }
            }

            // Mostrar resultados
            if (coincidencias.Count == 0)
            {
                Console.WriteLine("\nNo se encontraron contactos que coincidan con la búsqueda.");
                return false;
            }

            Console.WriteLine($"\nSe encontraron {coincidencias.Count} coincidencias:");
            for (int i = 0; i < coincidencias.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {coincidencias[i]}");
            }

            // Seleccionar contacto a editar
            Console.Write("\nSeleccione el número del contacto a editar (0 para cancelar): ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) ||
                seleccion < 0 ||
                seleccion > coincidencias.Count)
            {
                Console.WriteLine("Selección no válida.");
                return false;
            }

            if (seleccion == 0)
            {
                Console.WriteLine("\nOperación cancelada.");
                return false;
            }

            // Obtener el contacto seleccionado y su índice en la lista original
            Contacto contactoAEditar = coincidencias[seleccion - 1];
            int indiceOriginal = indicesCoincidencias[seleccion - 1];

            // Menú de edición
            Console.Clear();
            Console.WriteLine("\n===== EDITANDO CONTACTO =====");
            Console.WriteLine(contactoAEditar);
            Console.WriteLine("\nSeleccione el campo a editar:");
            Console.WriteLine("1. Nombre");
            Console.WriteLine("2. Apellido");
            Console.WriteLine("3. Teléfono");
            Console.WriteLine("4. Email");
            Console.WriteLine("5. Todos los campos");
            Console.WriteLine("0. Cancelar");

            Console.Write("\nOpción: ");
            if (!int.TryParse(Console.ReadLine(), out int opcion) || opcion < 0 || opcion > 5)
            {
                Console.WriteLine("Opción no válida.");
                return false;
            }

            if (opcion == 0)
            {
                Console.WriteLine("\nOperación cancelada.");
                return false;
            }

            // Crear una copia del contacto para editar
            Contacto contactoEditado = new Contacto
            {
                Nombre = contactoAEditar.Nombre,
                Apellido = contactoAEditar.Apellido,
                Telefono = contactoAEditar.Telefono,
                Email = contactoAEditar.Email
            };

            // Editar el campo seleccionado
            switch (opcion)
            {
                case 1: // Nombre
                    Console.Write($"Nuevo nombre [{contactoEditado.Nombre}]: ");
                    string nuevoNombre = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoNombre))
                        contactoEditado.Nombre = nuevoNombre;
                    break;
                case 2: // Apellido
                    Console.Write($"Nuevo apellido [{contactoEditado.Apellido}]: ");
                    string nuevoApellido = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoApellido))
                        contactoEditado.Apellido = nuevoApellido;
                    break;
                case 3: // Teléfono
                    Console.Write($"Nuevo teléfono [{contactoEditado.Telefono}]: ");
                    string nuevoTelefono = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoTelefono))
                        contactoEditado.Telefono = nuevoTelefono;
                    break;
                case 4: // Email
                    Console.Write($"Nuevo email [{contactoEditado.Email}]: ");
                    string nuevoEmail = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoEmail))
                        contactoEditado.Email = nuevoEmail;
                    break;
                case 5: // Todos los campos
                    Console.Write($"Nuevo nombre [{contactoEditado.Nombre}]: ");
                    nuevoNombre = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoNombre))
                        contactoEditado.Nombre = nuevoNombre;

                    Console.Write($"Nuevo apellido [{contactoEditado.Apellido}]: ");
                    nuevoApellido = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoApellido))
                        contactoEditado.Apellido = nuevoApellido;

                    Console.Write($"Nuevo teléfono [{contactoEditado.Telefono}]: ");
                    nuevoTelefono = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoTelefono))
                        contactoEditado.Telefono = nuevoTelefono;

                    Console.Write($"Nuevo email [{contactoEditado.Email}]: ");
                    nuevoEmail = Console.ReadLine()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nuevoEmail))
                        contactoEditado.Email = nuevoEmail;
                    break;
            }

            // Validar que nombre y apellido no estén ambos en blanco
            if (string.IsNullOrWhiteSpace(contactoEditado.Nombre) && string.IsNullOrWhiteSpace(contactoEditado.Apellido))
            {
                Console.WriteLine("Error: El nombre y apellido no pueden estar ambos en blanco.");
                return false;
            }

            // Validar que teléfono y email no estén ambos en blanco
            if (string.IsNullOrWhiteSpace(contactoEditado.Telefono) && string.IsNullOrWhiteSpace(contactoEditado.Email))
            {
                Console.WriteLine("Error: El teléfono y email no pueden estar ambos en blanco.");
                return false;
            }

            // Mostrar información del contacto editado y confirmar
            Console.WriteLine("\nInformación del contacto editado:");
            Console.WriteLine(contactoEditado);
            Console.Write("\n¿Desea guardar los cambios? (S/N): ");
            string confirmacion = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (confirmacion == "S")
            {
                contactos[indiceOriginal] = contactoEditado;
                Console.WriteLine("\n¡Éxito! El contacto ha sido actualizado correctamente.");
                cambiosSinGuardar = true;
                return true;
            }
            else
            {
                Console.WriteLine("\nOperación cancelada. El contacto no ha sido actualizado.");
                return false;
            }
        }

        public bool EliminarContacto()
        {
            if (contactos.Count == 0)
            {
                Console.WriteLine("No hay contactos para eliminar.");
                return false;
            }

            Console.Clear();
            Console.WriteLine("\n===== ELIMINAR CONTACTO =====");
            Console.WriteLine("Ingrese el índice o campo para buscar el contacto (deje vacío para cancelar)");
            Console.Write("Búsqueda: ");
            string busqueda = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(busqueda))
            {
                Console.WriteLine("\nOperación cancelada.");
                return false;
            }

            // Buscar contactos que coincidan
            List<Contacto> coincidencias = new List<Contacto>();
            List<int> indicesCoincidencias = new List<int>();

            // Intentar interpretar la búsqueda como un índice numérico
            if (int.TryParse(busqueda, out int indice) && indice > 0 && indice <= contactos.Count)
            {
                coincidencias.Add(contactos[indice - 1]);
                indicesCoincidencias.Add(indice - 1);
            }
            else
            {
                // Buscar coincidencias en todos los campos
                for (int i = 0; i < contactos.Count; i++)
                {
                    Contacto c = contactos[i];
                    if (c.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                        c.Apellido.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                        c.Telefono.Contains(busqueda) ||
                        c.Email.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
                    {
                        coincidencias.Add(c);
                        indicesCoincidencias.Add(i);
                    }
                }
            }

            // Mostrar resultados
            if (coincidencias.Count == 0)
            {
                Console.WriteLine("\nNo se encontraron contactos que coincidan con la búsqueda.");
                return false;
            }

            Console.WriteLine($"\nSe encontraron {coincidencias.Count} coincidencias:");
            for (int i = 0; i < coincidencias.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {coincidencias[i]}");
            }

            // Seleccionar contacto a eliminar
            Console.Write("\nSeleccione el número del contacto a eliminar (0 para cancelar): ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) ||
                seleccion < 0 ||
                seleccion > coincidencias.Count)
            {
                Console.WriteLine("Selección no válida.");
                return false;
            }

            if (seleccion == 0)
            {
                Console.WriteLine("\nOperación cancelada.");
                return false;
            }

            // Obtener el contacto seleccionado y su índice en la lista original
            Contacto contactoAEliminar = coincidencias[seleccion - 1];
            int indiceOriginal = indicesCoincidencias[seleccion - 1];

            // Confirmar eliminación
            Console.WriteLine("\nContacto a eliminar:");
            Console.WriteLine(contactoAEliminar);
            Console.Write("\n¿Está seguro que desea eliminar este contacto? (S/N): ");
            string confirmacion = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (confirmacion == "S")
            {
                contactos.RemoveAt(indiceOriginal);
                Console.WriteLine("\n¡Éxito! El contacto ha sido eliminado correctamente.");
                cambiosSinGuardar = true;
                return true;
            }
            else
            {
                Console.WriteLine("\nOperación cancelada. El contacto no ha sido eliminado.");
                return false;
            }
        }

        public bool FusionarContactosDuplicados()
        {
            if (contactos.Count <= 1)
            {
                Console.WriteLine("No hay suficientes contactos para buscar duplicados.");
                return false;
            }

            Console.Clear();
            Console.WriteLine("\n===== BUSCAR Y FUSIONAR DUPLICADOS =====");

            // Buscar duplicados
            Dictionary<string, List<int>> duplicadosNombre = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> duplicadosTelefono = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> duplicadosEmail = new Dictionary<string, List<int>>();

            // Buscar por nombre y apellido
            for (int i = 0; i < contactos.Count; i++)
            {
                string nombreCompleto = $"{contactos[i].Nombre.ToLower()} {contactos[i].Apellido.ToLower()}".Trim();
                if (!string.IsNullOrEmpty(nombreCompleto))
                {
                    if (!duplicadosNombre.ContainsKey(nombreCompleto))
                        duplicadosNombre[nombreCompleto] = new List<int>();

                    duplicadosNombre[nombreCompleto].Add(i);
                }

                // Buscar por teléfono
                if (!string.IsNullOrEmpty(contactos[i].Telefono))
                {
                    if (!duplicadosTelefono.ContainsKey(contactos[i].Telefono))
                        duplicadosTelefono[contactos[i].Telefono] = new List<int>();

                    duplicadosTelefono[contactos[i].Telefono].Add(i);
                }

                // Buscar por email
                if (!string.IsNullOrEmpty(contactos[i].Email))
                {
                    string email = contactos[i].Email.ToLower();
                    if (!duplicadosEmail.ContainsKey(email))
                        duplicadosEmail[email] = new List<int>();

                    duplicadosEmail[email].Add(i);
                }
            }

            // Filtrar solo los que tienen duplicados
            var gruposDuplicados = new List<List<int>>();
            foreach (var grupo in duplicadosNombre.Values.Where(v => v.Count > 1))
                gruposDuplicados.Add(grupo);

            foreach (var grupo in duplicadosTelefono.Values.Where(v => v.Count > 1))
                if (!YaExisteGrupo(gruposDuplicados, grupo))
                    gruposDuplicados.Add(grupo);

            foreach (var grupo in duplicadosEmail.Values.Where(v => v.Count > 1))
                if (!YaExisteGrupo(gruposDuplicados, grupo))
                    gruposDuplicados.Add(grupo);

            // Si no hay duplicados
            if (gruposDuplicados.Count == 0)
            {
                Console.WriteLine("No se encontraron contactos duplicados.");
                return false;
            }

            // Procesar cada grupo de duplicados
            bool cambiosRealizados = false;

            for (int i = 0; i < gruposDuplicados.Count; i++)
            {
                List<int> grupo = gruposDuplicados[i];
                Console.Clear();
                Console.WriteLine($"\n===== DUPLICADOS ENCONTRADOS (Grupo {i + 1}/{gruposDuplicados.Count}) =====");

                // Mostrar contactos duplicados
                Console.WriteLine("Contactos duplicados:");
                for (int j = 0; j < grupo.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {contactos[grupo[j]]}");
                }

                // Crear contacto fusionado
                Contacto contactoFusionado = CrearContactoFusionado(grupo);

                // Mostrar contacto fusionado
                Console.WriteLine("\nContacto fusionado propuesto:");
                Console.WriteLine(contactoFusionado);

                // Preguntar si añadir el contacto fusionado
                Console.Write("\n¿Desea añadir este contacto fusionado? (S/N): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (respuesta == "S")
                {
                    contactos.Add(contactoFusionado);
                    Console.WriteLine("\n¡Éxito! El contacto fusionado ha sido añadido.");
                    cambiosRealizados = true;
                    cambiosSinGuardar = true;

                    // Preguntar cuáles contactos eliminar
                    Console.WriteLine("\n¿Qué contactos duplicados desea eliminar?");
                    Console.WriteLine("Ingrese los números separados por comas (ej: 1,2), o 'todos' para eliminar todos los duplicados:");
                    string seleccion = Console.ReadLine()?.Trim().ToLower() ?? "";

                    if (seleccion == "todos")
                    {
                        // Eliminar todos los duplicados (de atrás hacia adelante para evitar problemas con los índices)
                        List<int> indicesAEliminar = new List<int>(grupo);
                        indicesAEliminar.Sort((a, b) => b.CompareTo(a)); // Ordenar de mayor a menor

                        foreach (int idx in indicesAEliminar)
                        {
                            contactos.RemoveAt(idx);
                        }

                        Console.WriteLine("Todos los contactos duplicados han sido eliminados.");
                    }
                    else
                    {
                        // Eliminar solo los seleccionados
                        string[] seleccionados = seleccion.Split(',');
                        List<int> indicesAEliminar = new List<int>();

                        foreach (string num in seleccionados)
                        {
                            if (int.TryParse(num, out int selIdx) && selIdx > 0 && selIdx <= grupo.Count)
                            {
                                indicesAEliminar.Add(grupo[selIdx - 1]);
                            }
                        }

                        // Ordenar de mayor a menor para evitar problemas con los índices
                        indicesAEliminar.Sort((a, b) => b.CompareTo(a));

                        foreach (int idx in indicesAEliminar)
                        {
                            contactos.RemoveAt(idx);
                        }

                        Console.WriteLine($"Se eliminaron {indicesAEliminar.Count} contactos seleccionados.");
                    }
                }
                else
                {
                    Console.WriteLine("No se añadió el contacto fusionado.");
                }

                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }

            if (cambiosRealizados)
            {
                Console.WriteLine("\nEl proceso de fusión de duplicados ha finalizado con éxito.");
                return true;
            }
            else
            {
                Console.WriteLine("\nNo se realizaron cambios en la lista de contactos.");
                return false;
            }
        }

        // Método auxiliar para verificar si un grupo ya existe entre los grupos de duplicados
        private bool YaExisteGrupo(List<List<int>> grupos, List<int> nuevoGrupo)
        {
            foreach (var grupo in grupos)
            {
                if (grupo.Intersect(nuevoGrupo).Any())
                    return true;
            }
            return false;
        }

        // Método auxiliar para crear un contacto fusionado a partir de varios contactos
        private Contacto CrearContactoFusionado(List<int> indices)
        {
            Contacto fusionado = new Contacto();

            // Usar el primer contacto como base
            fusionado.Nombre = contactos[indices[0]].Nombre;
            fusionado.Apellido = contactos[indices[0]].Apellido;
            fusionado.Telefono = contactos[indices[0]].Telefono;
            fusionado.Email = contactos[indices[0]].Email;

            // Completar con información de los otros contactos si falta algún dato
            foreach (int idx in indices.Skip(1))
            {
                if (string.IsNullOrEmpty(fusionado.Nombre) && !string.IsNullOrEmpty(contactos[idx].Nombre))
                    fusionado.Nombre = contactos[idx].Nombre;

                if (string.IsNullOrEmpty(fusionado.Apellido) && !string.IsNullOrEmpty(contactos[idx].Apellido))
                    fusionado.Apellido = contactos[idx].Apellido;

                if (string.IsNullOrEmpty(fusionado.Telefono) && !string.IsNullOrEmpty(contactos[idx].Telefono))
                    fusionado.Telefono = contactos[idx].Telefono;

                if (string.IsNullOrEmpty(fusionado.Email) && !string.IsNullOrEmpty(contactos[idx].Email))
                    fusionado.Email = contactos[idx].Email;
            }

            return fusionado;
        }

        public void GuardarContactos()
        {
            Console.Clear();
            Console.WriteLine("===== GUARDAR CONTACTOS EN ARCHIVO =====");

            Console.Write("Ingrese el nombre del archivo (sin dejar vacío): ");
            string nombreArchivo = Console.ReadLine()?.Trim();

            // Verifica si el nombre del archivo está vacío
            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                Console.WriteLine("Operación cancelada: No se proporcionó un nombre de archivo.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            // Agrega extensión .csv si no se proporciona
            if (!nombreArchivo.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                nombreArchivo += ".csv";
            }

            // Verifica si el archivo ya existe
            if (File.Exists(nombreArchivo))
            {
                Console.WriteLine($"El archivo '{nombreArchivo}' ya existe.");
                Console.Write("¿Desea sobrescribirlo? (S/N): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper();

                if (respuesta != "S")
                {
                    Console.WriteLine("Operación cancelada: No se sobrescribió el archivo existente.");
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
            }

            try
            {
                List<string> lineas = contactos.Select(c => c.ToCSV()).ToList();
                File.WriteAllLines(nombreArchivo, lineas);
                Console.WriteLine($"\n¡Éxito! Se guardaron {contactos.Count} contactos en el archivo '{nombreArchivo}'.");
                cambiosSinGuardar = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Ocurrió un problema al guardar el archivo. Detalles: {ex.Message}");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }
        public bool SalirAplicacion()
        {
            Console.Clear();
            Console.WriteLine("===== SALIR DE LA APLICACIÓN =====");

            if (cambiosSinGuardar)
            {
                Console.WriteLine("Hay cambios no guardados en la lista de contactos.");
                Console.Write("¿Desea salir y DESCARTAR los cambios? (S para salir / N para volver al menú): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper();

                if (respuesta == "S")
                {
                    Console.WriteLine("\nGracias por usar el gestor de contactos. ¡Hasta pronto!");
                    return true; // salir
                }
                else
                {
                    Console.WriteLine("\nRegresando al menú principal...");
                    return false; // no salir
                }
            }
            else
            {
                Console.Write("¿Está seguro que desea salir? (S para salir / N para volver al menú): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper();

                if (respuesta == "S")
                {
                    Console.WriteLine("\nGracias por usar el gestor de contactos. ¡Hasta pronto!");
                    return true; // salir
                }
                else
                {
                    Console.WriteLine("\nRegresando al menú principal...");
                    return false; // no salir
                }
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var libreta = new LibretaDeContactos();
            bool salir = false;

            Console.WriteLine("===== LIBRETA DE CONTACTOS =====");
            Console.WriteLine("     Creado por Neftalí Valentín");

            // Cargar contactos al iniciar
            libreta.CargarContactos();

            while (!salir)
            {
                MostrarMenu();
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                Console.WriteLine();
                switch (opcion)
                {
                    case "1":
                        libreta.CargarContactos();
                        break;
                    case "2":
                        libreta.MostrarContactos();
                        break;
                    case "3":
                        libreta.AñadirContacto();
                        break;
                    case "4":
                        libreta.EditarContacto();
                        break;
                    case "5":
                        libreta.EliminarContacto();
                        break;
                    case "6":
                        libreta.FusionarContactosDuplicados();
                        break;
                    case "7":
                        libreta.GuardarContactos();
                        break;
                    case "8":
                        libreta.SalirAplicacion();
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("===== MENÚ PRINCIPAL =====");
            Console.WriteLine("     Creado por Neftalí Valentín");
            Console.WriteLine("1. Cargar contactos desde archivo");
            Console.WriteLine("2. Mostrar todos los contactos");
            Console.WriteLine("3. Añadir un contacto");
            Console.WriteLine("4. Editar un contacto");
            Console.WriteLine("5. Eliminar un contacto");
            Console.WriteLine("6. Fusionar contactos duplicados");
            Console.WriteLine("7. Guardar contactos en archivo");
            Console.WriteLine("8. Salir");
        }
    }
}
