using Rent.Contracts.DTOs.Owner;
using Rent.UI.Services.Api;
using System.Net;
using System.Text.Json;

namespace Rent.UI.Services
{
    public class OwnerService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions _jsonOptions = CreateJsonOptions();

        private static JsonSerializerOptions CreateJsonOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            return options;
        }

        public OwnerService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PropertySummaryResponse> ListPropertiesAsync(int page)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("RentApi");
            ApiResult<PropertySummaryResponse> result = new ApiResult<PropertySummaryResponse>();

            using (HttpResponseMessage response = await httpClient.GetAsync($"/api/owner/property?page={page}"))
            {
                response.StatusCode = response.StatusCode;

                string responseJson = await response.Content.ReadAsStringAsync();

                try
                {
                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        ApiResponse<PropertySummaryResponse>? body = JsonSerializer.Deserialize<ApiResponse<PropertySummaryResponse>>(responseJson, _jsonOptions);

                        if (body != null)
                        {
                            try
                            {
                                result.Data = body.Data;
                                result.Message = body.Message;
                                result.Errors = body.Errors;

                            }
                            catch (JsonException)
                            {
                                result.Success = false;
                            }
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    result.StatusCode = HttpStatusCode.ServiceUnavailable;
                    result.Success = false;
                    result.Message = "The service is not available. Try again later";
                    return result.Data;
                }

                catch (TaskCanceledException)
                {
                    result.StatusCode = HttpStatusCode.GatewayTimeout;
                    result.Success = false;
                    result.Message = "The service took too long to respond. Please try again.";
                    return result.Data;
                }

                // A success must contain null data; a failure must have a message
                if (result.Success && result.Data == null)
                {
                    result.Success = false;
                }
                if (!result.Success && result.Message == null)
                {
                    result.Message = "Login failed. Please try again.";
                }

                return result.Data;
            }
        }


        public async Task<ApiResult<PropertyHierarchyResponse>> GetPropertyHierarchyAsync(int id)
        {
            ApiResult<PropertyHierarchyResponse> result = new ApiResult<PropertyHierarchyResponse>();
            HttpClient httpClient = _httpClientFactory.CreateClient("RentApi");

            try
            {
                // The call that can throw is now INSIDE the try
                using (HttpResponseMessage response = await httpClient.GetAsync("api/owner/GetPropertyHierarchy/" + id))
                {
                    result.StatusCode = response.StatusCode;
                    result.Success = response.IsSuccessStatusCode;

                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        try
                        {
                            // Deserialize is the line that can throw JsonException
                            ApiResponse<PropertyHierarchyResponse>?body = JsonSerializer.Deserialize<ApiResponse<PropertyHierarchyResponse>>(responseJson, _jsonOptions);

                            if (body != null)
                            {
                                result.Data = body.Data;
                                result.Message = body.Message;
                                result.Errors = body.Errors;
                            }
                        }
                        catch (JsonException)
                        {
                            result.Success = false;
                            result.Message = "Unexpected response from the server.";
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                result.StatusCode = HttpStatusCode.ServiceUnavailable;
                result.Success = false;
                result.Message = "Service is not available. Try again later.";
            }
            catch (TaskCanceledException)
            {
                result.StatusCode = HttpStatusCode.GatewayTimeout;
                result.Success = false;
                result.Message = "Service took too long to respond. Try again later.";
            }

            if (!result.Success && result.Message == null)
            {
                result.Message = "Could not load the property. Please try again.";
            }

            return result;
        }
    }
}
