using MyTodo.Core.Models;
using RestSharp;

namespace MyTodo.Core.Api
{
    public class MyTodoClient : IDisposable
    {
        private readonly RestClient _client;

        public MyTodoClient()
        {
            var options = new RestClientOptions("https://localhost:7246/api");
            _client = new RestClient(options);
        }

        public async Task<ApiResponse?> LoginAsync(LoginModel loginModel)
        {
            var request = new RestRequest("account/login").AddJsonBody(loginModel);
            var response = await _client.PostAsync<ApiResponse>(request);
            return response;
        }

        public async Task<ApiResponse?> RegisterAsync(RegisterModel registerModel)
        {
            var request = new RestRequest("account/register").AddJsonBody(registerModel);
            var response = await _client.PostAsync<ApiResponse>(request);
            return response;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
