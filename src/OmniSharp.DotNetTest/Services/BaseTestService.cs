using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OmniSharp.Mef;
using OmniSharp.Models;
using OmniSharp.Services;

namespace OmniSharp.DotNetTest.Services
{
    [OmniSharpHandler(OmnisharpEndpoints.V2.GetDotNetTestStartInfo, LanguageNames.CSharp)]
    public abstract class BaseTestService<TRequest, TResponse> : RequestHandler<TRequest, TResponse>
        where TRequest: Request
    {
        private readonly OmniSharpWorkspace _workspace;
        private readonly DotNetCliService _dotNetCli;
        private readonly ILoggerFactory _loggerFactory;

        protected BaseTestService(OmniSharpWorkspace workspace, DotNetCliService dotNetCli, ILoggerFactory loggerFactory)
        {
            _workspace = workspace;
            _dotNetCli = dotNetCli;
            _loggerFactory = loggerFactory;
        }

        protected abstract TResponse HandleRequest(TRequest request, TestManager testManager);

        public Task<TResponse> Handle(TRequest request)
        {
            var document = _workspace.GetDocument(request.FileName);

            using (var testManager = TestManager.Start(document.Project, _dotNetCli, _loggerFactory))
            {
                var response = HandleRequest(request, testManager);
                return Task.FromResult(response);
            }
        }
    }
}
