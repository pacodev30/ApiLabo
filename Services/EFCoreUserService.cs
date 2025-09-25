using ApiLabo.Data;
using ApiLabo.Data.Models;
using ApiLabo.Dto;
using Microsoft.EntityFrameworkCore;

namespace ApiLabo.Services
{
    public class EFCoreUserService(ApiDbContext context) : IUserService
    {
        private readonly ApiDbContext _context = context;
        private UserOutputModel ToOutputModel(User dbUser)
        {
            return new UserOutputModel(
                dbUser.Id,
                $"Pseudo : {dbUser.Pseudo} | Password : {dbUser.Password}",
                dbUser.Birthday == DateTime.MinValue ? null : dbUser.Birthday);
        }
        public async Task<UserOutputModel> Add(UserInputModel userInput)
        {
            var dbUser = new User
            {
                Pseudo = userInput.Pseudo,
                Password = userInput.Password,
                Birthday = userInput.Birthday.GetValueOrDefault(),
            };
            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();
            return ToOutputModel(dbUser);
        }

        public async Task<bool> Delete(int id)
        {
            return await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync() > 0;
        }

        public async Task<List<UserOutputModel>> GetAll()
        {
            var  users = (await _context.Users.ToListAsync()).ConvertAll(ToOutputModel);
            return users;
        }

        public async Task<UserOutputModel?> GetById(int id)
        {
            var dbUser = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (dbUser is null) return null;
            return ToOutputModel(dbUser);
        }

        public async Task<bool> Update(int id, UserInputModel user)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(usr => usr
                    .SetProperty(us => us.Pseudo, user.Pseudo)
                    .SetProperty(us => us.Password, user.Password)
                    .SetProperty(us => us.Birthday, user.Birthday)) > 0;
        }
    }
}
