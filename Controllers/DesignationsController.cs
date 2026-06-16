using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignationsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public DesignationsController(IErpService erpService)
        {
            _erpService = erpService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Designation>>> GetDesignations()
        {
            var designations = await _erpService.GetDesignationsAsync();
            return Ok(designations);
        }
    }
}
