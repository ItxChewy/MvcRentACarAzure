using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetRentACar.Models;

namespace MvcRentACarAzure.Services
{
    public class ServiceRentACar : IServiceRentACar
    {
        private string Url;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;

        public ServiceRentACar(IConfiguration configuration,IHttpContextAccessor contextAccessor)
        {
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.Url = configuration.GetValue<string>("ApiUrls:ApiRentACar");
            this.contextAccessor = contextAccessor;
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response =
                    await client.GetAsync(this.Url + request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await
                        response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task<T> CallApiPostAsync<T>(string request, object obj)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string json = JsonConvert.SerializeObject(obj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync( this.Url + request, content);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        
        private async Task<T> CallApiPutAsync<T>(string request, object obj)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string json = JsonConvert.SerializeObject(obj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(this.Url + request, content);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<bool> CallApiDeleteAsync(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.DeleteAsync(this.Url + request);
                return response.IsSuccessStatusCode;
            }
        }
        private async Task<T> CallApiGetTokenAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.GetAsync(this.Url + request);

                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiPostTokenAsync<T>(string request, object obj)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string json = JsonConvert.SerializeObject(obj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(this.Url + request, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        string mensaje = errorObj?.message ?? "Error desconocido al hacer POST";
                        throw new Exception(mensaje);
                    }
                    catch
                    {
                        throw new Exception(responseContent);
                    }
                }
            }
        }

        private async Task<T> CallApiPutTokenAsync<T>(string request, object obj)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string json = JsonConvert.SerializeObject(obj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(this.Url + request, content);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<bool> CallApiDeleteTokenAsync(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.DeleteAsync(this.Url + request);
                return response.IsSuccessStatusCode;
            }
        }




        #region Vendedor Methods
        public async Task<List<VistaCoche>> GetCochesAsync()
        {
            string request = "/api/Vendedores/GetCoches";
            return await this.CallApiGetTokenAsync<List<VistaCoche>>(request);
        }

        public async Task<Coche> DetailsCocheAsync(int idcoche)
        {
            string request = $"/api/Vendedores/GetCoche/{idcoche}";
            return await this.CallApiGetTokenAsync<Coche>(request);
        }

        public async Task<List<Reserva>> GetReservasNoFinalizadasPorCocheAsync(int idcoche)
        {
            string request = $"/api/Vendedores/GetReservasNoFinalizadas/{idcoche}";
            return await this.CallApiGetTokenAsync<List<Reserva>>(request);
        }

        public async Task<List<EstadoReserva>> GetEstadoReservaAsync()
        {
            string request = "/api/Vendedores/GetEstadosReservas";
            return await this.CallApiGetTokenAsync<List<EstadoReserva>>(request);
        }

        public async Task<bool> CambiarAEstadoActivoReservaAsync(int idreserva)
        {
            string request = $"/api/Vendedores/ActivarReserva/{idreserva}";
            var result = await this.CallApiPostTokenAsync<dynamic>(request, new { });
            return result?.success ?? false;
        }

        public async Task CambiarAEstadoFinalizadoReservaAsync(int idreserva)
        {
            string request = $"/api/Vendedores/FinalizarReserva/{idreserva}";
            await this.CallApiPostTokenAsync<dynamic>(request, new { });
        }

        public async Task ActualizarKilometraje(int idreserva, int newkilometraje)
        {
            string request = $"/api/Vendedores/ActualizarKilometraje/{idreserva}";
            await this.CallApiPostTokenAsync<dynamic>(request, new { NuevoKilometraje = newkilometraje });
        }

        public async Task GetCargoExcedido(int idreserva, int newkilometraje)
        {
            await this.ActualizarKilometraje(idreserva, newkilometraje);
        }

        public async Task<List<Marchas>> GetMarchasAsync()
        {
            string request = "/api/Coches/GetMarchas";
            return await this.CallApiAsync<List<Marchas>>(request);
        }

        public async Task<List<Gama>> GetGamasAsync()
        {
            string request = "/api/Coches/GetGamas";
            return await this.CallApiAsync<List<Gama>>(request);
        }

        public async Task<List<Combustible>> GetCombustiblesAsync()
        {
            string request = "/api/Coches/GetCombustible";
            return await this.CallApiAsync<List<Combustible>>(request);
        }

        public async Task<bool> InsertCocheAsync(string marca, string modelo, string matricula, IFormFile fichero, int asientos, int idmarchas, int idgama, int kilometraje, int puertas, int idcombustible, int idvendedor, decimal preciokilometros, decimal precioilimitado)
        {
            string request = "/api/Coches/CreateCoche";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string token = contextAccessor.HttpContext.User.FindFirst("TOKEN")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(marca), "Marca");
                    content.Add(new StringContent(modelo), "Modelo");
                    content.Add(new StringContent(matricula), "Matricula");

                    var stream = fichero.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(fichero.ContentType);
                    content.Add(fileContent, "Fichero", fichero.FileName);

                    content.Add(new StringContent(asientos.ToString()), "Asientos");
                    content.Add(new StringContent(idmarchas.ToString()), "IdMarchas");
                    content.Add(new StringContent(idgama.ToString()), "IdGama");
                    content.Add(new StringContent(kilometraje.ToString()), "Kilometraje");
                    content.Add(new StringContent(puertas.ToString()), "Puertas");
                    content.Add(new StringContent(idcombustible.ToString()), "IdCombustible");
                    content.Add(new StringContent(idvendedor.ToString()), "IdVendedor");
                    content.Add(new StringContent(preciokilometros.ToString(CultureInfo.InvariantCulture)), "PrecioKilometros");
                    content.Add(new StringContent(precioilimitado.ToString(CultureInfo.InvariantCulture)), "PrecioIlimitado");

                    HttpResponseMessage response = await client.PostAsync(request, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<dynamic>();
                        return result.success;
                    }
                    return false;
                }
            }
        }

        public async Task UpdateCocheAsync(int idcoche, int idgama, decimal preciokilometros, decimal precioilimitado)
        {
            string request = $"/api/Coches/UpdateCoche/{idcoche}";
            var data = new
            {
                IdGama = idgama,
                PrecioKilometros = preciokilometros.ToString(),
                PrecioIlimitado = precioilimitado.ToString()
            };
            await this.CallApiPutTokenAsync<dynamic>(request, data);
        }

        public async Task DeleteCocheAsync(int idcoche)
        {
            string request = $"/api/Coches/DeleteCoche/{idcoche}";
            await this.CallApiDeleteTokenAsync(request);
        }

        public async Task<List<Reserva>> GetReservasFilterAsync()
        {
            string request = "/api/Vendedores/GetReservasProximas";
            return await this.CallApiGetTokenAsync<List<Reserva>>(request);
        }

        public async Task<List<Reserva>> GetReservasKilometrajeAsync()
        {
            string request = "/api/Vendedores/GetReservasProximasKilometraje";
            return await this.CallApiGetTokenAsync<List<Reserva>>(request);
        }

        public async Task<List<Reserva>> GetReservasIlimitadosAsync()
        {
            string request = "/api/Vendedores/GetReservasProximasIlimitadas";
            return await this.CallApiGetTokenAsync<List<Reserva>>(request);
        }

        public async Task<List<Reserva>> GetReservasPorCocheAsync(int idcoche)
        {
            string request = $"/api/Vendedores/GetReservasPorCoche/{idcoche}";
            return await this.CallApiGetTokenAsync<List<Reserva>>(request);
        }

        public async Task<Reserva> GetReservaAsync(int idreserva)
        {
            string request = $"/api/Reservas/GetReserva/{idreserva}";
            return await this.CallApiGetTokenAsync<Reserva>(request);
        }
        #endregion

        #region Comprador Methods
        public async Task<List<VistaCoche>> FindAllCochesAsync()
        {
            string request = "/api/Compradores/GetCoches";
            return await this.CallApiAsync<List<VistaCoche>>(request);
        }

        public async Task<List<VistaCoche>> FilterByPrecio(int valor)
        {
            string request = $"/api/Compradores/CochesFiltradosPorPrecio/{valor}";
            return await this.CallApiPostAsync<List<VistaCoche>>(request, new { });
        }

        public async Task<VistaCoche> GetVistaCocheAsync(int idcoche)
        {
            string request = $"/api/Compradores/GetCoche/{idcoche}";
            return await this.CallApiAsync<VistaCoche>(request);
        }

        public async Task<Coche> GetCocheAsync(int idcoche)
        {
            string request = $"/api/Compradores/GetCoche/{idcoche}";
            return await this.CallApiAsync<Coche>(request);
        }

        public async Task CompraCocheAsync(int idusuario, int idcoche, DateTime fechainicio, DateTime fechafin, double precio, bool kilometraje)
        {
            string request = $"/api/Compradores/ReservarCoche/{idcoche}";
            var data = new
            {
                fechaInicio = fechainicio,
                fechaFin = fechafin,
                valor = precio,
                kilometraje = kilometraje
            };
            await this.CallApiPostTokenAsync<dynamic>(request, data);
        }

        // TODO: este no hace falta, ya se controla en la api
        //public async Task<bool> ComprobarDisponibilidadCocheAsync(int idcoche, DateTime fechainicio, DateTime fechafin)
        //{
        //    // This is handled by the controller before calling CompraCocheAsync
        //    // Implementing as separate method for possible future use
        //    string request = $"/api/Compradores/ComprobarDisponibilidad/{idcoche}";
        //    var data = new
        //    {
        //        FechaInicio = fechainicio,
        //        FechaFin = fechafin
        //    };
        //    var result = await this.CallApiPostAsync<dynamic>(request, data);
        //    return result?.disponible ?? false;
        //}

        public async Task<List<Compra>> GetComprasUsuarioAsync(int idusuario, string estadoReserva = null)
        {
            string request = "/api/Compradores/GetCompras";
            if (!string.IsNullOrEmpty(estadoReserva))
            {
                request += $"?estadoReserva={estadoReserva}";
            }
            return await this.CallApiGetTokenAsync<List<Compra>>(request);
        }
        #endregion

        #region Auth Methods

        public async Task<string> GetTokenAsync
        (string userName, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                LoginModel model = new LoginModel
                {
                    Email = userName,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content
                        .ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }


        public async Task<Usuario> LoginAsync(string email, string password)
        {
            string token = await this.GetTokenAsync(email, password);

            if (token == null)
            {
                return null;
            }
            string request = "/api/Auth/GetCurrentUser";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.Url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject result = JObject.Parse(data);
                    bool success = result.Value<bool>("success");
                    if (success)
                    {
                        JObject usuarioJson = result["usuario"] as JObject;
                        if (usuarioJson != null)
                        {
                            Usuario usuario = usuarioJson.ToObject<Usuario>();
                            return usuario;
                        }
                    }
                }
                return null;
            }
        }


        public async Task<List<Rol>> GetRolesAsync()
        {
            string request = "/api/Auth/GetRoles";
            return await this.CallApiAsync<List<Rol>>(request);
        }

        public async Task<bool> RegisterUsuarioAsync(string nombre, string email, string password, int idrol, string telefono, string? apellidos, string? dni, DateTime? fechanacimiento, string? direccion, string? passpecial, string? nombreempresa)
        {
            string request = "/api/Auth/Register";
            var data = new
            {
                Nombre = nombre,
                Email = email,
                Password = password,
                IdRol = idrol,
                Telefono = telefono,
                Apellidos = apellidos,
                Dni = dni,
                FechaNacimiento = fechanacimiento,
                Direccion = direccion,
                PassPecial = passpecial,
                NombreEmpresa = nombreempresa
            };
            var result = await this.CallApiPostAsync<dynamic>(request, data);
            return result?.success ?? false;
        }
        #endregion

        #region Additional Methods
        //public async Task<Usuario> GetUsuarioAsync()
        //{
        //    string request = "/api/Auth/GetUsuario";
        //    return await this.CallApiAsync<Usuario>(request);
        //}

        //TODO: UN OJO PARA SACAR LAS COSAS DINAMICAMENTE
        public async Task<Vendedor> GetVendedorAsync(int idusuario)
        {
            string request = $"/api/Auth/GetVendedor/{idusuario}";
            return await this.CallApiAsync<Vendedor>(request);
        }
        public async Task<Vendedor> GetVendedorAsync()
        {
            string request = $"/api/Compradores/GetInformacionVendedor";
            return await this.CallApiAsync<Vendedor>(request);
        }

        //public async Task<int> GetMaxIdUser()
        //{
        //    // This is typically a server-side operation
        //    throw new NotImplementedException("This operation should be handled by the API internally");
        //}

        //public async Task<int> GetMaxIdComprador()
        //{
        //    // This is typically a server-side operation
        //    throw new NotImplementedException("This operation should be handled by the API internally");
        //}

        //public async Task<bool> CheckVendedor()
        //{
        //    string request = "/api/Auth/CheckVendedor";
        //    var result = await this.CallApiAsync<dynamic>(request);
        //    return result?.exists ?? false;
        //}

        //public async Task<List<VistaCoche>> GetAllCochesAsync()
        //{
        //    // This appears to be a duplicate of GetCochesAsync or FindAllCochesAsync
        //    return await this.FindAllCochesAsync();
        //}

        public async Task<List<VistaReserva>> GetVistaReservasAsync()
        {
            string request = "/api/Vendedores/GetReservas";
            return await this.CallApiGetTokenAsync<List<VistaReserva>>(request);
        }
        #endregion
    }
}
