using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseModel;
using Exchange.Constants;
using Exchange.Models;
using Exchange.Services;
using Exchange.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ErrorMessageService _ems;
        private readonly AuthService _service;

        public UserController(
            ExchangeDbContext context,
            ErrorMessageService ems,
            AuthService service
        )
        {
            _context = context;
            _ems = ems;
            _service = service;
        }

        [HttpGet("currentUser")]
        [Authorize]
        public Task<UserDTO> GetCurrentUserInfo()
        {
            return _service.GetCurrentUserInfo(Request);
        }

        [HttpGet("users")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            return (await _context.Users.ToListAsync()).Select(user => new UserDTO(user));
        }

        [HttpPost("userRole")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task ModifyUserRole(UserDTO userDto)
        {
            if (userDto.Id == default)
            {
                throw _ems.BuildError(ErrorTypes.InvalidParameters);
            }

            var user = await _context.Users.FindAsync(userDto.Id);
            user.Role = userDto.Role;
            await _context.SaveChangesAsync();
        }
    }
}
