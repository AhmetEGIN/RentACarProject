using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Helpers
{
    public class FileHelperManager : IFileHelper
    {
        public string Add(string root, IFormFile formFile)
        {

            string imageExtension = Path.GetExtension(formFile.FileName);
            string imagePath = Guid.NewGuid().ToString() + imageExtension;

            using (FileStream fileStream = File.Create(root + imagePath))
            {
                formFile.CopyTo(fileStream);
                fileStream.Flush();
                return imagePath;
            }

            
        }

        public void Delete(string filePath)
        {
            var result = CheckImage(filePath);
            if (!result.Success)
            {
                Console.WriteLine(result.Message);
            }
            File.Delete(filePath);

        }

        public string Update(string filePath, string root, IFormFile formFile)
        {

            File.Delete(filePath);

            return Add(root, formFile);
        }


        // private codes

        private IResult CheckImage(string filePath)
        {
            if (!File.Exists(filePath))
            {

                return new ErrorResult("Kein Bild gefunden");
            }
            return new SuccessResult();
        }
     
        private void CreateDirectory(string root)
        {
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }

    }
}
