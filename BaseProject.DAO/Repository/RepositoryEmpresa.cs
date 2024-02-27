using BaseProject.DAO.Data;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;

namespace BaseProject.DAO.Repository
{
	public class RepositoryEmpresa : Repository<Empresa>, IRepositoryEmpresa
    {
        public RepositoryEmpresa(ApplicationDbContext context) : base(context) { }
    }
}
