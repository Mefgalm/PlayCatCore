using PlayCat.ApiModels;

namespace PlayCat.DataServices.Mappers
{
    public static class AuthTokenMapper
    {
        public static class ToApi
        {
            public static AuthToken FromData(DataModels.AuthToken dataToken)
            {
                return dataToken == null ? null : new AuthToken
                {
                    Id = dataToken.Id,
                    DateExpired = dataToken.DateExpired                       
                };
            }
        }
    }
}
