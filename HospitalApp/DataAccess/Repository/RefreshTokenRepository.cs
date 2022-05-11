using Dapper;
using DataAccess.Constant;
using DataAccess.Interfaces;
using DataAccess.Services.Contract;
using Models.Models;
using System.Data;

namespace DataAccess.Repository
{
    public class RefreshTokenRepository : IRefreshToken
    {
        private readonly IDataContext _context;

        public RefreshTokenRepository(IDataContext context)
        {
            _context = context;
        }

        public User FindById(int request)
        {
            User checkUser = new();

            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<User>
                    (SpConstants.FindById, new
                    {
                        UserID = request
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                if (response != null && response.FirstOrDefault()!.UserID == request)
                {
                    checkUser.UserID = response.FirstOrDefault()!.UserID;
                    checkUser.Email = response.FirstOrDefault()!.Email;
                    checkUser.Roles = response.FirstOrDefault()!.Roles;
                    checkUser.PasswordHash = response.FirstOrDefault()!.PasswordHash;
                    checkUser.PasswordSalt = response.FirstOrDefault()!.PasswordSalt;
                }

                dbConnection.Close();

                return checkUser;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveNewToken(int id,  string refreshToken)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<User>
                    (SpConstants.SaveNewToken, new
                    {
                        UserID = id,
                        Token = refreshToken
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                dbConnection.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateRefreshToken(RefreshToken request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<RefreshToken>
                    (SpConstants.UpdateRefreshToken, new
                    {
                        Id = request.Id,
                        UserID = request.UserID,
                        Token = request.Token,
                        IsUsed = request.IsUsed,
                        IsRevoked = request.IsRevoked,
                        AddedDate = request.AddedDate,
                        ExpireDate = request.ExpireDate
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                dbConnection.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public RefreshToken FirstOrDefault(string request)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<RefreshToken>
                    (SpConstants.FindRefreshToken, new
                    {
                        Token = request
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                RefreshToken result = response![0];

                dbConnection.Close();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<RefreshToken>
                    (SpConstants.AddValuesToRefreshToken, new
                    {
                        IsUsed = refreshToken.IsUsed,
                        IsRevoked = refreshToken.IsRevoked,
                        UserID = refreshToken.UserID,
                        AddedDate = refreshToken.AddedDate,
                        ExpireDate = refreshToken.ExpireDate,
                        Token = refreshToken.Token
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                dbConnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveLoginToken(AuthResult request, User user)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<User>
                    (SpConstants.SaveToken, new
                    {
                        Email = user.Email,
                        Token = request.Token
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ToList();

                dbConnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
