using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.Devices;



namespace EventGridTrigger
{
    public class EventGridTriggerT5
    {

        private readonly ILogger<EventGridTriggerT5> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IServiceHubContext _serviceHubContext;
        private readonly ServiceClient _serviceClient;

        public EventGridTriggerT5(IServiceManager serviceManager, ServiceClient serviceClient, ILogger<EventGridTriggerT5> logger, IServiceHubContext serviceHubContextTask)
        {
            _serviceManager = serviceManager;
            _serviceClient = serviceClient;
            _logger = logger;
            _serviceHubContext = serviceHubContextTask; // Blocking wait, adjust if necessary
        }

        [Function("ReceiveFromDevice")]
        public async Task ReceiveFromDevice([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            try
            {
                var eventData = eventGridEvent.Data.ToString();
                var eventDataJson = JsonSerializer.Deserialize<JsonElement>(eventData);

                if (eventDataJson.TryGetProperty("body", out var body))
                {
                    var bodyString = body.ToString();
                    if (string.IsNullOrEmpty(bodyString))
                    {
                        _logger.LogInformation("Event body is empty");
                        return;
                    }
                    await _serviceHubContext.Clients.All.SendAsync("ReceiveTelemetry", bodyString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        [Function("SendToDevice")]
        public async Task<HttpResponseData> SendToDevice([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK); // Default response

            try
            {
                string requestBody = await req.ReadAsStringAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorResponse.WriteStringAsync("Request body is empty.");
                    return errorResponse;
                }

                var messageData = JsonSerializer.Deserialize<IotDeviceMessage>(requestBody);

                if (messageData == null)
                {
                    var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorResponse.WriteStringAsync("Failed to deserialize request body to IotDeviceMessage.");
                    return errorResponse;
                }

                var commandMessage = new Microsoft.Azure.Devices.Message(Encoding.UTF8.GetBytes(requestBody))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };

                // Send the message to the IoT Hub
                await _serviceClient.SendAsync(messageData.DeviceId, commandMessage);

                _logger.LogInformation("Message sent successfully to device {DeviceId}", messageData.DeviceId);
                await response.WriteStringAsync("Message sent successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");

                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error sending message to IoT Hub: {ex.Message}");
                return errorResponse;
            }
        }


        [Function("Negotiate")]
        public async Task<HttpResponseData> Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            try
            {
                // Construct the SignalR connection URL
                string hubName = "deviceHub";
                string endpoint = _serviceManager.GetClientEndpoint(hubName);

                // Generate an access token for the client
                string accessToken = _serviceManager.GenerateClientAccessToken(hubName);

                // Create and return the response
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    url = endpoint,
                    accessToken = accessToken
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating SignalR connection info: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                errorResponse.WriteString("Error generating SignalR connection info.");
                return errorResponse;
            }
        }
    }
}
