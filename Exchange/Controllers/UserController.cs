using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ExchangeDbContext _context;

        public UserController(ExchangeDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IEnumerable<UserInfoModel>> GetUsers()
        {
            return (await _context.Users.ToListAsync()).Select(user => new UserInfoModel(user));
        }
    }
}
