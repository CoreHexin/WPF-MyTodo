﻿namespace MyTodo.WebServer
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}