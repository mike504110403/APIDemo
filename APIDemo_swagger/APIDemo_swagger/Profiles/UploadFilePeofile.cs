using AutoMapper;
using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;

namespace APIDemo_swagger.Profiles
{
    public class UploadFilePeofile : Profile
    {
        public UploadFilePeofile() // Automapper 設定檔
        {
            CreateMap<UploadFile, UploadFileDto>();
            CreateMap<UploadFilePostDto, UploadFile>();
        }
    }
}
