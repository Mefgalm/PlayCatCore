using System;
using System.Linq;
using PlayCat.DataService;
using PlayCat.DataServices.Test;
using PlayCat.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace PlayCat.Tests.ProfileTests
{
    public class Profile : BaseTest
    {
        [Fact]
        public void ShouldFailGetProfileOnWrongId()
        {
            SqlLiteDatabaseTest(options =>
            {
                var playListService = _server.Host.Services.GetService(typeof(IProfileService)) as IProfileService;

                using (var context = new PlayCatDbContext(options))
                {
                    playListService.SetDbContext(context);

                    var user = context.CreateUser("test@mail.com", "v", "last", "m", "123");
                    context.SaveChanges();

                    var result = playListService.GetProfile(Guid.Empty);

                    CheckIfFail(result);
                    Assert.True(result.Info.Any());
                }
            });
        }

        [Fact]
        public void ShouldSuccessReturnProfile()
        {
            SqlLiteDatabaseTest(options =>
            {
                var playListService = _server.Host.Services.GetService(typeof(IProfileService)) as IProfileService;

                using (var context = new PlayCatDbContext(options))
                {
                    playListService.SetDbContext(context);

                    string email = "test@mail.com";
                    string firstName = "v";
                    string lastName = "last";
                    string nickName = "mef";
                    string password = "12345678qwe";
                    var user = context.CreateUser(email, firstName, lastName, nickName, password);
                    context.SaveChanges();

                    var result = playListService.GetProfile(user.Id);

                    CheckIfSuccess(result);
                    Assert.NotNull(result.User);
                    Assert.Equal(email, result.User.Email);
                    Assert.Equal(firstName, result.User.FirstName);
                    Assert.Equal(lastName, result.User.LastName);
                    Assert.Equal(nickName, result.User.NickName);
                }
            });
        }

        [Fact]
        public void ShouldFailUpdateProfileOnWrongId()
        {
            SqlLiteDatabaseTest(options =>
            {
                var playListService = _server.Host.Services.GetService(typeof(IProfileService)) as IProfileService;

                using (var context = new PlayCatDbContext(options))
                {
                    playListService.SetDbContext(context);

                    var user = context.CreateUser("test@mail.com", "v", "last", "m", "123");
                    context.SaveChanges();

                    var result = playListService.UpdateProfile(new DataService.Request.UpdateProfileRequest
                    {
                        Id = Guid.Empty
                    });

                    CheckIfFail(result);
                    Assert.True(result.Info.Any());
                }
            });
        }

        [Fact]
        public void ShouldUpdateProfile()
        {
            SqlLiteDatabaseTest(options =>
            {
                var playListService = _server.Host.Services.GetService(typeof(IProfileService)) as IProfileService;

                using (var context = new PlayCatDbContext(options))
                {
                    playListService.SetDbContext(context);

                    var user = context.CreateUser("test@mail.com", "v", "last", "m", "123");
                    context.SaveChanges();

                    string newFirstName = "newFirstName";
                    string newLastName = "newLastName";
                    string newNickname = "newNickName";
                    var result = playListService.UpdateProfile(new DataService.Request.UpdateProfileRequest
                    {
                        Id = user.Id,
                        FirstName = newFirstName,
                        LastName = newLastName,
                        NickName = newNickname
                    });

                    CheckIfSuccess(result);
                    Assert.NotNull(result.User);
                    Assert.Equal(newFirstName, result.User.FirstName);
                    Assert.Equal(newLastName, result.User.LastName);
                    Assert.Equal(newNickname, result.User.NickName);
                }
            });
        }

        public Profile(ITestOutputHelper output) : base(output)
        {
        }
    }
}
