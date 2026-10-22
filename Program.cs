using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace CrudGastos

{
    internal class Program
    {
        static void Main(string[] args)
        {
            GastoRepositorio repo = new GastoRepositorio();
            string opcion;

            do
            {
                opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case "1":
                        RegistrarGasto(repo);
                        break;
                    case "2":
                        ListarGastos(repo);
                        break;
                    case "3":
                        BuscarGasto(repo);
                        break;
                    case "4":
                        ActualizarMonto(repo);
                        break;
                    case "5":
                        EliminarGasto(repo);
                        break;
                    case "6":
                        Console.WriteLine("Saliendo del sistema...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida, intenta de nuevo.");
                        break;
                }
            }
            while (opcion != "6");
        }

        private static string LeerOpcionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== MENÚ GASTOS ===");
            Console.WriteLine("1. Registrar gasto");
            Console.WriteLine("2. Listar gastos");
            Console.WriteLine("3. Buscar gasto por Id");
            Console.WriteLine("4. Actualizar monto");
            Console.WriteLine("5. Eliminar gasto");
            Console.WriteLine("6. Salir");
            Console.Write("Elige una opción: ");
            return Console.ReadLine();
        }

        private static void RegistrarGasto(GastoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine().Trim();

            Console.Write("Monto: ");
            bool montoOk = decimal.TryParse(Console.ReadLine(), out decimal monto);

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine().Trim();

            if (!montoOk || monto <= 0
                || string.IsNullOrWhiteSpace(descripcion)
                || string.IsNullOrWhiteSpace(categoria))
            {
                Console.WriteLine("Datos inválidos.");
                return;
            }

            try
            {
                repo.Insertar(new Gasto
                {
                    Descripcion = descripcion,
                    Monto = monto,
                    Categoria = categoria
                });
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al registrar: " + ex.Message);
            }
        }

        private static void ListarGastos(GastoRepositorio repo)
        {
            try
            {
                List<Gasto> lista = repo.ListarTodos();

                if (lista.Count == 0)
                {
                    Console.WriteLine("No hay gastos registrados.");
                    return;
                }

                decimal totalGeneral = 0;
                foreach (Gasto g in lista)
                {
                    Console.WriteLine(g);
                    totalGeneral += g.Monto;
                }

                Console.WriteLine($"TOTAL GASTADO: Q {totalGeneral:N2}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al listar: " + ex.Message);
            }
        }

        private static void BuscarGasto(GastoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Id a buscar: ");
            bool idOk = int.TryParse(Console.ReadLine(), out int id);

            if (!idOk)
            {
                Console.WriteLine("Id inválido.");
                return;
            }

            try
            {
                Gasto? g = repo.BuscarPorId(id);

                if (g == null)
                {
                    Console.WriteLine("No existe ese Id.");
                    return;
                }

                Console.WriteLine(g);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al buscar: " + ex.Message);
            }
        }

        private static void ActualizarMonto(GastoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Id del gasto a actualizar: ");
            bool idOk = int.TryParse(Console.ReadLine(), out int id);

            if (!idOk)
            {
                Console.WriteLine("Id inválido.");
                return;
            }

            try
            {
                Gasto? g = repo.BuscarPorId(id);

                if (g == null)
                {
                    Console.WriteLine("No existe ese Id.");
                    return;
                }

                Console.WriteLine("Gasto actual: " + g);
                Console.Write("Nuevo monto: ");
                bool montoOk = decimal.TryParse(Console.ReadLine(), out decimal monto);

                if (!montoOk || monto <= 0)
                {
                    Console.WriteLine("Monto inválido.");
                    return;
                }

                g.Monto = monto;

                if (repo.Actualizar(g))
                {
                    Console.WriteLine("Gasto actualizado.");
                }
                else
                {
                    Console.WriteLine("No existe ese Id.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al actualizar: " + ex.Message);
            }
        }

        private static void EliminarGasto(GastoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Id del gasto a eliminar: ");
            bool idOk = int.TryParse(Console.ReadLine(), out int id);

            if (!idOk)
            {
                Console.WriteLine("Id inválido.");
                return;
            }

            try
            {
                if (repo.Eliminar(id))
                {
                    Console.WriteLine("Gasto eliminado.");
                }
                else
                {
                    Console.WriteLine("No existe ese Id.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al eliminar: " + ex.Message);
            }
        }
    }
}