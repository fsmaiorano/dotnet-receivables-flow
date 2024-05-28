// using System.Reflection;
// using Application.Common.Exceptions;
// using Application.Common.Security.Jwt;
// using Domain.Enums;
// using Domain.IdentityEntities;
// using MediatR;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
//
// namespace Application.Common.Behaviours;
//
// public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : notnull
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly RoleManager<ApplicationRole> _roleManager;
//     private readonly SignInManager<ApplicationUser> _signInManager;
//     private readonly SigningConfigurations _signingConfigurations;
//     private readonly TokenConfigurations _tokenConfigurations;
//
//     public AuthorizationBehaviour(
//         UserManager<ApplicationUser> userManager,
//         RoleManager<ApplicationRole> roleManager,
//         SignInManager<ApplicationUser> signInManager,
//         SigningConfigurations signingConfigurations,
//         TokenConfigurations tokenConfigurations)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//         _signInManager = signInManager;
//         _signingConfigurations = signingConfigurations;
//         _tokenConfigurations = tokenConfigurations;
//     }
//
//
//     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
//         CancellationToken cancellationToken)
//     {
//         var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
//
//         if (authorizeAttributes.Any())
//         {
//             // Must be authenticated user
//             if (_user.Id == null)
//             {
//                 throw new UnauthorizedAccessException();
//             }
//
//             // Role-based authorization
//             var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
//
//             if (authorizeAttributesWithRoles.Any())
//             {
//                 var authorized = false;
//
//                 var roles = Enum.GetValues<RoleEnum>().Select(role => role.ToString()).ToArray();
//
//                 foreach (var role in roles)
//                 {
//                     var appRole = new ApplicationRole
//                     {
//                         Name = role
//                     };
//
//                     if (_roleManager.RoleExistsAsync(role).Result) continue;
//
//                     var result = _roleManager.CreateAsync(appRole).Result;
//
//                     authorized = result.Succeeded;
//                 }
//
//                 // Must be a member of at least one role in roles
//                 if (!authorized)
//                 {
//                     throw new ForbiddenAccessException();
//                 }
//             }
//
//             // Policy-based authorization
//             var authorizeAttributesWithPolicies =
//                 authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
//             if (authorizeAttributesWithPolicies.Any())
//             {
//                 foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
//                 {
//                     var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);
//
//                     if (!authorized)
//                     {
//                         throw new ForbiddenAccessException();
//                     }
//                 }
//             }
//         }
//
//         // User is authorized / authorization not required
//         return await next();
//     }
// }
//
// }