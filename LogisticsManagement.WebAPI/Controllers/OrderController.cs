using LogisticsManagement.Service.DTOs;
using LogisticsManagement.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IManagerService _managerService;
        public OrderController(IManagerService managerService)
        {
            _managerService = managerService;
        }


        [HttpGet]
        [Route("Orders", Name = "GetAllOrders")]
        [ProducesResponseType(200, Type = typeof(List<OrderDTO>))]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<OrderDTO> orders = await _managerService.GetOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}
