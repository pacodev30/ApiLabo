using ApiLabo.Data.Models;
using ApiLabo.Dto;

namespace ApiLabo.Services;
public interface IUserService
{
    Task<List<UserOutputModel>> GetAll();
    Task<UserOutputModel?> GetById(int id);
    Task<UserOutputModel> Add(UserInputModel user);
    Task<bool> Update(int id, UserInputModel user);
    Task<bool> Delete(int id);
}
