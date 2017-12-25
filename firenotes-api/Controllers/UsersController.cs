using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace firenotes_api.Controllers
{
    public class UsersController : Controller
    {
        [Route("{id}"), HttpDelete]
        public async Task<IActionResult> ArchiveUser()
        {
            return Ok();
        }
    }
}