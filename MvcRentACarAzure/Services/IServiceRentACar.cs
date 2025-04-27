using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NugetRentACar.Models;

namespace MvcRentACarAzure.Services
{
    /// <summary>
    /// Interfaz única para consumir la API de RentACar
    /// Esta interfaz permite cambiar fácilmente entre proveedores (Azure, AWS, etc.)
    /// </summary>
    public interface IServiceRentACar
    {
        #region Autenticación y Usuarios
        Task<Usuario> LoginAsync(string email, string password);
        Task<bool> RegisterUsuarioAsync(string nombre, string email, string password, int idrol, string telefono,
            string? apellidos, string? dni, DateTime? fechanacimiento, string? direccion, string? passpecial,
            string? nombreempresa);
        //Task<Usuario> GetUsuarioAsync();
        Task<Vendedor> GetVendedorAsync(int idusuario);
        Task<Vendedor> GetVendedorAsync();
        Task<List<Rol>> GetRolesAsync();
        //Task<int> GetMaxIdUser();
        //Task<int> GetMaxIdComprador();
        //Task<bool> CheckVendedor();
        #endregion

        #region Vehículos
        //Task<List<VistaCoche>> GetAllCochesAsync();
        Task<List<VistaCoche>> FindAllCochesAsync();
        Task<List<VistaCoche>> GetCochesAsync();
        Task<List<VistaCoche>> FilterByPrecio(int valor);
        Task<VistaCoche> GetVistaCocheAsync(int idcoche);
        Task<Coche> GetCocheAsync(int idcoche);
        Task<Coche> DetailsCocheAsync(int idcoche);
        //Task<bool> ComprobarDisponibilidadCocheAsync(int idcoche, DateTime fechainicio, DateTime fechafin);
        Task<bool> InsertCocheAsync(string marca, string modelo, string matricula, IFormFile fichero, int asientos, int idmarchas,
            int idgama, int kilometraje, int puertas, int idcombustible, int idvendedor,
            decimal preciokilometros, decimal precioilimitado);
        Task UpdateCocheAsync(int idcoche, int idgama, decimal preciokilometros, decimal precioilimitado);
        Task DeleteCocheAsync(int idcoche);
        #endregion

        #region Catálogos
        Task<List<Marchas>> GetMarchasAsync();
        Task<List<Gama>> GetGamasAsync();
        Task<List<Combustible>> GetCombustiblesAsync();
        Task<List<EstadoReserva>> GetEstadoReservaAsync();
        #endregion

        #region Reservas y Compras
        Task<List<VistaReserva>> GetVistaReservasAsync();
        Task<List<Reserva>> GetReservasFilterAsync();
        Task<List<Reserva>> GetReservasKilometrajeAsync();
        Task<List<Reserva>> GetReservasIlimitadosAsync();
        Task<List<Reserva>> GetReservasPorCocheAsync(int idcoche);
        Task<List<Reserva>> GetReservasNoFinalizadasPorCocheAsync(int idcoche);
        Task<Reserva> GetReservaAsync(int idreserva);
        Task<bool> CambiarAEstadoActivoReservaAsync(int idreserva);
        Task CambiarAEstadoFinalizadoReservaAsync(int idreserva);
        Task ActualizarKilometraje(int idreserva, int newkilometraje);
        Task GetCargoExcedido(int idreserva, int newkilometraje);
        Task CompraCocheAsync(int idusuario, int idcoche, DateTime fechainicio, DateTime fechafin, double precio, bool kilometraje);
        Task<List<Compra>> GetComprasUsuarioAsync(int idusuario, string estadoReserva = null);
        #endregion
        Task<string> GetTokenAsync(string userName, string password);
    }
}
