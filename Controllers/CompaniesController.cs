using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IErpService _erpService;

        public CompaniesController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            var companies = await _erpService.GetCompaniesAsync();
            return Ok(companies);
        }

        // GET: api/companies/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _erpService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound($"Company with ID {id} not found.");
            }
            return Ok(company);
        }

        // POST: api/companies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany([FromBody] Company company)
        {
            if (company == null)
            {
                return BadRequest("Invalid company data.");
            }

            await _erpService.AddCompanyAsync(company);
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        // PUT: api/companies/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCompany(int id, [FromBody] Company company)
        {
            if (company == null || id != company.Id)
            {
                return BadRequest("Company ID mismatch.");
            }

            var existing = await _erpService.GetCompanyByIdAsync(id);
            if (existing == null)
            {
                return NotFound($"Company with ID {id} not found.");
            }

            await _erpService.UpdateCompanyAsync(company);
            return NoContent();
        }

        // DELETE: api/companies/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var existing = await _erpService.GetCompanyByIdAsync(id);
            if (existing == null)
            {
                return NotFound($"Company with ID {id} not found.");
            }

            await _erpService.DeleteCompanyAsync(id);
            return Ok($"Company with ID {id} has been deleted.");
        }

        // POST: api/companies/seed-demo
        [HttpPost("seed-demo")]
        public async Task<IActionResult> SeedDemoCompanies()
        {
            await _erpService.SeedDemoCompaniesAsync();
            return Ok("Demo companies seeded successfully.");
        }

        // POST: api/companies/remove-demo
        [HttpPost("remove-demo")]
        public async Task<IActionResult> RemoveDemoCompanies()
        {
            await _erpService.RemoveDemoCompaniesAsync();
            return Ok("Demo companies removed successfully.");
        }
    }
}
