using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.DTOs.EnrollmentDTOs;
using UniversityEnrollment.Core.Domain.Entities;

namespace UniversityEnrollment.Core.DTOs.MappingProfiles
{
    public class EnrollmentMappingProfile : Profile
    {
        public EnrollmentMappingProfile()
        {
            CreateMap<EnrollmentDTO, Enrollment>();
            CreateMap<Enrollment, EnrollmentDTO>();

            CreateMap<CreateEnrollmentDTO, Enrollment>();
        }
    }
}
