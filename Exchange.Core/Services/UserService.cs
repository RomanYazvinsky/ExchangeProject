using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Core.Models.Dto;
using Exchange.Core.ViewModels;
using Exchange.Data;
using Exchange.Data.Constants;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Core.Services
{
    public class UserService
    {
        private readonly ExchangeDbContext _context;

        public UserService(ExchangeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserVm>> GetAllUsersAsync()
        {
            return (await _context.Users.ToListAsync()).Select(user => new UserVm(user));
        }

        public async Task<bool> ModifyUserRoleAsync(Guid userId, Role role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.Role = role;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
