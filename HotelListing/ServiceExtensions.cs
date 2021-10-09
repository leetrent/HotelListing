using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            IdentityBuilder identityBuilder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), services);
            identityBuilder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }
    }
}
