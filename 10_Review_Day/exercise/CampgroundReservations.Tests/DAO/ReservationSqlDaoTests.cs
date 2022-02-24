using CampgroundReservations.DAO;
using CampgroundReservations.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CampgroundReservations.Tests.DAO
{
    [TestClass]
    public class ReservationSqlDaoTests : BaseDaoTests
    {
        [TestMethod]
        public void CreateReservation_Should_ReturnNewReservationId()
        {
            // Arrange
            ReservationSqlDao dao = new ReservationSqlDao(ConnectionString);

            // Act
            int returnedId = dao.CreateReservation(SiteId, "Test Name", DateTime.Now.AddDays(1), DateTime.Now.AddDays(3));

            // Assert
            Assert.AreEqual(ReservationId + 1, returnedId);
        }

        [TestMethod] 
        public void UpcomingReservationTestReturnsAll30DaysFromNow()
        {
            // Arrange
            ReservationSqlDao dao = new ReservationSqlDao(ConnectionString);

            // Act
            IList<Reservation> upcomingReservations30Days = dao.UpcomingReservation(ParkId);

            // Assert
            Assert.AreEqual(upcomingReservations30Days.Count, 2);

        }


    }
}
