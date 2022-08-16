using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;
        ICarService _carService;

        public RentalManager(IRentalDal rentalDal, ICarService carService)
        {
            _rentalDal = rentalDal;
            _carService = carService;
        }


        public IResult Add(Rental item)
        {
            IResult result = BusinessRules.Run(IsCarRentable(item.CarId));
            if (result == null)
            {
                item.RentPrice = TotalPrice(item.RentDate, item.ReturnDate, item.CarId);
                _rentalDal.Add(item);
                return new SuccessResult(Messages.CarRented);
            }
            return new ErrorResult(Messages.CarNotAvailable);

        }

        public IResult Delete(Rental item)
        {
            _rentalDal.Delete(item);
            return new SuccessResult(Messages.RentalDeleted);
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
        }

        public IDataResult<Rental> GetById(int id)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r=>r.Id == id), Messages.RentalListed);
        }

        public IResult Update(Rental item)
        {
            _rentalDal.Update(item);
            return new SuccessResult(Messages.RentalUpdated);
        }

        public int TotalPrice(DateTime rentDate, DateTime returnDate, int carId)
        {
            int dayDifference = (returnDate - rentDate).Days;
            int price = dayDifference * Decimal.ToInt16(_carService.GetById(carId).Data.DailyPrice);
            return price;
        }

        private IResult IsCarRentable(int carId)
        {
            var result = _rentalDal.GetAll(r => r.CarId == carId).Where(r => r.ReturnDate == null);
            if (result is not null)
            {
                return new SuccessResult();
            }
            return new ErrorResult();
        }


    }
}
