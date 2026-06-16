using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionsController : ControllerBase
    {
        private readonly IErpService _erpService;

        public SectionsController(IErpService erpService)
        {
            _erpService = erpService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections()
        {
            var sections = await _erpService.GetSectionsAsync();
            return Ok(sections);
        }
    }
}
