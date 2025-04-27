using Microsoft.AspNetCore.Mvc;
using MvcRentACarAzure.Filters;
using MvcRentACarAzure.Services;
using NugetRentACar.Models;
using System;

namespace MvcRentACarAzure.Controllers
{
    public class CompradoresController : Controller
    {
        private IServiceRentACar service;

        public CompradoresController(IServiceRentACar service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Coches()
        {
            List<VistaCoche> coches = await this.service.FindAllCochesAsync();
            return View(coches);
        }

        [HttpPost]
        public async Task<IActionResult> Coches(int valor)
        {
            List<VistaCoche> coches = await this.service.FilterByPrecio(valor);
            return View(coches);
        }

        public async Task<IActionResult> FilterCoches(string search, string sort, string marcha, int? puertas, string combustible)
        {
            List<VistaCoche> coches = await this.service.FindAllCochesAsync();

            if (!string.IsNullOrEmpty(search))
            {
                coches = coches.Where(c => c.Marca.Contains(search, StringComparison.OrdinalIgnoreCase) || c.Modelo.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(marcha))
            {
                coches = coches.Where(c => c.Marcha.Equals(marcha, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (puertas.HasValue)
            {
                coches = coches.Where(c => c.Puertas == puertas.Value).ToList();
            }

            if (!string.IsNullOrEmpty(combustible))
            {
                coches = coches.Where(c => c.Combustible.Equals(combustible, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            switch (sort)
            {
                case "precio_asc":
                    coches = coches.OrderBy(c => c.PrecioKilometros).ToList();
                    break;
                case "precio_desc":
                    coches = coches.OrderByDescending(c => c.PrecioKilometros).ToList();
                    break;
            }

            return View("Coches", coches);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> DetailsCoche(int id)
        {
            VistaCoche coche = await this.service.GetVistaCocheAsync(id);
            return View(coche);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservasPorCocheComprador(int idcoche)
        {
            List<VistaReserva> reservas = await this.service.GetVistaReservasAsync();
            reservas = reservas.Where(r => r.IdCoche == idcoche).ToList();

            string color = "#FF5733";

            var eventos = reservas.Select(r => new
            {
                title = $"{r.Marca} {r.Modelo}",
                start = r.FechaInicio.ToString("yyyy-MM-dd"),
                end = r.FechaFin.AddDays(1).ToString("yyyy-MM-dd"),
                color = color
            }).ToList();

            return Json(eventos);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> DetailsCoche(int idcoche, DateTime fechainicio, DateTime fechafin, double valor, bool kilometraje)
        {
            try
            {
                int idusuario = int.Parse(HttpContext.User.FindFirst("id").Value);

                await this.service.CompraCocheAsync(idusuario, idcoche, fechainicio, fechafin, valor, kilometraje);

                TempData["SuccessMessage"] = "Reserva realizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            VistaCoche coche = await this.service.GetVistaCocheAsync(idcoche);
            return View(coche);
        }


        [AuthorizeUsers]
        public async Task<IActionResult> Compras(string estadoReserva)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("id").Value);
            List<Compra> compras = await this.service.GetComprasUsuarioAsync(idusuario, estadoReserva);
            ViewData["EstadoReservaSeleccionado"] = estadoReserva;
            return View(compras);
        }

        public async Task<IActionResult> ComprasListPartial(string estadoReserva)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("id").Value);
            List<Compra> compras = await this.service.GetComprasUsuarioAsync(idusuario, estadoReserva);
            return PartialView("_ComprasList", compras);
        }
    }
}
