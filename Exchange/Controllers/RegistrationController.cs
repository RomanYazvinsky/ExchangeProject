using System;
using System.Threading.Tasks;
using DatabaseModel;
using DatabaseModel.Constants;
using DatabaseModel.Entities;
using Exchange.Models;
using Exchange.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exchange.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly ExchangeDbContext _context;

        public RegistrationController(ILogger<RegistrationController> logger, ExchangeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserRegistrationModel registrationModel)
        {
            if (string.IsNullOrWhiteSpace(registrationModel.Username) || string.IsNullOrWhiteSpace(registrationModel.Password))
            {
                return BadRequest();
            }
            var alreadyExist = await _context.Users.AnyAsync(u => u.UserName.Equals(registrationModel.Username));
            if (alreadyExist)
            {
                return BadRequest();
            }
            await _context.Users.AddAsync(new UserEntity
            {
                Guid = Guid.NewGuid(),
                UserName = registrationModel.Username,
                PasswordHash = PasswordUtil.HashPassword(registrationModel.Password),
                Role = Role.Customer,
                Email = registrationModel.Email
            });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
