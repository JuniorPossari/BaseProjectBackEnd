using BaseProject.DAO.Data;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;

namespace BaseProject.DAO.Repository
{
	public class RepositoryLogAcessoUsuario : Repository<LogAcessoUsuario>, IRepositoryLogAcessoUsuario
    {
        public RepositoryLogAcessoUsuario(ApplicationDbContext context) : base(context) { }
    }
}
