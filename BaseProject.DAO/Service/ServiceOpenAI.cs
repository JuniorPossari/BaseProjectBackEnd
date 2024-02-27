using Newtonsoft.Json;
using BaseProject.DAO.Data;
using BaseProject.DAO.IService;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.API.OpenAI;
using BaseProject.Util;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using BaseProject.DAO.Models.API.Speech;
using BaseProject.DAO.Models.Views;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BaseProject.DAO.Service
{
    public class ServiceOpenAI : IServiceOpenAI
    {
		private readonly IConfiguration _config;
		private readonly ILogger<ServiceOpenAI> _logger;
		private readonly string _api_resource_name;
        private readonly string _api_deployment_id;
        private readonly string _api_key;
        private readonly string _api_base_url;
        private readonly string _api_version;

        public ServiceOpenAI(
            IConfiguration config,
			ILogger<ServiceOpenAI> logger
		) 
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _config = config;
            _logger = logger;
            _api_resource_name = _config.GetProperty<string>("AzureOpenAIAPI", "ResourceName");
            _api_deployment_id = _config.GetProperty<string>("AzureOpenAIAPI", "DeploymentId");
            _api_key = _config.GetProperty<string>("AzureOpenAIAPI", "Key");
            _api_version = _config.GetProperty<string>("AzureOpenAIAPI", "Version");
            _api_base_url = $"https://{_api_resource_name}.openai.azure.com/openai/deployments/{_api_deployment_id}";
        }

		private async Task<string> InitChat(List<Message> mensagens, int? idUsuario = null, decimal temperature = 0.2M)
		{
			var resposta = "";

			var chatRequest = new ChatRequest
			{
				messages = [.. mensagens],
				temperature = temperature,
				user = idUsuario.HasValue ? idUsuario.ToString() : "1",
			};

			var log = await this.Chat(chatRequest, idUsuario);

			if (log.Success)
			{
				try
				{
					var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(log.RespContent);

					resposta = chatResponse.choices.FirstOrDefault()?.message?.content ?? "";
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				}
			}

			return resposta;
		}

		private string[] ObterRespostas(string resposta)
		{
			try
			{
				if (string.IsNullOrEmpty(resposta) || resposta.Contains("NADA ENCONTRADO")) return [];

				return resposta.TrimStart('\n').Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(x => Regex.Replace(x, @"^\d{1,}. ", "")).Select(x => Regex.Replace(x, @"^\d{1,}° ", "")).Select(x => Regex.Replace(x, @"^\d{1,} ?- ", "")).Select(x => x.Replace("  ", " ").Replace("- ", "").Replace(";", "").Replace("\"", "").Replace("'", "").Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? Array.Empty<string>();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				return [];
			}
		}

		private async Task<LogOpenAI> Chat(ChatRequest chatRequest, int? idUsuario = null)
        {
            return await this.Request(HttpMethod.Post, $"chat/completions", chatRequest, idUsuario);
        }

        private async Task<LogOpenAI> Request<T>(HttpMethod httpMethod, string endPoint, T content, int? idUsuario = null)
        {
            var log = new LogOpenAI
            {
                ReqMethod = (byte)EnumExtensions.GetValueFromDisplayName<EnumRequestMethod>(httpMethod.ToString()),
                ReqURL = $"{_api_base_url}/{endPoint}?api-version={_api_version}",
                ReqDate = DateTime.Now.ToBrasiliaTime(),
                IdUsuario = idUsuario,
            };

            try
            {
                using var httpClient = new HttpClient();

                httpClient.BaseAddress = new Uri(_api_base_url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request = new HttpRequestMessage(httpMethod, log.ReqURL)
                {
                    Headers =
                    {
                        {"api-key", _api_key}
                    },
                    Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
                };

                log.ReqContent = await request.Content.ReadAsStringAsync();

                var response = await httpClient.SendAsync(request);

                log.RespDate = DateTime.Now.ToBrasiliaTime();
                log.RespContent = await response.Content.ReadAsStringAsync();
                log.RespStatusCode = (int)response.StatusCode;

                if (!response.IsSuccessStatusCode) throw new Exception($"Error {log.RespStatusCode} ({response.StatusCode})!");

                log.Success = true;
            }
            catch (Exception ex)
            {
				_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				log.Exception = ex.InnerException?.Message ?? ex.Message;
                log.Success = false;
            }

            SaveLog(log);

            return log;
        }

        private bool SaveLog(LogOpenAI log)
        {
            try
            {
                using var context = new ApplicationDbContext();

                context.LogOpenAI.Add(log);

                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
				_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				return false;
            }
        }
    }
}