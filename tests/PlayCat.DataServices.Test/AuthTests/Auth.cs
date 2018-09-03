using System;
using System.Linq;
using PlayCat.DataModels;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.AuthResponse;
using Xunit;
using Xunit.Abstractions;

namespace PlayCat.DataServices.Test.AuthTests
{
    public class Auth : BaseTest
    {
        #region Sign Up

        [Fact]
        public void IsEmptyModelSignUp()
        {            
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;
                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    SignUpInResult result = authService.SignUp(new SignUpRequest { });

                    Assert.NotNull(result);
                    Assert.False(result.Ok);
                    Assert.Equal("Model is not valid", result.Info);
                    Assert.False(result.ShowInfo);
                    Assert.Null(result.User);
                    Assert.Null(result.AuthToken);
                    Assert.NotNull(result.Errors);
                    Assert.True(result.Errors.Any());
                }
            });
        }
        

        [Fact]
        public void IsValidModelSignUp()
        {
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    SignUpInResult result = authService.SignUp(new SignUpRequest
                    {
                        FirstName = "vlad",
                        LastName = "Kuz",
                        Password = "123456abc",
                        ConfirmPassword = "123456abc",
                        Email = "mefgalm@gmail.com",
                    });

                    Assert.NotNull(result);
                    Assert.True(result.Ok);
                    Assert.NotNull(result.User);
                    Assert.NotNull(result.AuthToken);
                    Assert.Null(result.Errors);
                    Assert.Null(result.Info);
                }
            });                        
        }

        [Fact]
        public void IsUsedEmailSignUp()
        {            
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    SignUpInResult result = authService.SignUp(new SignUpRequest
                    {
                        FirstName = "vlad",
                        LastName = "Kuz",
                        Password = "123456abc",
                        ConfirmPassword = "123456abc",
                        Email = "mefgalm@gmail.com",
                    });

                    SignUpInResult result2 = authService.SignUp(new SignUpRequest
                    {
                        FirstName = "vlad",
                        LastName = "Kuz",
                        Password = "123456abc",
                        ConfirmPassword = "123456abc",
                        Email = "mefgalm@gmail.com",
                    });

                    Assert.NotNull(result2);
                    Assert.False(result2.Ok);
                    Assert.Null(result2.User);
                    Assert.Null(result2.AuthToken);
                    Assert.Null(result2.Errors);
                    Assert.Equal("User with this email already registered", result2.Info);
                    Assert.True(result2.ShowInfo);
                }
            });            
        }
       

        #endregion

        #region Sign In

        [Fact]
        public void IsValidModelSignIn()
        {           
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    SignUpInResult result = authService.SignUp(new SignUpRequest
                    {
                        FirstName = "vlad",
                        LastName = "Kuz",
                        Password = "123456abc",
                        ConfirmPassword = "123456abc",
                        Email = "mefgalm@gmail.com",
                    });

                    Assert.NotNull(result);
                    Assert.True(result.Ok);
                    Assert.NotNull(result.User);
                    Assert.NotNull(result.AuthToken);
                    Assert.Null(result.Errors);
                    Assert.Null(result.Info);

                    SignUpInResult resultSignIn = authService.SignIn(new SignInRequest
                    {
                        Email = "mefgalm@gmail.com",
                        Password = "123456abc"
                    });

                    Assert.NotNull(resultSignIn);
                    Assert.True(resultSignIn.Ok);
                    Assert.NotNull(resultSignIn.User);
                    Assert.NotNull(resultSignIn.AuthToken);
                    Assert.Null(resultSignIn.Errors);
                    Assert.Null(resultSignIn.Info);
                }
            });
        }

        [Fact]
        public void IsUserNotFoundSignIn()
        {            
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);
                    SignUpInResult resultSignIn = authService.SignIn(new SignInRequest
                    {
                        Email = "mefgalm@gmail.com",
                        Password = "123456abc"
                    });

                    Assert.NotNull(resultSignIn);
                    Assert.False(resultSignIn.Ok);
                    Assert.Null(resultSignIn.User);
                    Assert.Null(resultSignIn.AuthToken);
                    Assert.Null(resultSignIn.Errors);
                    Assert.Equal("Email or password is incorrect", resultSignIn.Info);
                    Assert.True(resultSignIn.ShowInfo);
                }
            });            
        }

        [Fact]
        public void IsUserFoundButWrongPasswordSignIn()
        {            
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    SignUpInResult result = authService.SignUp(new SignUpRequest
                    {
                        FirstName = "vlad",
                        LastName = "Kuz",
                        Password = "123456abc",
                        ConfirmPassword = "123456abc",
                        Email = "mefgalm@gmail.com",
                    });

                    Assert.NotNull(result);
                    Assert.True(result.Ok);
                    Assert.NotNull(result.User);
                    Assert.NotNull(result.AuthToken);
                    Assert.Null(result.Errors);
                    Assert.Null(result.Info);

                    SignUpInResult resultSignIn = authService.SignIn(new SignInRequest
                    {
                        Email = "mefgalm@gmail.com",
                        Password = "wrongPass1234"
                    });

                    Assert.NotNull(resultSignIn);
                    Assert.False(resultSignIn.Ok);
                    Assert.Null(resultSignIn.User);
                    Assert.Null(resultSignIn.AuthToken);
                    Assert.Null(resultSignIn.Errors);
                    Assert.Equal("Email or password is incorrect", resultSignIn.Info);
                    Assert.True(resultSignIn.ShowInfo);
                }
            });
        }


        [Fact]
        public void IsUpdateTokenSignIn()
        {
            SqlLiteDatabaseTest(options =>
            {
                var authService = _server.Host.Services.GetService(typeof(IAuthService)) as IAuthService;

                using (var context = new PlayCatDbContext(options))
                {
                    authService.SetDbContext(context);

                    string password = "123456abc";
                    string email = "test@gmail.com";

                    var salt = new Crypto().GenerateSalt();
                    var passwordHah = new Crypto().HashPassword(salt, password);

                    var user = context.Users.Add(new User
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        FirstName = "test",
                        LastName = "test",
                        PasswordHash = passwordHah,
                        PasswordSalt = salt,
                        RegisterDate = DateTime.Now,
                    });

                    var authToken = context.AuthTokens.Add(new AuthToken
                    {
                        Id = Guid.NewGuid(),
                        DateExpired = DateTime.Now.AddDays(-1),
                        IsActive = false,
                        UserId = user.Entity.Id
                    });

                    context.SaveChanges();

                    SignUpInResult result = authService.SignIn(new SignInRequest
                    {
                        Email = email,
                        Password = password
                    });

                    var updatedAuthToken = context.AuthTokens.FirstOrDefault();

                    Assert.NotNull(updatedAuthToken);
                    Assert.True(updatedAuthToken.DateExpired > DateTime.Now);
                    Assert.True(updatedAuthToken.IsActive);
                }
            });
        }

        #endregion

        public Auth(ITestOutputHelper output) : base(output)
        {
        }
    }
}
