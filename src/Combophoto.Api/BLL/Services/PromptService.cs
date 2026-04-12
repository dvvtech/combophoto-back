using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.BLL.Exceptions;
using System.Reflection;

namespace Combophoto.Api.BLL.Services
{
    public class PromptService : IPromptService
    {
        private const string PromptResourceName = "Combophoto.Api.BLL.Prompts.CombophotoPrompt.txt";

        private static readonly Lazy<string> PromptLazy = new Lazy<string>(() =>
            LoadPromptFromResources(PromptResourceName));

        private static string Prompt => PromptLazy.Value;

        public string GetPrompt()
        {            
            return Prompt;                                 
        }

        private static string LoadPromptFromResources(string promptResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(promptResourceName);
            if (stream == null)
                throw new ResourceNotFoundException(promptResourceName);

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
