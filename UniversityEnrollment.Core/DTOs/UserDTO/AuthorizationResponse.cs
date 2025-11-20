using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.DTOs.UserDTO
{
    public class AuthorizationResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
