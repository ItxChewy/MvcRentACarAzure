using Microsoft.AspNetCore.Mvc;
using MvcRentACarAzure.Filters;
using MvcRentACarAzure.Services;
using NugetRentACar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcRentACarAzure.Controllers
{
    public class VendedoresController : Controller
    {
        private IServiceRentACar service;
        public VendedoresController(IServiceRentACar service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeUsers(Policy = "Admin")]
        public async Task<IActionResult> Coches()
        {
            TempData["WarningMessage"] = "¿Estas seguro de eliminar el coche?";
            TempData["WarningMessageBtn"] = "Eliminar";
            List<VistaCoche> coches = await this.service.GetCochesAsync();
            return View(coches);
        }

        [AuthorizeUsers(Policy = "Admin")]
        public async Task<IActionResult> ManageCoche(int idcoche)
        {
            ViewData["coche"] = await this.service.DetailsCocheAsync(idcoche);
            ViewData["estado"] = await this.service.GetEstadoReservaAsync();
            List<Reserva> reservas = await this.service.GetReservasNoFinalizadasPorCocheAsync(idcoche);
            return View(reservas);
        }

        [AuthorizeUsers(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ManageCoche(int idcoche, int? setactive, int? finalizado)
        {
            ViewData["coche"] = await this.service.DetailsCocheAsync(idcoche);
            ViewData["estado"] = await this.service.GetEstadoReservaAsync();

            try
            {
                if (setactive != null)
                {
                    bool result = await this.service.CambiarAEstadoActivoReservaAsync((int)setactive);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "La reserva ha sido activada correctamente.";
                    }
                }
                else if (finalizado != null)
                {
                    await this.service.CambiarAEstadoFinalizadoReservaAsync((int)finalizado);
                    TempData["SuccessMessage"] = "La reserva ha sido finalizada correctamente.";
                }
            }
            catch (Exception ex)
            {
                // Capture and display the exception message
                TempData["ErrorMessage"] = ex.Message;
            }

            // Always reload reservations regardless of success or failure
            List<Reserva> reservas = await this.service.GetReservasNoFinalizadasPorCocheAsync(idcoche);
            return View(reservas);
        }

        [AuthorizeUsers(Policy = "Admin")]
        public async Task<IActionResult> ComprobarKilometraje(int? filtro)
        {
            List<Reserva> reservas = await this.service.GetReservasFilterAsync();

            if (filtro.HasValue && filtro.Value != 0)
            {
                if (filtro.Value == 1)
                {
                    reservas = reservas.Where(r => r.Kilometraje == true).ToList();
                }
                else if (filtro.Value == 2)
                {
                    reservas = reservas.Where(r => r.Kilometraje == false).ToList();
                }
            }

            // Load car data for each reservation
            foreach (var reserva in reservas)
            {
                reserva.Coche = await this.service.DetailsCocheAsync(reserva.IdCoche);
            }

            ViewData["Filtro"] = filtro ?? 0;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ComprobarKilometrajeTable", reservas);
            }

            return View(reservas);
        }

        [AuthorizeUsers(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ComprobarKilometraje(int idreserva, int newkilometraje)
        {
            await this.service.GetCargoExcedido(idreserva, newkilometraje);

            TempData["SuccessMessage"] = "Kilometraje actualizado correctamente.";

            List<Reserva> reservas = await this.service.GetReservasFilterAsync();

            int filtro = TempData["FiltroActual"] != null ? (int)TempData["FiltroActual"] : 0;
            ViewData["Filtro"] = filtro;

            if (filtro == 1)
            {
                reservas = reservas.Where(r => r.Kilometraje == true).ToList();
            }
            else if (filtro == 2)
            {
                reservas = reservas.Where(r => r.Kilometraje == false).ToList();
            }

            // Load car data for each reservation
            foreach (var reserva in reservas)
            {
                reserva.Coche = await this.service.DetailsCocheAsync(reserva.IdCoche);
            }

            return View(reservas);
        }

        public async Task<IActionResult> ConfirmarKilometraje(int idreserva)
        {
            TempData["WarningMessage"] = "¿Estás seguro de actualizar el kilometraje?";
            TempData["WarningMessageBtn"] = "Actualizar";
            TempData["IdReservaKm"] = idreserva;
            return RedirectToAction("ComprobarKilometraje");
        }

        [HttpGet]
        public async Task<IActionResult> GetReservasConCoche()
        {
            List<VistaReserva> reservas = await this.service.GetVistaReservasAsync();

            string[] colors = new[] { "#FF5733", "#33FF57", "#3357FF", "#F1C40F", "#9B59B6", "#1ABC9C" };
            Dictionary<int, string> carColorMap = new Dictionary<int, string>();
            int colorIndex = 0;

            var eventos = reservas.Select(r =>
            {
                if (!carColorMap.ContainsKey(r.IdCoche))
                {
                    carColorMap[r.IdCoche] = colors[colorIndex % colors.Length];
                    colorIndex++;
                }
                return new
                {
                    title = $"{r.Marca} {r.Modelo}",
                    start = r.FechaInicio.ToString("yyyy-MM-dd"),
                    end = r.FechaFin.AddDays(1).ToString("yyyy-MM-dd"),
                    color = carColorMap[r.IdCoche]
                };
            }).ToList();

            return Json(eventos);
        }
    }
}
