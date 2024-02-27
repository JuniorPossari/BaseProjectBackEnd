using Microsoft.AspNetCore.Mvc;
using BaseProject.DAO.IService;
using Microsoft.AspNetCore.Authorization;

namespace BaseProject.API.Areas.API.Controllers
{
	[AllowAnonymous]
    [Area("API")]
    [Route("[area]/[controller]")]
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly IServiceSpeech _serviceSpeech;
        private readonly IServiceOpenAI _serviceOpenAI;

        public TestController
        (
            ILogger<TestController> logger,
            IServiceSpeech serviceSpeech,
            IServiceOpenAI serviceOpenAI
		)
        {
            _logger = logger;
            _serviceSpeech = serviceSpeech;
            _serviceOpenAI = serviceOpenAI;
        }

	}
}