using BaseProject.Util;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using BaseProject.DAO.IService;
using Xabe.FFmpeg;
using BaseProject.DAO.Models.Views;
using BaseProject.DAO.Models.API.Speech;

namespace BaseProject.DAO.Service
{
	public class ServiceSpeech : IServiceSpeech
    {
        private readonly IConfiguration _config;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<ServiceSpeech> _logger;
        private readonly string _azure_speech_location;
        private readonly string _azure_speech_key;

        public ServiceSpeech(
            IConfiguration config,
			IWebHostEnvironment webHostEnvironment,
			ILogger<ServiceSpeech> logger
        ) 
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
			_logger = logger;
            _azure_speech_location = _config.GetProperty<string>("AzureSpeechSDK", "Location");
            _azure_speech_key = _config.GetProperty<string>("AzureSpeechSDK", "Key");
        }

		public async Task<SpeechText> ConvertSpeechToText(ArquivoVM inputFile)
		{
			var speechText = new SpeechText();
			var outputFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Temp", $"{Guid.NewGuid()}_output.wav");
			var inputFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Temp", $"{Guid.NewGuid()}_input.{inputFile.Extensao}");

			try
			{
				FFmpeg.SetExecutablesPath(Path.Combine(_webHostEnvironment.WebRootPath, "FFmpeg"));

				File.WriteAllBytes(inputFilePath, Convert.FromBase64String(inputFile.Base64));

				var mediaInfo = await FFmpeg.GetMediaInfo(inputFilePath);

				var audioStream = mediaInfo.AudioStreams.FirstOrDefault();

				var conversionResult = await FFmpeg.Conversions.New()
					.AddStream(audioStream)
					.SetOutput(outputFilePath)
					.Start();

				var outputFileInfo = new FileInfo(outputFilePath);

				if (outputFileInfo.Exists)
				{
					var speechConfig = SpeechConfig.FromSubscription(_azure_speech_key, _azure_speech_location);

					speechConfig.SpeechRecognitionLanguage = "pt-BR";
					speechConfig.RequestWordLevelTimestamps();

					using var audioConfig = AudioConfig.FromWavFileInput(outputFileInfo.FullName);
					using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

					while (true)
					{
						var result = await speechRecognizer.RecognizeOnceAsync();

						if (result.Reason == ResultReason.RecognizedSpeech)
						{
							if (result.Text.Length > 0)
							{
								var detailedResults = result.Best();

								if (detailedResults != null && detailedResults.Any())
								{
									var bestResult = detailedResults?.ToList()[0];

									foreach (var word in bestResult.Words)
									{
										speechText.Words.Add(new SpeechWord
										{
											Word = word.Word,
											Offset = word.Offset,
											Duration = word.Duration,
										});
									}
								}

								speechText.Text += $"{(string.IsNullOrEmpty(speechText.Text) ? string.Empty : " ")}{result.Text}";
							}
						}
						else break;
					}

					speechRecognizer.Dispose();
				}

				if (string.IsNullOrEmpty(speechText.Text) || speechText.Words.Count == 0) speechText = null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				speechText = null;
			}

			if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
			if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

			return speechText;
		}

    }
}