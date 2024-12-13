using System.Net;
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
        /// <param name="todoItemDTO"></param>
        /// <returns></returns>
        public async Task<ApiResponse?> SaveTodoItemAsync(TodoItemDTO todoItemDTO)
        {
            RestResponse response;
            var request = new RestRequest("todo").AddJsonBody(todoItemDTO);
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
        public async Task<ApiResponse?> GetTodoItemsAsync()
        {
            RestResponse response;
            var request = new RestRequest("todo");
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
        /// 更新待办事项状态
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        public async Task<ApiResponse?> UpdateTodoStatusAsync(TodoItem todoItem)
        {
            RestResponse response;
            var request = new RestRequest("todo").AddJsonBody(
                new { Id = todoItem.Id, Status = todoItem.Status }
            );
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
        /// 解析响应
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private ApiResponse? ParseResponse(RestResponse response)
        {
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
