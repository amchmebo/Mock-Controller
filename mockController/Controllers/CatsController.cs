using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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

        public virtual string UserId { get { return User.FindFirstValue(ClaimTypes.NameIdentifier)!; } }


        // TODO Pour facilité les tests il vaut mieux utiliser un ActionResult<Type>
        // DELETE: api/Cats/5
        [HttpDelete("{id}")]
        public ActionResult<Cat> DeleteCat(int id)
        {
            // TODO vous devrez surement en faire une propriété pour pouvoir la "mock"
            string? userid = UserId;

            // TODO vous devrez aussi faire un mock avec le service
            Cat? cat = _service.Get(id);
            if (cat == null)
            {
                //test cas chat non trouvé
                return NotFound();
            }
            if (cat.CatOwner!.Id != userid)
            {
                //test cas chat n'appartient pas à la personne qui tente de le supprimer
                return BadRequest("Cat is not yours");
            }
            if (cat.CuteLevel == Cuteness.BarelyOk)
            {
                //test suppression ok
                cat = _service.Delete(id);
                return Ok(cat);
            }
            else
            {
                //test cas chat est trop mignon pour être supprimé
                return BadRequest("Cat is too cute");
            }
        }
    }
}
