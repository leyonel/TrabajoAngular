using System.Data;

namespace DataAccess.Services.Contract
{
    public interface IDataContext
    {
        public IDbConnection CreateConnection();
    }
}
