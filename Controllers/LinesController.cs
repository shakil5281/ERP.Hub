using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinesController : ControllerBase
    {
        private readonly IErpService _erpService;

        public LinesController(IErpService erpService)
        {
            _erpService = erpService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Line>>> GetLines()
        {
            var lines = await _erpService.GetLinesAsync();
            return Ok(lines);
        }
    }
}
