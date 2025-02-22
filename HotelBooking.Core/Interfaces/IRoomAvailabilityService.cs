using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooking.Core.Interfaces
{
    public interface IRoomAvailabilityService
    {
        int FindAvailableRoom(DateTime startDate, DateTime endDate, IEnumerable<Room> rooms, IEnumerable<Booking> bookings);
    }
}