using BaseProject.DAO.Models.API.Speech;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.IService
{
    public interface IServiceSpeech
    {
        Task<SpeechText> ConvertSpeechToText(ArquivoVM inputFile);
	}
}