using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal : EfEntityRepositoryBase<Rental, RentACarContext>, IRentalDal
    {
        public List<RentalDetailsDto> GetRentalDetails(int rentalId)
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from c in context.Cars
                             join r in context.Rentals
                             on c.Id equals r.CarId
                             join b in context.Brands
                             on c.BrandId equals b.Id
                             where r.Id == rentalId
                             select new RentalDetailsDto
                             {
                                 BrandName = b.BrandName,
                                 CarDescription = c.Description,
                                 RentDate = r.RentDate,
                                 ReturnDate = r.ReturnDate,
                                 RentPrice = r.RentPrice
                             };
                return result.ToList();
            }
        }
    }
}
