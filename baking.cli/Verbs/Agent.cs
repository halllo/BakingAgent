using System.Text;
using System.Text.Json;
using CommandLine;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace baking.cli.Verbs
{
    /// <summary>
    /// Taken from https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/AGUIClientServer/AGUIClient/Program.cs
    /// </summary>
    [Verb("agent", HelpText = "Invoke the agent.")]
    class Agent
    {
        [Value(0, MetaName = "Mission", HelpText = "The mission or task for the agent to perform.", Required = false)]
        public string? Mission { get; set; } = null!;

        public async Task Do(ILogger<Agent> logger)
        {
            var cancellationToken = CancellationToken.None;
            var serverUrl = "http://localhost:5057/agui";
            logger.LogInformation("Connecting to AG-UI server at: {ServerUrl}", serverUrl);

            // Create the AG-UI client agent
            using HttpClient httpClient = new()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            var changeBackground = AIFunctionFactory.Create(
                () =>
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Changing color to blue");
                },
                name: "change_background_color",
                description: "Change the console background color to dark blue."
            );

            IChatClient chatClient = new AGUIChatClient(httpClient, serverUrl);
            AIAgent agent = chatClient.CreateAIAgent(
                name: "agui-client",
                description: "AG-UI Client Agent",
                tools: [changeBackground]);

            AgentThread thread = agent.GetNewThread();
            List<ChatMessage> messages = [];
            try
            {
                while (true)
                {
                    string? message;
                    if (!string.IsNullOrWhiteSpace(Mission))
                    {
                        // We use the mission for the first run.
                        message = Mission;
                        Mission = null;
                    }
                    else
                    {
                        // Get user message interactively.
                        Console.Write("\nUser: ");
                        message = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            Console.WriteLine("Request cannot be empty.");
                            continue;
                        }
                        else if (message is ":q" or "quit")
                        {
                            break;
                        }
                    }

                    messages.Add(new(ChatRole.User, message));

                    // Call RunStreamingAsync to get streaming updates
                    bool isFirstUpdate = true;
                    string? threadId = null;
                    var updates = new List<ChatResponseUpdate>();
                    StringBuilder lastWrittenText = new();
                    await foreach (AgentRunResponseUpdate update in agent.RunStreamingAsync(messages, thread, cancellationToken: cancellationToken))
                    {
                        // Use AsChatResponseUpdate to access ChatResponseUpdate properties
                        ChatResponseUpdate chatUpdate = update.AsChatResponseUpdate();
                        updates.Add(chatUpdate);
                        if (chatUpdate.ConversationId != null)
                        {
                            threadId = chatUpdate.ConversationId;
                        }

                        // Display run started information from the first update
                        if (isFirstUpdate && threadId != null && update.ResponseId != null)
                        {
                            logger.LogDebug("Run {runId} started - Thread: {threadId}", update.ResponseId, threadId);
                            isFirstUpdate = false;
                        }

                        // Display different content types with appropriate formatting
                        foreach (AIContent content in update.Contents)
                        {
                            switch (content)
                            {
                                case TextContent textContent:
                                    var previousColor = Console.ForegroundColor;
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.Write(textContent.Text);
                                    lastWrittenText.Append(textContent.Text);
                                    Console.ForegroundColor = previousColor;
                                    break;

                                case FunctionCallContent functionCallContent:
                                    var lastWrittenTextString = lastWrittenText.ToString();
                                    if (!string.IsNullOrWhiteSpace(lastWrittenTextString) && !lastWrittenTextString.EndsWith("\n")) Console.WriteLine();
                                    lastWrittenText.Clear();
                                    logger.LogDebug("Function Call {functionName}({arguments})",
                                        functionCallContent.Name,
                                        JsonSerializer.Serialize(functionCallContent.Arguments));
                                    break;

                                case FunctionResultContent functionResultContent:
                                    if (functionResultContent.Exception != null)
                                    {
                                        logger.LogError("Function Exception {exception}, Result: {result}",
                                            functionResultContent.Exception,
                                            functionResultContent.Result);
                                    }
                                    else if (logger.IsEnabled(LogLevel.Debug))
                                    {
                                        logger.LogDebug("Function Result {result}",
                                            functionResultContent.Result);
                                    }
                                    break;

                                case ErrorContent errorContent:
                                    string code = errorContent.AdditionalProperties?["Code"] as string ?? "Unknown";
                                    logger.LogError("Error - Code: {code}, Message: {message}", code, errorContent.Message);
                                    break;
                            }
                        }
                    }
                    if (updates.Count > 0 && !updates[^1].Contents.Any(c => c is TextContent))
                    {
                        var lastUpdate = updates[^1];
                        Console.WriteLine();
                        logger.LogDebug("Run {runId} ended - Thread: {threadId}", lastUpdate.ResponseId, threadId);
                        await Task.Delay(500); // Small delay to ensure logs are flushed
                    }
                    else
                    {
                        Console.WriteLine();
                    }

                    // Add assistant response to messages list
                    var chatResponse = updates.ToChatResponse();
                    messages.AddMessages(chatResponse);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("AGUIClient operation was canceled.");
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException and not ThreadAbortException and not AccessViolationException)
            {
                logger.LogError(ex, "An error occurred while running the AGUIClient");
                return;
            }
        }
    }
}