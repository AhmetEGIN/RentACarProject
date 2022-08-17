using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
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

        [SecuredOperation("admin,rental.add")]
        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental item)
        {
            IResult result = BusinessRules.Run(IsCarRentedInDateRange(item));
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
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.Id == id), Messages.RentalListed);
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

        private IResult IsCarRentedInDateRange(Rental rental)
        {
            var rentals = _rentalDal.GetAll(r => r.CarId == rental.CarId);
            foreach (var reservation in rentals)
            {
                if ((rental.ReturnDate >= reservation.RentDate && rental.ReturnDate <= reservation.ReturnDate) ||
                    (rental.RentDate >= reservation.RentDate && rental.RentDate <= reservation.ReturnDate) ||
                    (rental.RentDate <= reservation.RentDate && rental.ReturnDate >= reservation.ReturnDate)
                    )
                {
                    return new ErrorResult(Messages.CarIsAlreadyRentedInSelectedDateRange);
                }
            }
            return new SuccessResult();

        }
    }
}
