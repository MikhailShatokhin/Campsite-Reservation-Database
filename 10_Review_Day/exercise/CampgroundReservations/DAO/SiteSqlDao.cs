using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class SiteSqlDao : ISiteDao
    {
        private readonly string connectionString;

        public SiteSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Site> GetSitesThatAllowRVs(int parkId)
        {
            IList<Site> sites = new List<Site>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site s " +
                                               "JOIN campground c ON c.campground_id = s.campground_id " +
                                               "WHERE s.max_rv_length > 0 AND c.park_id = @park_id;", conn);

                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = GetSiteFromReader(reader);
                    sites.Add(site);
                }
            }
            return sites;



            throw new NotImplementedException();
        }

        ////Bonus Question #1
        //public IList<Site> GetSitesAvaliable(int parkId)
        //{
        //    IList<Site> availableSites = new List<Site>();
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM site s " +
        //                                        "JOIN campground c ON c.campground_id = s.campground_id " +
        //                                        "JOIN reservation r ON r.site_id = s.site_id " +
        //                                        "WHERE c.park_id = @park_id AND GETDATE()+1 NOT BETWEEN r.from_date AND r.to_date;", conn);
        //                                        //"GROUP BY s.site_id, s.campground_id, s.max_occupancy, s.max_rv_length, s.accessible, s.site_number, s.utilities " +
        //                                        //"ORDER BY s.site_id;", conn);                                            
        //        cmd.Parameters.AddWithValue("@park_id", parkId);
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Site avaliableSite = GetSiteFromReader(reader);
        //            availableSites.Add(avaliableSite);
        //        }
        //    }
        //    return availableSites;
        //}

        //Bonus Question #1
        public IList<Site> GetSitesAvailable(int parkId)
        {
            IList<Site> availableSites = new List<Site>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site s " +
                                                "WHERE s.site_id " +
                                                "NOT IN (SELECT s.site_id FROM site s " +
                                                "JOIN campground c ON c.campground_id = s.campground_id " +
                                                "JOIN reservation r ON r.site_id = s.site_id WHERE park_id = @park_id " +
                                                "AND GETDATE() BETWEEN r.from_date AND r.to_date " +
                                                "GROUP BY s.site_id);", conn);

                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site avaliableSite = GetSiteFromReader(reader);
                    availableSites.Add(avaliableSite);
                }
            }
            return availableSites;
        }

        private Site GetSiteFromReader(SqlDataReader reader)
        {
            Site site = new Site();
            site.SiteId = Convert.ToInt32(reader["site_id"]);
            site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            site.SiteNumber = Convert.ToInt32(reader["site_number"]);
            site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            site.Accessible = Convert.ToBoolean(reader["accessible"]);
            site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
            site.Utilities = Convert.ToBoolean(reader["utilities"]);

            return site;
        }
    }
}
