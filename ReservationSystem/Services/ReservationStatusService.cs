using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Services
{
    public class ReservationStatusService
    {
        private readonly ApplicationDbContext _cxt;

        public async void ArchivePastReservations()
        {
            var expiredReservations = _cxt.Reservations.Where(r => r.Sitting.Date < DateTime.Now.AddDays(-1) && (int)r.Status < 2);
            var seatedReservations = _cxt.Reservations.Where(r => r.Sitting.Date < DateTime.Now.AddDays(-1) && (int)r.Status == 2);
            foreach (var r in expiredReservations)
            {
                r.Status = Data.Enums.ReservationStatus.Expired;
            }
            foreach (var r in seatedReservations)
            {
                r.Status = Data.Enums.ReservationStatus.Completed;
            }
            await _cxt.SaveChangesAsync();
        }


    }
}
