using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace zsq.UserIdentity
{
    public class IdentityConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("gateway_api","gateway api"),
                new ApiResource("contact_api","contact api"),
                new ApiResource("user_api","user api"),
                new ApiResource("project_api","project api"),
                new ApiResource("recommend_api","recommend api")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //GrantType.ClientCredentials模式只能使用form-data
                new Client
                {
                    ClientId = "zsq1",
                    ClientName = "sanchez1",
                    AllowedGrantTypes = { "sms_auth_code" },//自定义模式
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedScopes = {
                        "gateway_api",
                        "contact_api",
                        "user_api",
                        "project_api",
                        "recommend_api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "Mvc",
                    ClientUri = "http://localhost:5001",
                    LogoUri = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1526955882826&di=d53ce0d491bf61a180194fd358641f81&imgtype=0&src=http%3A%2F%2Fimg.mukewang.com%2F5a77b61000013ca502560192.jpg",
                    AllowedGrantTypes = { GrantType.Hybrid },
                    ClientSecrets = { new Secret("secret".Sha256())},
                    //RequireConsent=false,
                    //设为true则会展示‘需要用户授权页面’
                    RequireConsent = true,
                    RedirectUris = {"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5001/signout-callback-oidc"},
                    //AllowedScopes={"api1"}
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
