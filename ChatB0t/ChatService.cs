using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace ChatB0t;

public delegate void NotifyMessageReceived(string message);

internal class ChatService
{
    private readonly NotifyMessageReceived? notifyMessageReceived;
    Kernel kernel = null!;
    ChatHistory chatHistory = null!;
    IChatCompletionService aiChatService = null!;

    public ChatService(NotifyMessageReceived? notifyMessageReceived)
    {
        // Create a kernel with OpenAI chat completion
#pragma warning disable SKEXP0010
        kernel = Kernel.CreateBuilder()
                           .AddOpenAIChatCompletion(
                               modelId: "llama3",
                               endpoint: new Uri("http://localhost:11434"),
                               apiKey: "")
                           .Build();

        aiChatService = kernel.GetRequiredService<IChatCompletionService>();
        chatHistory = new ChatHistory();
        this.notifyMessageReceived = notifyMessageReceived;
    }

    public void ClearChatHistory()
    {
        chatHistory.Clear();
    }

    public async Task<string> GetChatResponse(string message)
    {
        try
        {
            chatHistory.Add(new ChatMessageContent(AuthorRole.User, message));

            string response = string.Empty;

            await foreach (var item in aiChatService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                notifyMessageReceived?.Invoke(item.Content ?? "");

                response += item.Content;
            }

            chatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, response));

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }


}