using BaseProject.DAO.Data;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;

namespace BaseProject.DAO.Repository
{
	public class RepositoryUsuario : Repository<Usuario>, IRepositoryUsuario
    {
        public RepositoryUsuario(ApplicationDbContext context) : base(context) { }
    }
}
