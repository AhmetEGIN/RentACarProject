using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Helpers
{
    public interface IFileHelper
    {
        string Add(string root, IFormFile formFile);
        void Delete(string filePath);
        string Update(string filePath, string root, IFormFile formFile);

    }
}
