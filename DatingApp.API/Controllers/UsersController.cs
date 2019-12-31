using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;

        public UsersController(IDatingRepository datingRepository)
        {
            this.datingRepository = datingRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await datingRepository.GetUsers();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await datingRepository.GetUser(id);

            return Ok(user);
        }
    }
}