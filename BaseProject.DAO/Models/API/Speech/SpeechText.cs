namespace BaseProject.DAO.Models.API.Speech
{
    public class SpeechText
	{
        public string Text { get; set; } = string.Empty;
        public List<SpeechWord> Words { get; set; } = [];       
    }
}
