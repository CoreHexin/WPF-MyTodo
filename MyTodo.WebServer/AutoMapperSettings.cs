﻿using AutoMapper;
using MyTodo.WebServer.DTOs.Account;
using MyTodo.WebServer.Models;

namespace MyTodo.WebServer
{
    public class AutoMapperSettings : Profile
    {
        public AutoMapperSettings()
        {
            CreateMap<User, NewUserDTO>();
            CreateMap<RegisterDTO, NewUserDTO>();
        }
    }
}
