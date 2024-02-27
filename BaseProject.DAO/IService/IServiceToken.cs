using BaseProject.DAO.Models;

namespace BaseProject.DAO.IService
{
	public interface IServiceToken
    {
        string GenerateToken(AspNetUser user, List<string> currentRoles);
    }
}
