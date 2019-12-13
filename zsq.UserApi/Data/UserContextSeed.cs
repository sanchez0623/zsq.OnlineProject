using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using zsq.UserApi.Models;

namespace zsq.UserApi.Data
{
    public class UserContextSeed
    {
        private ILogger<UserContextSeed> _logger;

        public UserContextSeed(ILogger<UserContextSeed> logger)
        {
            _logger = logger;
        }

        public static async Task SeedAsync(IApplicationBuilder app, ILoggerFactory loggerFactory, int? retry = 0)
        {
            //重试机制：解决在docker-compose中由于db还没启动完成，而userapi就去访问了db的问题
            var retryCount = retry.Value;
            try
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<UserContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserContextSeed>>();
                    logger.LogDebug("begin seed");

                    context.Database.Migrate();

                    //Todo: 为啥使用context.Users.Any()会报错??
                    //No coercion operator is defined between types 'System.Int16' and 'System.Boolean'.
                    //类型'System.Int16'和'System.Boolean'之间没有定义强制操作符。
                    if (context.Users.Count() <= 0)
                    {
                        context.Users.Add(new AppUser
                        {
                            Name = "sanchez"
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (retryCount < 10)
                {
                    retryCount++;

                    var logger = loggerFactory.CreateLogger(typeof(UserContextSeed));
                    logger.LogError(ex.Message);

                    await SeedAsync(app, loggerFactory, retryCount);
                }
            }
        }
    }
}
