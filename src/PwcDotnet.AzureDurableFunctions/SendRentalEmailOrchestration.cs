using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace PwcDotnet.AzureDurableFunctions;

public record SendRentalEmailDto(int RentalId, string CustomerEmail, DateTime StartDate, DateTime EndDate);
public static class SendRentalEmailOrchestration
{
    [Function(nameof(SendRentalEmailOrchestration))]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(SendRentalEmailOrchestration));
        logger.LogInformation("Starting SendRentalEmailOrchestration...");

        var input = context.GetInput<SendRentalEmailDto>();

        var outputs = new List<string>();

        // Replace name and input with values relevant for your Durable Functions Activity
        outputs.Add(await context.CallActivityAsync<string>(nameof(SendRentalEmailActivity), input));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [Function(nameof(SendRentalEmailActivity))]
    public static Task<string> SendRentalEmailActivity([ActivityTrigger] SendRentalEmailDto input, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("SendRentalEmailActivity");
        logger.LogInformation($"Email sent to {input.CustomerEmail} - rentalId: {input.RentalId}");
        return Task.FromResult($"Output: Email sent to {input.CustomerEmail} - rentalId: {input.RentalId}");
    }

    [Function("SendRentalEmailOrchestration_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SendRentalEmailOrchestration_HttpStart");
        var input = await req.ReadFromJsonAsync<SendRentalEmailDto>();

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(SendRentalEmailOrchestration), input);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}