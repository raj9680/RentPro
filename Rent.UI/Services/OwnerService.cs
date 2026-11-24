using Microsoft.AspNetCore.Http.HttpResults;
using Rent.Contracts.DTOs.Auth;
using Rent.Contracts.DTOs.Owner;
using Rent.UI.Services.Api;
using System.Net;
using System.Text;
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
    }
}
