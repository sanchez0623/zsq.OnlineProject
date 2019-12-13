using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using zsq.UserIdentity.Services;

namespace zsq.UserIdentity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        private IAuthCodeService _authCodeService;
        private IUserService _userService;

        public SmsAuthCodeValidator(IAuthCodeService authCodeService, IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;
        }

        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];
            var errorResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(code))
            {
                context.Result = errorResult;
                return;
            }

            if (!_authCodeService.Validate(phone, code))
            {
                context.Result = errorResult;
                return;
            }

            var userInfo = await _userService.CheckOrCreateAsync(phone);
            if (userInfo == null || userInfo.Id <= 0)
            {
                context.Result = errorResult;
                return;
            }

            var claims = new Claim[]
            {
                new Claim("name", userInfo.Name ?? string.Empty),
                new Claim("company", userInfo.Company ?? string.Empty),
                new Claim("title", userInfo.Title ?? string.Empty),
                new Claim("avatar", userInfo.Avatar ?? string.Empty),
            };

            context.Result = new GrantValidationResult(userInfo.Id.ToString(), GrantType, claims);
        }
    }
}
