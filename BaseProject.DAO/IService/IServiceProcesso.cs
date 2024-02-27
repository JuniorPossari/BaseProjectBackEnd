using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.IService
{
	public interface IServiceProcesso : IService<Processo>
    {
        bool Processando(int idEmpresa, byte? tipo = null);
        DTResult<ProcessoVM> Listar(DTParam<ProcessoFM> param, int idEmpresa);
    }
}
