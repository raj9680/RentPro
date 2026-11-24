using Rent.Contracts.DTOs.Auth;
using Rent.UI.Services.Api;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Rent.UI.Services
{
    public class AuthService
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

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        
        public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest login)
        {
            // 1 Create the client
            HttpClient httpClient = _httpClientFactory.CreateClient("RentApi");
            ApiResult<LoginResponse> result = new ApiResult<LoginResponse>();

            // 2. Prepare the data - DTO -> JSON
            string requestJson = JsonSerializer.Serialize(login, _jsonOptions);

            try
            {
                // 3. Prepare Body Data & SEND
                using(StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json"))

                using (HttpResponseMessage response = await httpClient.PostAsync("api/auth/login", content))
                {
                    // 4. Status
                    response.StatusCode = response.StatusCode;

                    // 5. Read the body
                    string responseJson = await response.Content.ReadAsStringAsync();

                    // 6. Json to DTO
                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        try
                        {
                            ApiResponse<LoginResponse>? body = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseJson, _jsonOptions);

                            if(body != null)
                            {
                                result.Data = body.Data;
                                result.Message = body.Message;
                                result.Errors = body.Errors;
                            }
                        }
                        catch (JsonException)
                        {
                            result.Success = false; // not the JSON shape we expected
                        }
                    }
                }
            } catch(HttpRequestException)
            {
                result.StatusCode = HttpStatusCode.ServiceUnavailable;
                result.Success = false;
                result.Message = "The service is not available. Try again later";
                return result;
            }
            catch (TaskCanceledException)
            {
                result.StatusCode = HttpStatusCode.GatewayTimeout;
                result.Success = false;
                result.Message = "The service took too long to respond. Please try again.";
                return result;
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

            return result;
        }


        public async Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            // 1. Create Client
            HttpClient httpClient = _httpClientFactory.CreateClient("RentApi");
            ApiResult<RegisterResponse> result = new ApiResult<RegisterResponse>();

            // 2. Prepare data to JSON
            string requestJson = JsonSerializer.Serialize(request, _jsonOptions);

            try
            {
                // 3. Prepare text type as Body
                using(StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json"))

                // 4. Make Request
                using(HttpResponseMessage response = await httpClient.PostAsync("/api/auth/register", content))
                {
                    response.StatusCode = response.StatusCode;

                    // 5. Read as String
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        try
                        {
                            ApiResponse<RegisterResponse>? body = JsonSerializer.Deserialize<ApiResponse<RegisterResponse>>(responseJson, _jsonOptions);

                            if (body != null)
                            {
                                result.Data = body.Data;
                                result.Errors = body.Errors;
                                result.Message = body.Message;
                            }
                        }
                        catch (JsonException)
                        {
                            result.Success = false; // not the valid JSON expected
                        }
                    }
                }
            } catch (HttpRequestException)
            {
                result.StatusCode = HttpStatusCode.ServiceUnavailable;
                result.Success = false;
                result.Message = "Service is not available. Try again later!";
                return result;

            } catch (TaskCanceledException)
            {
                result.StatusCode = HttpStatusCode.GatewayTimeout;
                result.Success = false;
                result.Message = "Request took too long to respond. Try again later!";
                return result;
            }

            if (result.Success == false && result.Data == null)
                result.Success = false;

            return result;
        }
    }
}
