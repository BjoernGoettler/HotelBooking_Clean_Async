using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private Mock<IRepository<Room>> mockRoomRepository;
        private Mock<IRepository<Booking>> mockBookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            mockRoomRepository = new Mock<IRepository<Room>>();
            mockBookingRepository = new Mock<IRepository<Booking>>();
            
            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "Room 1" },
                new Room { Id = 2, Description = "Room 2" }
            };
            var customers = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    Name = "Customer One",
                    Email = "customer_one@example.org"
                },
                new Customer
                {
                    Id = 2,
                    Name = "Customer Two",
                    Email = "customer_two@example.org"
                }
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    StartDate = start,
                    EndDate = end,
                    IsActive = true,
                    CustomerId = customers[0].Id,
                    RoomId = 1
                },
                new Booking
                {
                    Id = 2,
                    StartDate = start,
                    EndDate = end,
                    IsActive = true,
                    CustomerId = customers[1].Id,
                    RoomId = 2
                }
            };
            
            mockRoomRepository.Setup(x => x.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Room>>(rooms));
            mockRoomRepository.Setup(x => x.GetAsync(It.IsInRange(1, 2, Moq.Range.Inclusive))).Returns(Task.FromResult<Room>(rooms[0]));
            mockRoomRepository.Setup(x => x.RemoveAsync(It.IsInRange(1, 2, Moq.Range.Inclusive))).Returns(Task.CompletedTask);

            mockBookingRepository.Setup(x => x.GetAsync(It.IsInRange(1, 2, Moq.Range.Inclusive))).Returns(Task.FromResult<Booking>(bookings[0]));
            mockBookingRepository.Setup(x => x.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Booking>>(bookings));
            mockBookingRepository.Setup(x => x.AddAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
            mockBookingRepository.Setup(x => x.RemoveAsync(It.IsInRange(1, 2, Moq.Range.Inclusive))).Returns(Task.CompletedTask);
            mockBookingRepository.Setup(x => x.EditAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
            
            //bookingRepository = new FakeBookingRepository(start, end);
            //IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }
        
        
        [Theory]
        [InlineData("13/12/2024", "12/12/2024")]
        public async Task FindAvailableRoom_StartDateLaterThanEndDate_ThrowsArgumentException(string startDateString,
            string endDateString)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateString);
            var endDate = DateTime.Parse(endDateString);
            // Act
            Task result() => bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
        
        [Fact]
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Task result() => bookingManager.FindAvailableRoom(date, date);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }
        
        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);

            var bookingForReturnedRoomId = (await mockBookingRepository.Object.GetAllAsync()).
                Where(b => b.RoomId == roomId
                           && b.StartDate <= date
                           && b.EndDate >= date
                           && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }

    }
}
