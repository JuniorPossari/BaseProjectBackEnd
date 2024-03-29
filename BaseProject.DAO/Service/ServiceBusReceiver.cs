﻿using Azure.Messaging.ServiceBus;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BaseProject.DAO.Data;
using BaseProject.DAO.Models;
using BaseProject.DAO.Models.Others;
using BaseProject.Util;
using System.Text;
using BaseProject.DAO.Models.Views;

namespace BaseProject.DAO.Service
{

    public class ServiceBusReceiver : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ServiceBusReceiver> _logger;
        private readonly string _connection_string;
        private readonly ServiceBusClient _client;
        
        public ServiceBusReceiver(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            IWebHostEnvironment env,
            ILogger<ServiceBusReceiver> logger
        ) 
        {
            _scopeFactory = scopeFactory;            
            _config = config;
            _env = env;
            _logger = logger;
            _connection_string = _config.GetProperty<string>("AzureServiceBus", "ConnectionString");
            _client = new ServiceBusClient(_connection_string);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ReceiveMessagesAsync();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _client.DisposeAsync();
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                foreach(EnumProcessoTipo queue in Enum.GetValues(typeof(EnumProcessoTipo)))
                {
                    var queue_enum = (byte)queue;
                    var queue_name = $"{queue.GetDisplayPrompt().ToLower()}_{_env.EnvironmentName.ToLower()}";

                    try
                    {
                        var receiver = _client.CreateReceiver(queue_name, new ServiceBusReceiverOptions { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

                        var receivedMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));

                        if (receivedMessage != null)
                        {
                            var body = receivedMessage.Body.ToString();

                            switch (queue_enum)
                            {
                                case (byte)EnumProcessoTipo.CarregarUsuarios:
                                    {
                                        var message = JsonConvert.DeserializeObject<ServiceBusMessageProcesso<CarregarUsuariosVM>>(body);

                                        await CarregarUsuarios(message);

                                        break;
                                    }                                
                            }

                        }

                        await receiver.CloseAsync();
                    }
					catch (Exception ex)
					{
                        _logger.LogError(ex, $"Erro ao receber mensagens do Azure Service Bus! Queue Enum: {queue_enum} - Queue Name: {queue_name}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro no foreach (EnumProcessoTipo) ao receber mensagens do Azure Service Bus!");
            }
        }

        private async Task CarregarUsuarios(ServiceBusMessageProcesso<CarregarUsuariosVM> message)
        {
            var sucesso = false;

            try
            {
                using var scope = _scopeFactory.CreateScope();

                using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();

                var processo = context.Processo.AsNoTracking().First(x => x.Id == message.IdProcesso);

                var idEmpresa = processo.IdEmpresa;

                var empresa = context.Empresa.AsNoTracking().First(x => x.Id == idEmpresa);

                var model = message.Payload;

                var arquivo = model.Arquivo;

                using var ms = new MemoryStream(Convert.FromBase64String(arquivo.Base64));

                using var workbook = new XLWorkbook(ms);

                var worksheet = workbook.Worksheet("USUÁRIOS");

                var ultimaLinha = worksheet.LastRowUsed().RowNumber();

                var ultimaColuna = worksheet.LastColumnUsed().ColumnNumber();

                var coluna = 0;
                var colunaNome = 1;
                var colunaSobrenome = 2;
                var colunaEmail = 3;
                var colunaCPF = 4;

                var usuarios = new List<AspNetUser>();

                for (int linha = 2; linha <= ultimaLinha; linha++)
                {
                    try
                    {
                        var nome = worksheet.Cell(linha, coluna = colunaNome).GetString().RemoveSpace();

                        if (string.IsNullOrEmpty(nome) || nome.Length > 50) throw new Exception("O nome do usuário está vazio ou inválido!");

                        var sobrenome = worksheet.Cell(linha, coluna = colunaSobrenome).GetString().RemoveSpace();

                        if (string.IsNullOrEmpty(sobrenome) || sobrenome.Length > 50) throw new Exception("O sobrenome do usuário está vazio ou inválido!");

                        var email = worksheet.Cell(linha, coluna = colunaEmail).GetString().RemoveSpace();

                        if (string.IsNullOrEmpty(email) || !email.IsValidEmail()) throw new Exception("O email do usuário está vazio ou inválido!");

                        var emailDominio = email.GetEmailDominio() ?? "";

                        if (empresa.Dominio.ToLower() != emailDominio.ToLower()) throw new Exception($"O domínio do email do usuário ({emailDominio}) não pertence a empresa!");

                        var existeEmail = context.Usuario.Any(x => x.Email.ToLower() == email.ToLower());

                        if (existeEmail) throw new Exception($"Esse email ({email}) já está vinculado a uma conta!");

                        var cpf = worksheet.Cell(linha, coluna = colunaCPF).GetString().RemoveSpace();

                        if (string.IsNullOrEmpty(cpf)) throw new Exception("O CPF do usuário está vazio ou inválido!");

                        cpf = cpf.Unmask();

                        if (string.IsNullOrEmpty(cpf)) throw new Exception("O CPF do usuário está vazio ou inválido!");

                        cpf = cpf.PadZeroLeft();

                        if (cpf.Length != 11) throw new Exception("O CPF do usuário não possui 11 dígitos!");

                        var existeCPF = context.Usuario.Any(x => !string.IsNullOrEmpty(x.CPF) && x.CPF == cpf);

                        if (existeCPF) throw new Exception($"Esse CPF ({cpf}) já está vinculado a uma conta!");

                        var senha = TextExtensions.RandomPassword(14);

                        var user = new AspNetUser
                        {
                            UserName = email.ToLower(),
                            Email = email.ToLower(),
                            TwoFactorEnabled = true,
                            Usuario = new Usuario
                            {
                                IdEmpresa = idEmpresa,
                                Nome = nome.Trim(),
                                Sobrenome = sobrenome.Trim(),
                                Email = email.ToLower(),
                                CPF = cpf,
                                Senha = Convert.ToBase64String(Encoding.ASCII.GetBytes(senha)),
                                DataCadastro = DateTime.Now.ToBrasiliaTime(),
                                Ativo = true
                            }
                        };

                        if (usuarios.Any(x => (x.Usuario.Email.ToLower() == email.ToLower()) || (x.Usuario.CPF.ToLower() == cpf.ToLower()))) throw new Exception($"O email ou CPF do usuário está duplicado! Nome: {nome} - Sobrenome: {sobrenome} - Email: {email} - CPF: {cpf}");

                        var chkUser = await userManager.CreateAsync(user, senha);

                        if (!chkUser.Succeeded) throw new Exception($"Erro ao adicionar o usuário! Nome: {nome} - Sobrenome: {sobrenome} - Email: {email} - CPF: {cpf}");

                        usuarios.Add(user);
                    }
                    catch (Exception ex)
                    {
						_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
						LogProcesso(message.IdProcesso, $"Linha: {linha} - Coluna: {coluna} - Erro: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }

                sucesso = true;
            }
            catch (Exception ex)
            {
				_logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
				LogProcesso(message.IdProcesso, ex.InnerException?.Message ?? ex.Message);
            }

            FinalizarProcesso(message.IdProcesso, sucesso);
        }

        private void LogProcesso(int idProcesso, string message)
        {
            try
            {
				using var scope = _scopeFactory.CreateScope();

				using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				var processo = context.Processo.Include(x => x.ProcessoArquivo).First(x => x.Id == idProcesso);

                if (processo.ProcessoArquivo == null)
                {
                    using var ms = new MemoryStream();
                    using var sw = new StreamWriter(ms);

                    sw.WriteLine(message);

                    sw.Flush();

                    byte[] bytes = ms.ToArray();

                    ms.Close();

                    processo.ProcessoArquivo = new ProcessoArquivo
                    {
                        Nome = $"LOG_PROCESSO_{((EnumProcessoTipo)processo.Tipo).GetDisplayName().ToUpper()}_{processo.Id}_{processo.DataInicial:yyyy-MM-dd_HH-mm-ss}.txt",
                        Extensao = "txt",
                        Tamanho = bytes.Length,
                        Tipo = "text/plain",
                        Base64 = Convert.ToBase64String(bytes)
                    };
                }
                else
                {
                    using var msReader = new MemoryStream(Convert.FromBase64String(processo.ProcessoArquivo.Base64));
                    using var sr = new StreamReader(msReader);
                    using var msWriter = new MemoryStream();
                    using var sw = new StreamWriter(msWriter);

                    sw.Write(sr.ReadToEnd());
                    sw.WriteLine(message);

                    sw.Flush();

                    byte[] bytes = msWriter.ToArray();

                    sr.Close();
                    sw.Close();
                    msReader.Close();
                    msWriter.Close();

                    processo.ProcessoArquivo.Nome = $"LOG_PROCESSO_{((EnumProcessoTipo)processo.Tipo).GetDisplayName().ToUpper()}_{processo.Id}_{processo.DataInicial:yyyy-MM-dd_HH-mm-ss}.txt";
                    processo.ProcessoArquivo.Extensao = "txt";
                    processo.ProcessoArquivo.Tamanho = bytes.Length;
                    processo.ProcessoArquivo.Tipo = "text/plain";
                    processo.ProcessoArquivo.Base64 = Convert.ToBase64String(bytes);
                }

                context.Processo.Update(processo);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao adicionar/editar log do processo! IdProcesso: {idProcesso} - Message: {message}");
            }
        }

        private void FinalizarProcesso(int idProcesso, bool sucesso)
        {
            try
            {
				using var scope = _scopeFactory.CreateScope();

				using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				var processo = context.Processo.Include(x => x.ProcessoArquivo).First(x => x.Id == idProcesso);

                processo.DataFinal = DateTime.Now.ToBrasiliaTime();
                processo.Status = (byte)EnumProcessoStatus.Completo;

                if (processo.ProcessoArquivo != null)
                {
                    if (sucesso) processo.Status = (byte)EnumProcessoStatus.Incompleto;
                    else processo.Status = (byte)EnumProcessoStatus.Erro;
                }

                context.Processo.Update(processo);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao finalizar o processo! IdProcesso: {idProcesso} - Sucesso: {sucesso}");
            }
        }
    }
}