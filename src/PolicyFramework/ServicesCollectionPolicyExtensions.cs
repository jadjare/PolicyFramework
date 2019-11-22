using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Jadjare.PolicyFramework
{
    public static class ServicesCollectionPolicyExtensions
    {
        public static void AddPolicies(this IServiceCollection service, PolicyContext policies)
        {
            foreach (var namedPolicy in policies.GetPolicySets())
            {
                var policySet = namedPolicy.Value;

                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireAuthenticatedUser();

                var noOneHasAccessToPolicy = policySet == null;
                if (noOneHasAccessToPolicy) {
                    policyBuilder.RequireAssertion((context) => false); //If no one has access to the policy then always make a false assertion;
                }
                else
                {
                    var allowableRulesAndUsers = policySet.GetAllowableRolesAndUsersCombined();
                    if(allowableRulesAndUsers.Any()) policyBuilder.RequireRole(policySet.GetAllowableRolesAndUsersCombined());

                    if (policySet.DenyRoles != null)
                        policyBuilder.RequireAssertion(context => 
                            policySet.GetAllowableRolesAndUsersCombined().Any(allowedRole => context.User.IsInRole(allowedRole)) ||
                            !policySet.DenyRoles.Any(deniedRole => context.User.IsInRole(deniedRole)));

                    if (policySet.DenyUsers != null)
                        policyBuilder.RequireAssertion(context =>
                            policySet.GetAllowableRolesAndUsersCombined().Any(allowedRole => context.User.IsInRole(allowedRole)) ||
                            !policySet.DenyUsers.Any(deniedUser => context.User.IsInRole(deniedUser)));

                    if (policySet.AllowClaim != null)
                        policyBuilder.RequireClaim(policySet.AllowClaim.Type, policySet.AllowClaim.Values);

                    if (policySet.AllowClaims != null)
                        policySet.AllowClaims.ForEach(claim => policyBuilder.RequireClaim(claim.Type, claim.Values));
                }
                
                var policy = policyBuilder.Build();

                service.AddAuthorization(options => options.AddPolicy(namedPolicy.Key, policy));
            }
        }
    }
}
