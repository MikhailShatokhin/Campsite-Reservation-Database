using CampgroundReservations.DAO;
using CampgroundReservations.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CampgroundReservations.Tests.DAO
{
    [TestClass]
    public class SiteSqlDaoTests : BaseDaoTests
    {
        [TestMethod]
        public void GetSitesThatAllowRVs_Should_ReturnSites()
        {
            // Arrange
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            // Act
            IList<Site> sites = dao.GetSitesThatAllowRVs(ParkId);

            // Assert
            Assert.AreEqual(2, sites.Count);
        }

        [TestMethod]
        public void GetSitesAvaliableTest()
        {
            // Arrange
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            // Act
            IList<Site> availableSites = dao.GetSitesAvaliable(ParkId);

            // Assert
            Assert.AreEqual(availableSites.Count, 2);

        }
    }
}
