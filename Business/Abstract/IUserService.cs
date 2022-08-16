using Core.Entities.Concrete;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService : IEntityBaseService<User>
    {
        IDataResult<User> GetByMail(string mail);
        IDataResult<List<OperationClaim>> GetClaims(User user);

    }
}
