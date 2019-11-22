using System.Collections.Generic;
using System.Linq;

namespace Jadjare.PolicyFramework
{
    public class PolicySet
    {
        public PolicySet()
        {
            AllowUsers = new List<string>();
            AllowRoles = new List<string>();
        }

        public List<string> AllowRoles { get; set; }
        public List<string> AllowUsers { get; set; }

        public List<string> DenyRoles { get; set; }
        public List<string> DenyUsers { get; set; }

        public Claim AllowClaim { get; set; }

        public List<Claim> AllowClaims { get; set; }
        public IEnumerable<string> GetAllowableRolesAndUsersCombined()
        {
            return AllowRoles.Concat(AllowUsers);
        }

        public class Claim
        {
            public string Type { get; set; }
            public List<string> Values { get; set; }
        }
    }
}
