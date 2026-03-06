using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM.API.DTOs.Item;
using SCM.API.Models;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _context.Items.ToList();
            return Ok(items);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
                return NotFound("Item not found");

            return Ok(item);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create(ItemCreateDto dto)
        {
            // Validate ABC & XYZ
            if (!new[] { "A", "B", "C" }.Contains(dto.ABCClass))
                return BadRequest("Invalid ABC classification");

            if (!new[] { "X", "Y", "Z" }.Contains(dto.XYZClass))
                return BadRequest("Invalid XYZ classification");

            var item = new Item
            {
                Name = dto.Name,
                SKU = dto.SKU,
                UnitType = dto.UnitType,
                ABCClass = dto.ABCClass,
                XYZClass = dto.XYZClass,
                MinStockLevel = dto.MinStockLevel,
                MaxStockLevel = dto.MaxStockLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            _context.SaveChanges();

            return Ok(item);
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, ItemUpdateDto dto)
        {
            var item = _context.Items.Find(id);
            if (item == null)
                return NotFound("Item not found");

            item.Name = dto.Name;
            item.UnitType = dto.UnitType;
            item.ABCClass = dto.ABCClass;
            item.XYZClass = dto.XYZClass;
            item.MinStockLevel = dto.MinStockLevel;
            item.MaxStockLevel = dto.MaxStockLevel;
            item.IsActive = dto.IsActive;

            _context.SaveChanges();

            return Ok(item);
        }

        // SOFT DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
                return NotFound("Item not found");

            item.IsActive = false;
            _context.SaveChanges();

            return Ok("Item deactivated");
        }
    }
}