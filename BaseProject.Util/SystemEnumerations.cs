using System.ComponentModel.DataAnnotations;

namespace BaseProject.Util
{
	public enum EnumRequestMethod
    {
        [Display(Name = "GET")]
        GET = 1,
        [Display(Name = "POST")]
        POST = 2,
        [Display(Name = "PUT")]
        PUT = 3,
        [Display(Name = "DELETE")]
        DELETE = 4,
    }

    public enum EnumProcessoTipo
    {
        [Display(Name = "Carregar Usuários", Prompt = "carregarusuarios")]
		CarregarUsuarios = 1,
    }

    public enum EnumProcessoStatus
    {
        [Display(Name = "Processando", Description = "info")]
        Processando = 1,
        [Display(Name = "Incompleto", Description = "warning")]
        Incompleto = 2,
        [Display(Name = "Completo", Description = "success")]
        Completo = 3,
        [Display(Name = "Erro", Description = "danger")]
        Erro = 4,
    }

	public enum EnumAnaliseTopico
	{
		//Tópicos de análise

		[Display(Name = "Oportunidades de respiro", GroupName = "Análise", Description = "Isso envolve analisar se o falante está fazendo pausas adequadas para respirar, o que pode afetar a fluência e o ritmo da fala.")]
		OportunidadesDeRespiro = 1,
		[Display(Name = "Terminologias de referência", GroupName = "Análise", Description = "Avaliação do uso de termos específicos ou jargões que podem ser relevantes para o assunto sendo discutido e se eles são utilizados de maneira apropriada.")]
		TerminologiasDeReferencia = 2,
		[Display(Name = "Pluralidade de termos", GroupName = "Análise", Description = "Verifica se há uma diversidade no vocabulário utilizado, evitando repetições desnecessárias que podem tornar a comunicação monótona.")]
		PluralidadeDeTermos = 3,
		[Display(Name = "Recursos de internalização", GroupName = "Análise", Description = "Refere-se à capacidade do falante de transmitir suas mensagens de maneira que o ouvinte possa facilmente absorver e compreender a informação.")]
		RecursosDeInternalizacao = 4,
		[Display(Name = "Performance de impacto", GroupName = "Análise", Description = "Analisa como o falante usa a sua comunicação para influenciar ou impactar o ouvinte.")]
		PerformanceDeImpacto = 5,
		[Display(Name = "Valoração da linguagem", GroupName = "Análise", Description = "Avalia o uso de palavras que expressam valores, o que pode ser importante para persuadir ou transmitir emoções.")]
		ValoracaoDaLinguagem = 6,
		[Display(Name = "Coerência das emoções", GroupName = "Análise", Description = "Verifica se as emoções expressas verbalmente estão alinhadas com o conteúdo da mensagem.")]
		CoerenciaDasEmocoes = 7,
		[Display(Name = "Temporalidade na comunicação", GroupName = "Análise", Description = "Avalia se o falante está usando tempos verbais de forma correta e se a organização temporal da fala está clara para o ouvinte.")]
		TemporalidadeNaComunicação = 8,
		[Display(Name = "Tom de voz", GroupName = "Análise", Description = "Analisa a qualidade do tom de voz e como ele é usado para expressar diferentes nuances na comunicação.")]
		TomDeVoz = 9,
		[Display(Name = "Volume da voz", GroupName = "Análise", Description = "Avalia se o volume da voz está apropriado para a situação e se está sendo usado de forma eficaz para manter a atenção do ouvinte.")]
		VolumeDaVoz = 10,
		[Display(Name = "Dicção", GroupName = "Análise", Description = "Refere-se à clareza com que as palavras são pronunciadas.")]
		Diccao = 11,

		//Tópicos de resultado

		[Display(Name = "Pontos de validação", GroupName = "Resultado", Description = "Aspectos da comunicação que estão funcionando bem e devem ser mantidos.")]
		PontosDeValidacao = 101,
		[Display(Name = "Pontos de melhoria", GroupName = "Resultado", Description = "Áreas onde o falante pode melhorar para tornar a comunicação mais eficaz.")]
		PontosDeMelhoria = 102,
		[Display(Name = "Pontos de experimentação", GroupName = "Resultado", Description = "Sugestões de novas técnicas ou estilos de comunicação que o falante pode tentar incorporar.")]
		PontosDeExperimentacao = 103,
		[Display(Name = "Plano de ação", GroupName = "Resultado", Description = "Um plano de ação para ajudar o indivíduo a trabalhar nos pontos de melhoria e experimentação, com o objetivo de aprimorar suas habilidades de comunicação. Este plano pode incluir práticas, exercícios específicos, treinamento de fala e outras atividades direcionadas.")]
		PlanoDeAcao = 104,
		[Display(Name = "Discurso sugerido", GroupName = "Resultado", Description = "O discurso correto do indivíduo aplicando os pontos de melhoria e experimentação.")]
		DiscursoSugerido = 105,
	}
}
