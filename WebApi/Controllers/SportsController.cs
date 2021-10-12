using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSportTypes()
        {
            var sportTypes = await _sportService.GetSportTypes();
            return Ok(sportTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string sportName)
        {
            if (await _sportService.AddSport(sportName))
                if(await _sportService.SaveChangesAsync())
                    return NoContent();
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (_sportService.DeleteSport(id))
                if (await _sportService.SaveChangesAsync())
                    return NoContent();
            return NotFound();
        }
    }
}
