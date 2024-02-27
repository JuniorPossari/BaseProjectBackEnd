using BaseProject.DAO.Data;
using BaseProject.DAO.IRepository;
using BaseProject.DAO.Models;

namespace BaseProject.DAO.Repository
{
	public class RepositoryProcesso : Repository<Processo>, IRepositoryProcesso
    {
        public RepositoryProcesso(ApplicationDbContext context) : base(context) { }
    }
}
