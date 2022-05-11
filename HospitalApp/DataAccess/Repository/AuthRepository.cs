using Dapper;
using DataAccess.Constant;
using DataAccess.Interfaces;
using DataAccess.Services.Contract;
using Models.Models;
using System.Data;

namespace DataAccess.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDataContext _context;

        public AuthRepository(IDataContext context)
        {
            _context = context;
        }

        public User Login(User user)
        {
            User checkUser = new();

            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                var response = dbConnection.Query<User>
                    (SpConstants.LoginUser, new
                    {
                        user.Email
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                if (response != null && response.FirstOrDefault()!.Email == user.Email)
                {
                    checkUser.UserID = response.FirstOrDefault()!.UserID;
                    checkUser.Email = response.FirstOrDefault()!.Email;
                    checkUser.Roles = response.FirstOrDefault()!.Roles;
                    checkUser.PasswordHash = response.FirstOrDefault()!.PasswordHash;
                    checkUser.PasswordSalt = response.FirstOrDefault()!.PasswordSalt;
                }
                else
                {
                    checkUser.Response = response!.FirstOrDefault()!.Response;
                }

                dbConnection.Close();

                return checkUser;

            }
            catch (Exception ex)
            {
                checkUser.Response = ex.Message;
                return checkUser;
            }
        }

        public string Register(User user)
        {
            try
            {
                using IDbConnection dbConnection = _context.CreateConnection();

                dbConnection.Open();

                // Execute function uses Dapper:
                var response = dbConnection.Query<User>
                    (SpConstants.RegisterUser, new
                    {
                        Email = user.Email,
                        PasswordHash = user.PasswordHash,
                        PasswordSalt = user.PasswordSalt,
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                string result;

                if (response != null && response.FirstOrDefault()!.Response == "Registed Successfully")
                {
                    result = "Registed Successfully";
                }
                else
                {
                    result = "Email was taken";
                }

                dbConnection.Close();

                return result;

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return message;
            }
        }
    }
}
