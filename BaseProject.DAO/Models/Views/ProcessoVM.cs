using Microsoft.EntityFrameworkCore;
using BaseProject.Util;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.DAO.Models.Views
{
	public class ProcessoVM
	{
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public byte Tipo { get; set; }
        public string TipoString { get; set; }
        public byte Status { get; set; }
        public string StatusString { get; set; }
        public string StatusColor { get; set; }
        public DateTime DataInicial { get; set; }
        public string DataInicialString { get; set; }
        public DateTime? DataFinal { get; set; }
        public string DataFinalString { get; set; }

        public ProcessoVM() { }

        public ProcessoVM(Processo model)
        {
            Id = model.Id;
            IdUsuario = model.IdUsuario;
            Tipo = model.Tipo;
            TipoString = ((EnumProcessoTipo)model.Tipo).GetDisplayName();
            Status = model.Status;
            StatusString = ((EnumProcessoStatus)model.Status).GetDisplayName();
            StatusColor = ((EnumProcessoStatus)model.Status).GetDisplayDescription();
            DataInicial = model.DataInicial;
            DataInicialString = model.DataInicial.ToString("dd/MM/yyyy HH:mm:ss");
            DataFinal = model.DataFinal;
            DataFinalString = model.DataFinal.HasValue ? model.DataFinal.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
        }
    }
}