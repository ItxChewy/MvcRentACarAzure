using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcRentACarAzure.Services;
using NugetRentACar.Models;


namespace MvcRentACarAzure.Filters
{
    public class ViewDataFilter:IAsyncActionFilter
    {
        private IServiceRentACar service;
        public ViewDataFilter(IServiceRentACar service)
        {
            this.service = service;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            if (controller != null)
            {

                
                Vendedor vendedor = await service.GetVendedorAsync();
                if (vendedor == null)
                {
                    await next();
                }
                

                controller.ViewData["nombreempresa"] = vendedor?.NombreEmpresa ?? "Empresa por defecto";
                controller.ViewData["direccion"] = vendedor?.Direccion ?? "Dirección no disponible";
                controller.ViewData["telefono"] = vendedor?.Telefono ?? "Teléfono no disponible";
                //controller.ViewData["email"] = vendedor?.Email ?? "Email no disponible";
                controller.ViewData["nombre"] = vendedor?.NombreEmpresa ?? "Usuario desconocido";
            }

            await next(); // Continúa con la ejecución de la acción
        }
    }
}
