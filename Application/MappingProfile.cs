using Application.Command.Car;
using Application.DTO.CarDTO;
using AutoMapper;

using Carproject.DTO;
using Carproject.Model;
using Domain.Model;

namespace Carproject
{

    public class MappingProfile : Profile
    {
        //public MappingProfile()
        //{
        //    // نگاشت برای CreateCarDto و معکوس آن به Car
        //    CreateMap<CommandCar, Car>().ReverseMap();

        //    // نگاشت برای CarDto و معکوس آن به Car
        //    CreateMap<CarDto, Car>().ReverseMap().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        public MappingProfile()
        {
            // نگاشت برای CommandCar به Car
            CreateMap<CommandCar, Car>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // از آنجا که `Id` خودکار توسط EF پر می‌شود
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // وضعیت خودرو
                  //.ForMember(dest => dest.Files, opt => opt.MapFrom(src => src.Files.Select(file => new FileBase
                    //{
             //    FileName =file.FileName,
                   // FilePath = file.FilePath
                   //}).ToList())) // نگاشتفایل‌ها
                .ForMember(dest => dest.Files, opt => opt.MapFrom(src => src.Files.Select(file =>
                 new FileBase(file.FileName, file.FilePath)).ToList()))


                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId)); // نگاشت دسته‌بندی

            // نگاشت برای CarDto به Car
            CreateMap<CarDto, Car>()
                .ReverseMap() // برای معکوس کردن نگاشت
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)) // نگاشت نام دسته‌بندی
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


        }



    }
}
