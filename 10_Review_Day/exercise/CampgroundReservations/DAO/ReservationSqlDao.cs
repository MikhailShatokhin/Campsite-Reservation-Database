using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ReservationSqlDao : IReservationDao
    {
        private readonly string connectionString;

        public ReservationSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int CreateReservation(int siteId, string name, DateTime fromDate, DateTime toDate)
        {
            int newReservation = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date) " +
                                                "OUTPUT INSERTED.reservation_id " +
                                                "VALUES (@site_id, @name, @from_date, @to_date);", conn);
                                                

                cmd.Parameters.AddWithValue("@site_id", siteId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@from_date", fromDate);
                cmd.Parameters.AddWithValue("@to_date", toDate);

                newReservation = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return newReservation;
        }

        public IList<Reservation> UpcomingReservation(int parkId)
        {
            IList<Reservation> upcomingReservations = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM reservation r " +
                                                "JOIN site s ON s.site_id = r.site_id " +
                                                "JOIN campground c ON c.campground_id = s.campground_id " +
                                                "WHERE c.park_id = @park_id AND r.from_date BETWEEN GETDATE() AND DATEADD(d,+30,GETDATE());", conn);

                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation upcomingReservation = GetReservationFromReader(reader);
                    upcomingReservations.Add(upcomingReservation);
                }
                
            }
            return upcomingReservations;
        }

        //Bonus Question #2
        public IList<Reservation> GetFureSitesAvaliable(int parkId, DateTime fromDate, DateTime todate)
        {
            IList<Reservation> futureAvailableSites = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM site s " +
                                                "JOIN campground c ON c.campground_id = s.campground_id " +
                                                "JOIN reservation r ON r.site_id = s.site_id " +
                                                "WHERE c.park_id = @park_id AND GETDATE()+1 NOT BETWEEN r.from_date AND r.to_date;", conn);
                //"GROUP BY s.site_id, s.campground_id, s.max_occupancy, s.max_rv_length, s.accessible, s.site_number, s.utilities " +
                //"ORDER BY s.site_id;", conn);                                            

                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation futureAvaliableSite = GetReservationFromReader(reader);
                    futureAvailableSites.Add(futureAvaliableSite);
                }
            }
            return futureAvailableSites;
        }

        private Reservation GetReservationFromReader(SqlDataReader reader)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            reservation.SiteId = Convert.ToInt32(reader["site_id"]);
            reservation.Name = Convert.ToString(reader["name"]);
            reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
            reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
            reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

            return reservation;
        }
    }
}
