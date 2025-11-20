using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.DTOs.UserDTO
{
    public class LoginResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public AuthorizationResponse AuthResponse { get; set; } = new AuthorizationResponse();
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    }
}
