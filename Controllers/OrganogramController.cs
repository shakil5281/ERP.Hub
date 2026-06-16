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
    public class OrganogramController : ControllerBase
    {
        private readonly IErpService _erpService;

        public OrganogramController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        // GET: api/organogram
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyNodeDto>>> GetOrganogramTree()
        {
            var tree = await _erpService.GetOrganogramTreeAsync();
            return Ok(tree);
        }
    }
}
