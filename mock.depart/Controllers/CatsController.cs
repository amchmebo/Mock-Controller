using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mock.depart.Data;
using mock.depart.Models;
using mock.depart.Services;

namespace mock.depart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatsController : ControllerBase
    {
        private readonly CatsService _service;

        public CatsController(CatsService service)
        {
            _service = service;
        }

        // TODO Pour facilité les tests il vaut mieux utiliser un ActionResult<Type>
        // DELETE: api/Cats/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCat(int id)
        {
            // TODO vous devrez surement en faire une propriété pour pouvoir la "mock"
            string? userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // TODO vous devrez aussi faire un mock avec le service
            Cat? cat = _service.Get(id);
            if (cat == null)
            {
                return NotFound();
            }
            if (cat.CatOwner!.Id != userid)
            {
                return BadRequest("Cat is not yours");
            }
            if (cat.CuteLevel == Cuteness.BarelyOk)
            {
                cat = _service.Delete(id);
                return Ok(cat);
            }
            else
            {
                return BadRequest("Cat is too cute");
            }
        }
    }
}
