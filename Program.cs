using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();

var openAiDeployment = Environment.GetEnvironmentVariable("OPENAI_DEPOYMENT");
var openAiUri = Environment.GetEnvironmentVariable("OPENAI_URI");
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

builder.Services.AddAzureOpenAIChatCompletion(
  deploymentName: openAiDeployment, 
  endpoint: openAiUri, 
  apiKey: openAiApiKey);

builder.Plugins.AddFromType<UniswapV3SubgraphPlugin>();

var kernel = builder.Build();

ChatHistory history = [];

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    Console.Write(">> ");
    history.AddUserMessage(Console.ReadLine()!);

    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    Console.WriteLine("<< " + result);

    history.AddAssistantMessage(result.Content);
}