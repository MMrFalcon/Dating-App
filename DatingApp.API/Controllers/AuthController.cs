using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await authRepository.Exist(userForRegisterDto.Username))
                return BadRequest("Username Already Exists");

            var userToCreate = new User
            {
              Username = userForRegisterDto.Username  
            };

            var createdUser = await authRepository.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }


    }
}