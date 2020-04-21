using AutoMapper;
using TodoApi.Models;

public class DomainProfile : Profile
{
	public DomainProfile()
	{
		CreateMap<TodoItem, TodoViewModel>();
		CreateMap<TodoViewModel, TodoItem>();
		CreateMap<WebApiUser, RegisterViewModel>();
		CreateMap<RegisterViewModel, WebApiUser>();
		CreateMap<WebApiUser, DisplayViewModel>();
		CreateMap<DisplayViewModel, WebApiUser>();
	}
}