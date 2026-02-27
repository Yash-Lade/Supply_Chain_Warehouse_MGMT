using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.API.Data;
using SCM.API.DTOs.Vendor;
using SCM.API.Models;

namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var vendors = _context.Vendors.ToList();
            return Ok(vendors);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var vendor = _context.Vendors.Find(id);
            if (vendor == null)
                return NotFound("Vendor not found");

            return Ok(vendor);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create(VendorCreateDto dto)
        {
            var vendor = new Vendor
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                LeadTimeDays = dto.LeadTimeDays,
                IsPreferred = dto.IsPreferred,
                IsActive = true,
                PerformanceScore = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Vendors.Add(vendor);
            _context.SaveChanges();

            return Ok(vendor);
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, VendorUpdateDto dto)
        {
            var vendor = _context.Vendors.Find(id);
            if (vendor == null)
                return NotFound("Vendor not found");

            vendor.Name = dto.Name;
            vendor.ContactPerson = dto.ContactPerson;
            vendor.Email = dto.Email;
            vendor.Phone = dto.Phone;
            vendor.LeadTimeDays = dto.LeadTimeDays;
            vendor.IsPreferred = dto.IsPreferred;
            vendor.IsActive = dto.IsActive;

            _context.SaveChanges();

            return Ok(vendor);
        }

        // SOFT DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var vendor = _context.Vendors.Find(id);
            if (vendor == null)
                return NotFound("Vendor not found");

            vendor.IsActive = false;
            _context.SaveChanges();

            return Ok("Vendor deactivated");
        }
    }
}