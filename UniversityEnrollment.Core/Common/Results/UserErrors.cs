using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.Common.Results
{
    public static class UserErrors
    {
        public static readonly Error UserNotFound =
new("User.UserNotFound",
"The specified user does not exist.");

        public static readonly Error UserAlreadyExists=
            new("User.UserAlreadyExists",
                "A user with the specified user id already exists.");

        public static readonly Error EmailAlreadyExists = 
            new("User.EmailAlreadyExists",
                "A user with the specified email already exists.");
    }
}
