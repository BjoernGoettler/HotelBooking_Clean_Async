using System;

namespace HotelBooking.Core
{
    public interface IDateTimeProvider
    {
        DateTime Today { get; }
    }

}