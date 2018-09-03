using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlayCat.DataModels;
using PlayCat.DataServices.Response;
using PlayCat.DataServices.Test.Extensions;
using PlayCat.Web;
using Xunit;
using Xunit.Abstractions;

namespace PlayCat.DataServices.Test
{
    public class BaseTest
    {
        protected const string BaseAudioExtension = ".mp3";
        protected readonly ITestOutputHelper _output;

        protected readonly TestServer _server;

        public BaseTest(ITestOutputHelper output)
        {
            _output = output;
            _server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<StartupTest>());
        }

        public void CheckIfSuccess(BaseResult result)
        {
            _output.WriteLine(JsonConvert.SerializeObject(result));
            
            Assert.NotNull(result);
            Assert.True(result.Ok);
            Assert.Null(result.Errors);
        }

        public void CheckIfFail(BaseResult result)
        {
            _output.WriteLine(JsonConvert.SerializeObject(result));
            
            Assert.NotNull(result);
            Assert.False(result.Ok);
            Assert.NotNull(result.Info);
            Assert.True(result.Info.Length > 0);
        }


        protected Guid GetUserId(PlayCatDbContext context)
        {
            string password = "123456abc";
            string email = "test@gmail.com";

            User user = context.CreateUser(email, "test", "test", "m", password);
            AuthToken authToken = context.CreateToken(DateTime.Now.AddDays(-1), false, user.Id);

            context.SaveChanges();

            return user.Id;
        }

        protected void SqlLiteDatabaseTest(Action<DbContextOptions<PlayCatDbContext>> action)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<PlayCatDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new PlayCatDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                action(options);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
