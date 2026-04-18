using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.BLL.Exceptions;
using System.Reflection;

namespace Combophoto.Api.BLL.Services
{
    public class PromptService : IPromptService
    {
        private const string PromptResourceName = "Combophoto.Api.BLL.Prompts.CombophotoPrompt.txt";
        private const string PromptV2ResourceName = "Combophoto.Api.BLL.Prompts.CombophotoPromptV2.txt";

        private static readonly Lazy<string> PromptLazy = new Lazy<string>(() =>
            LoadPromptFromResources(PromptResourceName));

        private static readonly Lazy<string> PromptV2Lazy = new Lazy<string>(() =>
            LoadPromptFromResources(PromptV2ResourceName));

        private static string Prompt => PromptLazy.Value;

        private static string PromptV2 => PromptV2Lazy.Value;

        public string GetPrompt()
        {            
            return Prompt;                                 
        }

        public string GetPromptV2()
        {
            return PromptV2;
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
