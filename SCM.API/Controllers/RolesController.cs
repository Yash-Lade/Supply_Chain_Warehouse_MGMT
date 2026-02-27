using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            return Ok(_context.Roles.ToList());
        }

        [HttpPost]
        public IActionResult CreateRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return Ok(role);
        }
    }
}