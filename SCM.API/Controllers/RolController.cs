using Microsoft.AspNetCore.Mvc;
using SCM_System.Services;

namespace SCM_System.Controllers
{
    [ApiController]
    [Route("api/rol")]
    public class RolController : ControllerBase
    {
        private readonly RolService _rolService;

        public RolController(RolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet("{itemId}/{vendorId}")]
        public async Task<IActionResult> GetRol(int itemId, int vendorId)
        {
            var rol = await _rolService.CalculateRolAsync(itemId, vendorId);

            return Ok(new
            {
                ItemId = itemId,
                Rol = rol
            });
        }

        // temproary for test
        [HttpPost("run")]
        public async Task<IActionResult> RunRol()
        {
            await _rolService.CheckAndGeneratePoAsync();
            return Ok("ROL executed");
        }
    }
}