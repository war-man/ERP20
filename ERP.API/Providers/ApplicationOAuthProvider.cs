﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ERP.Data.DbContext;
using System.Text;
using ERP.Service.Services.IServices;
using ERP.Common.Constants;
using ERP.Data.Identity;
using System.Web;
using ERP.Data.ModelsERP;
using ERP.Repository.Repositories;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ERP.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly ERPDbContext _dbContext = new ERPDbContext();
        private static string _clientId = "DOTNET";
        private static string _clientSecret = "EEF47D9A-DBA9-4D02-B7B0-04F4279A6D20";
        private static string Username = "";
        private static string Password = "";
        //Store the base address of the web api
        //You need to change the PORT number where your WEB API service is running
        private static string baseAddress = "http://localhost:44334/";
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ClientMaster client = context.OwinContext.Get<ClientMaster>("ta:client");
            var user = _dbContext.staffs.FirstOrDefault(t => t.sta_username.Contains(context.UserName) && t.sta_password.Contains(context.Password));
            if (user == null)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
            var role="";
            if (user.group_role_id == 1)
            {
                role = Roles.ADMIN;
            }
            else role = Roles.USER;
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.sta_fullname));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.sta_email));
            identity.AddClaim(new Claim("Id", user.sta_id.ToString()));
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    }
                });
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            // The TryGetBasicCredentials method checks the Authorization header and
            // Return the ClientId and clientSecret
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved through the Authorization header.");
                return Task.FromResult<object>(null);
            }
            //Check the existence of by calling the ValidateClient method
            ClientMaster client = (new ClientMasterRepository()).ValidateClient(clientId, clientSecret);
            if (client == null)
            {
                // Client could not be validated.
                context.SetError("invalid_client", "Client credentials are invalid.");
                return Task.FromResult<object>(null);
            }
            else
            {
                if (!client.Active)
                {
                    context.SetError("invalid_client", "Client is inactive.");
                    return Task.FromResult<object>(null);
                }
                // Client has been verified.
                context.OwinContext.Set<ClientMaster>("ta:client", client);
                context.OwinContext.Set<string>("ta:clientAllowedOrigin", client.AllowedOrigin);
                context.OwinContext.Set<string>("ta:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
                context.Validated();
                return Task.FromResult<object>(null);
            }
        }
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["client_id"];
            var currentClient = context.ClientId;
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }
            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));
            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
            return Task.FromResult<object>(null);
        }

    }
}