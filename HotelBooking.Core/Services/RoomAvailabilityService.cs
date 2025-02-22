using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core.Interfaces;
namespace HotelBooking.Core
{
    public class RoomAvailabilityService : IRoomAvailabilityService
    {
        public int FindAvailableRoom(DateTime startDate, DateTime endDate, IEnumerable<Room> rooms, IEnumerable<Booking> bookings)
        {
            var activeBookings = bookings.Where(b => b.IsActive);

            foreach (var room in rooms)
            {
                var activeBookingsForRoom = activeBookings.Where(b => b.RoomId == room.Id);
                if (activeBookingsForRoom.All(b => startDate < b.StartDate &&
                        endDate < b.StartDate || startDate > b.EndDate && endDate > b.EndDate))
                {
                    return room.Id;
                }
            }

            return -1; // No room available
        }
    }

}