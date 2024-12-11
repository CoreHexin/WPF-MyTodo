using System.Net;
using System.Text.Json;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using RestSharp;

namespace MyTodo.Core.Api
{
    public class ApiClient : IDisposable
    {
        private readonly RestClient _client;

        public ApiClient()
        {
            var options = new RestClientOptions("https://localhost:7246/api");
            _client = new RestClient(options);
        }

        public async Task<ApiResponse?> LoginAsync(LoginModel loginModel)
        {
            RestResponse response;
            var request = new RestRequest("account/login").AddJsonBody(loginModel);
            try
            {
                response = await _client.PostAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = $"服务器响应异常: {(int)response.StatusCode}",
                };
            }
            return JsonSerializer.Deserialize<ApiResponse>(response.Content, JsonHelper.Options);
        }

        public async Task<ApiResponse?> RegisterAsync(RegisterModel registerModel)
        {
            RestResponse response;
            var request = new RestRequest("account/register").AddJsonBody(registerModel);
            try
            {
                response = await _client.PostAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = $"服务器响应异常: {(int)response.StatusCode}",
                };
            }

            if (response.Content == null)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = $"服务器响应异常: 响应内容为空",
                };
            }

            return JsonSerializer.Deserialize<ApiResponse>(response.Content, JsonHelper.Options);
        }

        /// <summary>
        /// 请求待办事项统计接口
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse?> GetTodoStatistic()
        {
            RestResponse response;
            var request = new RestRequest("todo/statistic");
            try
            {
                response = await _client.GetAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = $"服务器响应异常: {(int)response.StatusCode}",
                };
            }

            if (response.Content == null)
            {
                return new ApiResponse()
                {
                    IsSuccess = false,
                    Message = $"服务器响应异常: 响应内容为空",
                };
            }

            return JsonSerializer.Deserialize<ApiResponse>(response.Content, JsonHelper.Options);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
