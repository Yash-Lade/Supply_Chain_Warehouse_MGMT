using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM.API.DTOs.Warehouse;
using SCM.API.Models;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarehousesController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var warehouses = _context.Warehouses.ToList();
            return Ok(warehouses);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null)
                return NotFound("Warehouse not found");

            return Ok(warehouse);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create(WarehouseCreateDto dto)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Location = dto.Location,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();

            return Ok(warehouse);
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, WarehouseUpdateDto dto)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null)
                return NotFound("Warehouse not found");

            warehouse.Name = dto.Name;
            warehouse.Location = dto.Location;
            warehouse.IsActive = dto.IsActive;

            _context.SaveChanges();

            return Ok(warehouse);
        }

        // SOFT DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null)
                return NotFound("Warehouse not found");

            warehouse.IsActive = false;
            _context.SaveChanges();

            return Ok("Warehouse deactivated");
        }
    }
}