using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.DTOs.UserDTOs;

namespace UniversityEnrollment.Core.DTOs.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDTOs.UserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserDTOs.UserDTO>();
            CreateMap<CreateUserDTO, ApplicationUser>();
            CreateMap<UpdateUserDTO, ApplicationUser>();
        }
    }
}
