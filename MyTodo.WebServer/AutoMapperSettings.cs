using AutoMapper;
using MyTodo.WebServer.DTOs;
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
