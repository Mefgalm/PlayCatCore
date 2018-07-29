using PlayCat.DataService.Mappers;
using PlayCat.DataService.Request;
using PlayCat.DataService.Response;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlayCat.DataService.Response.AuthResponse;

namespace PlayCat.DataService
{
    public class AuthService : BaseService, IAuthService
    {
        private const int AuthTokenDaysExpired = 180;        

        private readonly ICrypto _crypto;

        public AuthService(PlayCatDbContext dbContext, ILoggerFactory loggerFactory, ICrypto crypto) 
            : base(dbContext, loggerFactory.CreateLogger<AuthService>())
        {
            _crypto = crypto;
        }

        public SignUpInResult SignUp(SignUpRequest request)
        {
            return BaseInvokeCheckModel(request, () =>
            {
                var responseBuilder =
                    ResponseBuilder<SignUpInResult>
                    .Fail();

                if (_dbContext.Users.Any(x => x.Email == request.Email))
                    return responseBuilder.SetInfoAndBuild("User with this email already registered");

                var salt = _crypto.GenerateSalt();
                var passwordHah = _crypto.HashPassword(salt, request.Password);

                var dataUser = UserMapper.ToData.FromRequest(request, user =>
                {
                    user.Id = Guid.NewGuid();
                    user.IsUploadingAudio = false;
                    user.PasswordHash = passwordHah;
                    user.PasswordSalt = salt;
                    user.RegisterDate = DateTime.Now;
                });

                var dataAuthToken = new DataModel.AuthToken
                {
                    Id = Guid.NewGuid(),
                    DateExpired = DateTime.Now.AddDays(AuthTokenDaysExpired),
                    UserId = dataUser.Id,
                    IsActive = true
                };

                var playlist = new DataModel.Playlist
                {
                    Id = Guid.NewGuid(),
                    IsGeneral = true,
                    OwnerId = dataUser.Id,
                    OrderValue = 0
                };

                _dbContext.AuthTokens.Add(dataAuthToken);
                _dbContext.Users.Add(dataUser);
                _dbContext.Playlists.Add(playlist);
                _dbContext.SaveChanges();

                return ResponseBuilder<SignUpInResult>.SuccessBuild(new SignUpInResult
                {
                    User = UserMapper.ToApi.FromData(dataUser),
                    AuthToken = AuthTokenMapper.ToApi.FromData(dataAuthToken)
                });
            });            
        }

        public SignUpInResult SignIn(SignInRequest request)
        {
            return BaseInvokeCheckModel(request, () =>
            {
                DataModel.User dataUser = _dbContext.Users
                    .Include(x => x.AuthToken)
                    .FirstOrDefault(x => x.Email == request.Email);

                if (dataUser == null || !_crypto.IsValid(dataUser.PasswordHash, dataUser.PasswordSalt, request.Password))
                    return ResponseBuilder<SignUpInResult>.Fail().SetInfoAndBuild("Email or password is incorrect");

                UpdateAuthToken(dataUser.AuthToken);

                _dbContext.SaveChanges();

                return ResponseBuilder<SignUpInResult>.SuccessBuild(new SignUpInResult
                {
                    User = UserMapper.ToApi.FromData(dataUser),
                    AuthToken = AuthTokenMapper.ToApi.FromData(dataUser.AuthToken)
                });
            });
        }

        public CheckTokenResult CheckToken(string token)
        {
            var responseBuilder =
                ResponseBuilder<CheckTokenResult>
                .Fail()
                .IsShowInfo(false)
                .SetCode(ResponseCode.InvalidToken);

            if (string.IsNullOrEmpty(token))
                return responseBuilder.SetInfoAndBuild("Token not found in headers");

            if(!Guid.TryParse(token, out Guid tokenId))
                return responseBuilder.SetInfoAndBuild("Token wrong format");

            DataModel.AuthToken authToken = _dbContext.AuthTokens.FirstOrDefault(x => x.Id == tokenId);

            if(authToken == null)
                return responseBuilder.SetInfoAndBuild("Token not registered");

            if(authToken.DateExpired < DateTime.Now)
                return responseBuilder.SetInfoAndBuild("Token is expired");

            if(!authToken.IsActive)
                return responseBuilder.SetInfoAndBuild("Token is not active");

            return ResponseBuilder<CheckTokenResult>.SuccessBuild(new CheckTokenResult
            {
                AuthToken = authToken
            });
        }

        private void UpdateAuthToken(DataModel.AuthToken token)
        {
            token.DateExpired = DateTime.Now.AddDays(AuthTokenDaysExpired);
            token.IsActive = true;
        }
    }
}
