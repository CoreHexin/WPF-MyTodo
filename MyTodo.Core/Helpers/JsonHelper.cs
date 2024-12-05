using System.Text.Json;

namespace MyTodo.Core.Helpers
{
    public static class JsonHelper
    {
        public static JsonSerializerOptions Options =>
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
}
