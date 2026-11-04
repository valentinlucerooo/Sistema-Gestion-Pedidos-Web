using BackBLL;
using BackEntity;
using Microsoft.AspNetCore.Mvc;

namespace BackSIstema.Controladores
{
     [Route("api/pedido/[Controller]")]
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
        public IActionResult Post([FromBody] List<PedidoEntity> borrador)
        {
            try
            {
                BLL.ValidarBorrador(borrador);
                return Ok("Pedidos agregados correctamente.");
            }
            catch (BLLException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("individual")]
        public IActionResult CrearPedido([FromBody] PedidoEntity Nuevo)
        {
            try
            { BLL.ValidarPedido(Nuevo);
                return Ok("Pedido agregado correctamente.");
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
    }

    
}
