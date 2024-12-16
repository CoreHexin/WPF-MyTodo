using System.Text.Json;
using MyTodo.Core.DTOs;
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

            return ParseResponse(response);
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

            return ParseResponse(response);
        }

        /// <summary>
        /// 请求待办事项统计接口
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse?> GetTodoStatisticAsync()
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

            return ParseResponse(response);
        }

        /// <summary>
        /// 保存待办事项数据
        /// </summary>
        /// <param name="todoForCreateDTO"></param>
        /// <returns></returns>
        public async Task<ApiResponse?> CreateTodoAsync(TodoForCreateDTO todoForCreateDTO)
        {
            RestResponse response;
            var request = new RestRequest("todo").AddJsonBody(todoForCreateDTO);
            try
            {
                response = await _client.PostAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            return ParseResponse(response);
        }

        /// <summary>
        /// 获取所有待办事项数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse?> GetTodosAsync(TodoQueryObject? queryObject = null)
        {
            RestResponse response;
            RestRequest request;
            if (queryObject == null)
            {
                request = new RestRequest($"todo");
            }
            else
            {
                request = new RestRequest(
                    $"todo?title={queryObject.Title}&status={queryObject.Status}"
                );
            }
            try
            {
                response = await _client.GetAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            return ParseResponse(response);
        }

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="todoForUpdateDTO"></param>
        /// <returns></returns>
        public async Task<ApiResponse?> UpdateTodoAsync(int id, TodoForUpdateDTO todoForUpdateDTO)
        {
            RestResponse response;
            var request = new RestRequest($"todo/{id}").AddJsonBody(todoForUpdateDTO);
            try
            {
                response = await _client.PutAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }

            return ParseResponse(response);
        }

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse?> DeleteTodoAsync(int id)
        {
            RestResponse response;
            var request = new RestRequest($"todo/{id}");
            try
            {
                response = await _client.DeleteAsync(request);
            }
            catch (Exception ex)
            {
                return new ApiResponse() { IsSuccess = false, Message = ex.Message };
            }
            return ParseResponse(response);
        }

        /// <summary>
        /// 解析响应
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private ApiResponse? ParseResponse(RestResponse response)
        {
            if ((int)response.StatusCode >= 400)
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
