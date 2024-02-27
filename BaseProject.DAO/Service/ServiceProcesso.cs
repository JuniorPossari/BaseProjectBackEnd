using BaseProject.DAO.IRepository;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Filters;
using BaseProject.DAO.Models.Others;
using BaseProject.DAO.Models.Views;
using BaseProject.DAO.Repository;
using BaseProject.Util;
using System.Linq.Expressions;

namespace BaseProject.DAO.Service
{
	public class ServiceProcesso : IServiceProcesso
    {
        private readonly IRepositoryProcesso _repositoryProcesso;

        public ServiceProcesso(IRepositoryProcesso repositoryProcesso)
        {
            _repositoryProcesso = repositoryProcesso;
        }

        public Processo ObterPorId(int id, string includeProperties = "")
        {
            return _repositoryProcesso.FirstOrDefault(x => x.Id == id, includeProperties);
        }

        public Processo[] ObterTodos(string includeProperties = "", bool noTracking = true)
        {
            return _repositoryProcesso.Get(includeProperties: includeProperties, noTracking: noTracking);
        }

        public bool Processando(int idEmpresa, byte? tipo = null)
        {
            if (tipo.HasValue) return _repositoryProcesso.Exists(x => x.IdEmpresa == idEmpresa && x.Tipo == tipo.Value && x.Status == (byte)EnumProcessoStatus.Processando);

            return _repositoryProcesso.Exists(x => x.IdEmpresa == idEmpresa && x.Status == (byte)EnumProcessoStatus.Processando);
        }

        public bool Adicionar(Processo entity)
        {
            return _repositoryProcesso.Insert(entity);
        }

        public bool Editar(Processo entity)
        {
            return _repositoryProcesso.Update(entity);
        }

        public bool Deletar(int id)
        {
            return _repositoryProcesso.Delete(id);
        }

        public DTResult<ProcessoVM> Listar(DTParam<ProcessoFM> param, int idEmpresa)
        {
            var query = _repositoryProcesso.GetContext().Set<Processo>().AsQueryable();

            query = query.Where(x => x.IdEmpresa == idEmpresa);

            //Adicione as colunas de texto onde a pesquisa geral será aplicada
            //var search = param.SearchValue();
            //if (!string.IsNullOrEmpty(search)) query = query.Where(x =>
            //    x.Nome.Contains(search)
            //);

            //Adicione as colunas ordenáveis e o seu respectivo nome do datatables (É necessário pelo menos uma como padrão)
            var keyGen = new KeySelectorGenerator<Processo>(param.SortedColumnName());
            keyGen.AddKeySelector(x => x.Tipo, "TipoString");
            keyGen.AddKeySelector(x => x.Status, "StatusString");
            keyGen.AddKeySelector(x => x.DataInicial, "DataInicialString");
            keyGen.AddKeySelector(x => x.DataFinal.Value, "DataFinalString");
            query = keyGen.Sort(query, param.IsAscendingSort());

            //Adicione os filtros avançados
            var filters = param.Filters;
            if (filters is not null)
            {
                if (filters.Tipo.HasValue) query = query.Where(x => x.Tipo == filters.Tipo.Value);
                if (filters.Status.HasValue) query = query.Where(x => x.Status == filters.Status.Value);
                if (filters.DataInicial.HasValue) query = query.Where(x => x.DataInicial == filters.DataInicial.Value);
                if (filters.DataFinal.HasValue) query = query.Where(x => x.DataFinal.HasValue && x.DataFinal.Value == filters.DataFinal.Value);
            }

            //Adicione as tabelas relacionadas caso precise
            var includeProperties = "";

            var itens = _repositoryProcesso.Filter(query, param.InitialPosition(), param.ItensPerPage(), out int total, includeProperties);

            return new DTResult<ProcessoVM>
            {
                Itens = itens.Select(x => new ProcessoVM(x)).ToArray(),
                Total = total
            };
        }

    }
}
