﻿using AutoMapper;
using MyTodo.WebServer.DTOs.Account;
using MyTodo.WebServer.DTOs.Memo;
using MyTodo.WebServer.DTOs.Todo;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer
{
    public class AutoMapperSettings : Profile
    {
        public AutoMapperSettings()
        {
            CreateMap<User, NewUserDTO>();
            CreateMap<RegisterDTO, NewUserDTO>();

            CreateMap<TodoForCreateDTO, Todo>();

            CreateMap<MemoForCreateDTO, Memo>();
        }
    }
}
