using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityEnrollment.Core.Domain.Entities;
using UniversityEnrollment.Core.DTOs.UserDTO;

namespace UniversityEnrollment.Core.ServiceContracts
{
    public interface IJwtService
    {
        public AuthorizationResponse GenerateToken(ApplicationUser user);
    }
}
