using Business.Abstract;
using Business.Constants;
using Business.Constants.PathConstants;
using Core.Utilities.Business;
using Core.Utilities.Helpers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        IFileHelper _fileHelper;
        public CarImageManager(ICarImageDal carImageDal, IFileHelper fileHelper)
        {
            _carImageDal = carImageDal;
            _fileHelper = fileHelper;
        }

        public IResult Add(CarImage carImage, IFormFile formFile)
        {
            IResult result = BusinessRules.Run(CheckCarImageCount(carImage.CarId));
            if (result is not null)
            {
                return result;
            }

            carImage.Date = DateTime.Now;
            carImage.ImagePath = _fileHelper.Add(ImagePathConstants.ImagePathConstant, formFile);
            _carImageDal.Add(carImage);
            return new SuccessResult(Messages.CarImageAdded);
        }

        public IResult Delete(int imageId)
        {
            var carImage = GetImage(imageId);
            _fileHelper.Delete(ImagePathConstants.ImagePathConstant+ carImage.Data.ImagePath);
            _carImageDal.Delete(carImage.Data);
            return new SuccessResult(Messages.CarImageDeleted);
        }

        public IDataResult<List<CarImage>> GetCarImagesById(int carId)
        {
            IResult result = BusinessRules.Run(CheckCarImage(carId));
            if (result is null)
            {
                return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(c => c.CarId == carId));
            }
            return new ErrorDataResult<List<CarImage>>(GetDefaultImage().Data);
        }

        public IDataResult<CarImage> GetImage(int imageId)
        {
            var result = _carImageDal.Get(c => c.Id == imageId);
            return new SuccessDataResult<CarImage>(result);
        }

        public IDataResult<List<CarImage>> GetImages()
        {
            var result = _carImageDal.GetAll();
            return new SuccessDataResult<List<CarImage>>(result);
        }

        public IResult Update(int imageId, IFormFile file)
        {
            var carImage = GetImage(imageId);
            if (carImage.Data is null)
            {
                return new ErrorResult(Messages.CarImageNotFound);
            }
            carImage.Data.Date = DateTime.Now;
            _fileHelper.Update(ImagePathConstants.ImagePathConstant + carImage.Data.ImagePath, ImagePathConstants.ImagePathConstant, file);
            _carImageDal.Update(carImage.Data);
            return new SuccessResult(Messages.CarImageUpdated);
            
        }

        private IResult CheckCarImageCount(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result >= 5)
            {
                return new ErrorResult(Messages.CarImagesCountLimit);
            }
            return new SuccessResult();
        }

        private IResult CheckCarImage(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Any();
            if (result==null)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }

        private IDataResult<List<CarImage>> GetDefaultImage()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(c => c.ImagePath == ImagePathConstants.DefaultImagePath));
        }

    }
}
