using ApiLabo.Dto;

namespace ApiLabo.Services;
public interface IUserService
{
    Task<List<UserOutputModel>> GetAll();
    Task<UserOutputModel?> GetById(string id);
    Task<UserOutputModel> Add(UserInputModel user);
    Task<bool> Update(string id, UserInputModel user);
    Task<bool> Delete(string id);
}
