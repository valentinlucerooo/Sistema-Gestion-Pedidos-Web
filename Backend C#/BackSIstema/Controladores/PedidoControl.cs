using BackBLL;
using BackDAL;
using BackEntity;
using Microsoft.AspNetCore.Mvc;

namespace BackSIstema.Controladores
{
     [Route("api/pedido")]
        [ApiController]
        
    public class PedidoControl: ControllerBase
    {
         private readonly PedidoBLL BLL;
        public PedidoControl(PedidoBLL bll)
        {
            BLL = bll;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var listado = BLL.GetAll();
                return Ok(listado);
            }
            catch (BLLException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Lista")]
        public IActionResult Post([FromBody] LotePedidosRequestDTO request)
        {
            try
            {
                // Acá usamos request.Pedidos (en plural) porque este DTO maneja listas
                BLL.ValidarBorrador(request.Pedidos, request.Detalles);
                return Ok("Pedidos agregados correctamente.");
            }
            catch (BLLException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("individual")]
        public IActionResult CrearPedido([FromBody] PedidoConDetallesRequestDTO request)
        {
            try
            {
                // Llamamos a la BLL pasándole los datos que vienen dentro de la "caja" request
                int idGenerado = BLL.ValidarPedido(request.Pedido, request.Detalles);

                return Ok(new { mensaje = "Pedido agregado correctamente.", idPedido = idGenerado });
            }
            catch (BLLException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("usuario/{idUsuario}")]
        public IActionResult ObtenerPedidosPorUsuario(int idUsuario)
        {
            try
            {
                List<PedidoEntity> lista = BLL.ObtenerPedidosPorUsuario(idUsuario);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet("obtenerTodos")]
        public IActionResult ObtenerTodos()
        {
            try
            {
                var lista = BLL.GetAll();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // 2. ENDPOINT PARA ACTUALIZAR EL ESTADO (O ARCHIVAR)
        // Recibe el ID por la URL y el nuevo estado en formato JSON: { "estado": "Entregado" } o { "estado": "Archivado" }
        [HttpPatch("actualizarEstado/{id}")]
        public IActionResult ActualizarEstado(int id, [FromBody] ActualizarEstadoDto datos)
        {
            try
            {
                // Validamos que los datos no lleguen nulos
                if (datos == null || string.IsNullOrWhiteSpace(datos.Estado))
                {
                    return BadRequest(new { mensaje = "El estado es requerido." });
                }

                // Llamamos a la BLL que ya armamos para validar y actualizar en la DAL
                BLL.ValidarCambioEstado(id, datos.Estado);

                return Ok(new { mensaje = "Estado actualizado con éxito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

    // Clase de apoyo para recibir el JSON del frontend limpiamente
    public class ActualizarEstadoDto
    {
        public string Estado { get; set; }
    }
    // Clase de apoyo para recibir el JSON combinado del pedido y sus detalles
    public class PedidoConDetallesRequestDTO
    {
        public PedidoEntity Pedido { get; set; }
        public List<DetallePedidoEntity> Detalles { get; set; }
    }

    // DTO para un lote de pedidos
    public class LotePedidosRequestDTO
    {
        public List<PedidoEntity> Pedidos { get; set; }
        public List<DetallePedidoEntity> Detalles { get; set; }
    }
}

    

