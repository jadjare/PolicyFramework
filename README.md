# Introduction
The PolicyFramework library is designed to make it easier to maintain and apply policies within an application.  It allows for defining policies and who has access to them in an appsettings.json file and then wiring them up to a PolicyContext class which makes it easy to then hook up and refer to those policies throughout the application.


# Getting Started
TODO: Add to Nuget package feed

# How to Use

Note the following steps do not necessarily need to be followed in the order presented.

## Step 1
In an appsettings.json file add the json markup that describes the required policies and the roles / users who are allowed or denied access to the policy.

```json
"Policies": {
    "CanCreateProfile": {
      "AllowRoles": [ "Everyone" ]
    },
    "CanDeleteProfile": {
      "AllowUsers": [ "joseph_adjare", "dave_loper", "anita_coder" ],
      "DenyRoles": [ "Everyone" ]
    },
    "AllowPasswordChange": {
      "AllowClaim": { "Type": "jadjare.co.uk/policy", "Values": ["AllowPasswordChange"] }
    },
    "AllowUserAdministration": {
      "AllowClaim": {
        "Type": "groups",
        "Values": ["5a4e54b8-fc65-4a6c-825c-882ff6a22251","272fce48-44cb-439b-84f8-a3f986ad8640"]
      },
    "CanAuthorisePayments": {
      "AllowClaims": [
        { "Type": "jadjare.co.uk/policy", "Values": ["SeniorPaymentsManager"] },
        { "Type": "groups", "Values": ["5a4e54b8-fc65-4a6c-825c-882ff6a22251","272fce48-44cb-439b-84f8-a3f986ad8640"] },
      ]
    }
  }
```
The json above shows five different examples of setting up a policy.

The "CanCreateProfile" and "CanDeleteProfile" examples illustrate the mapping of a policy to a role.
When defining the `AllowRoles` and `DenyRoles` attributes you are defining a list of user or role names that a user must have any one of but does not need to be posess all.

The "AllowPasswordChange", "AllowUserAdministration" and "CanAuthorisePayments" examples illustrate how to map a policy to a claim.
When defining the allowable claims via the `AllowClaim` attribute you must specify one or more claim `Values` that the user must posess.  When a list is provided a user only needs to posess any on of the claim values to be granted the policy.
You can also stack claims using the "AllowClaims" attribute, in this instance when multiple claims are listed the user must posess at list one claim value from each specified claim.  Thus in the example below the user must have the "SeniorPaymentsManager" claim AND also be in one of the two specified values for the "groups" claim type.

> Note
> When the PolicyFramework is assessing whether a user has a policy or not it does this by invoking one or two methods on the current user's `ClaimPrincpal`.\
> Where "AllowRoles, AllowUsers, DenyRole or DenyUsers" has been specified the `IsInRole()` method is called on the `ClaimsPrincipal`.  The `IsInRole()` method is invoked regardless of whether the PolicyFramework is checking for a role or a specific username.\
> Where "AllowClaims" has been specified then the `RequireClaim()` method is invoked on the `AuthoricationPolicyBuilder` class, which will ultimately check the current user has the claim granted.
> 

## Step 2
Create a C# class that represents the policies.  This is configured in a similar way to Entity Framework to help make the process more familiar.

The Policy Framework library includes a `PolicyContext` class that should be inhereted by our C# class, e.g.
`public class Policies: PolicyContext`

Inside the C# class that you've created you then add a number of `PolicySets` that correspond to the policies defined in the `appsettings.json` file.
Thus we end up with a class like the example below.

```csharp
using Jadjare.PolicyFramework;

namespace Jadjare.MyApplication.Client.AppSettings
{
    public class Policies: PolicyContext
    {
        public PolicySet CanCreateProfile { get; set; }
        public PolicySet CanDeleteProfile { get; set; }
        public PolicySet AllowPasswordChange { get; set; }
    }
}
```

## Step 3
With the policy configuration defined in the appsettings.json file and the C# Policies class implemented we can now add code to the `Startup` class to tie the two together.

The Policy Framework library adds an extension method to the `IServiceCollection` class that allows us to easily attach our policies to the application and ensure that the matching roles, users and claims are mapped to each policy.  We do this by adding a line of code to the `ConfigureServices` method in the `Startup` class.

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            //configure any other services here...

            services.AddPolicies(Configuration.GetSection("Policies").Get<Policies>());
            
            //... and here!
        }
```

When the application starts up everything will be automatically wired up and ready to use.  All we now need do is implement our policies.

> Note
> Additional services may need to be configured to ensure that Windows Authentication or JWT tokens, for example, are correctly implemented.

## Step 4
As we now have a concrete class defining our policies, we can use that to define who is authorised to take certain actions and avoid hard coding policy names throughout the code base.

Here is a simple example.
```csharp
    [Authorize(nameof(Policies.ApplicationUser))]
    public class UserProfileController : Controller
    {
        private readonly ClientSettings _clientSettings;

        [HttpPost]
        [Authorize(nameof(Policies.CanCreateProfile))]
        public async Task<IActionResult> CreateProfile(UserProfile profile)
        {
            //...
        }

        [HttpDelete]
        [Authorize(nameof(Policies.CanDeleteProfile))]
        public async Task<IActionResult> DeleteProfile(Guid profileId)
        {
            //...
        }

        [HttpPost]
        [Authorize(nameof(Policies.AllowPasswordChange))]
        public async Task<IActionResult> ChangePassword(string newPassword, string oldPassword)
        {
            //...
        }
    }
```

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)