using System;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using Finanzas.Models;

namespace Finanzas.Data;

public class DashboardData
{
    public List<Month> GetMonths()
    {
        string queryString = "SELECT * FROM Months";

        List<Month> Months = new List<Month>();
        
        int idCurrentMonth = 0;
        string nameCurrentMonth = "";

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr["Name"].ToString() == (DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US"))))
                        {
                            idCurrentMonth = Int32.Parse(dr["idMonth"].ToString());
                            nameCurrentMonth = dr["Name"].ToString();
                        }
                        
                        var Month = new Month();
                        Month.idMonth = Int32.Parse(dr["idMonth"].ToString());
                        Month.Name = dr["Name"].ToString();
                        Month.StartDate = dr["StartDate"].ToString();
                        Month.EndDate = dr["EndDate"].ToString();
                        
                        Months.Add(Month);
                    }

                    return Months;
                }
            }
        }
        catch (Exception ex)
        {
            return new List<Month>();
        }
    }

    public Tuple<int, string> GetCurrentOrSelectedMonth(string name = "")
    {
        string nameMonth = "";

        if (name == "")
        {
            nameMonth = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US"));
        }
        else
        {
            nameMonth = name;
        }

        string queryString = "SELECT * FROM Months";
        
        int idCurrentMonth = 0;
        string nameCurrentMonth = "";

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (dr["Name"].ToString() == nameMonth)
                        {
                            idCurrentMonth = Int32.Parse(dr["idMonth"].ToString());
                            nameCurrentMonth = dr["Name"].ToString();
                        }
                    }

                    return new Tuple<int, string>(idCurrentMonth, nameCurrentMonth);
                }
            }
        }
        catch (Exception ex)
        {
            return new Tuple<int, string>(idCurrentMonth, nameCurrentMonth);
        }
    }

    public List<Categories> GetCategories(int idUser)
    {

        string queryString = "SELECT * FROM Categories WHERE idUser = " + idUser;

        List<Categories> categories = new List<Categories>();

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var category = new Categories();
                        category.idCategory = Int32.Parse(dr["idCategory"].ToString());
                        category.Name = dr["Name"].ToString();
                        category.idIcon = Int32.Parse(dr["idIcon"].ToString());
                        category.idUser = Int32.Parse(dr["idUser"].ToString());
                        
                        categories.Add(category);
                    }

                    return categories;
                }
            }
        }
        catch (Exception ex)
        {
            return new List<Categories>();
        }
    }

    public List<Categories> AddCategory(Categories oCategories)
    {
        string queryString = "INSERT INTO Categories (Name, idIcon, idUser) VALUES (@Name, @idIcon, @idUser)";

        List<Categories> categories = new List<Categories>();

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Name", oCategories.Name);
                command.Parameters.AddWithValue("@idIcon", "1");
                command.Parameters.AddWithValue("@idUser", oCategories.idUser);
                command.ExecuteNonQuery();
                connection.Close();
                
                categories = GetCategories(oCategories.idUser);

                return categories;
            }
        }
        catch (Exception ex)
        {
            return new List<Categories>();
        }
    }
    
    public List<PaymentMethods> GetPaymentMethods(int idUser)
    {

        string queryString = "SELECT * FROM PaymentsMethods WHERE idUser = " + idUser;

        List<PaymentMethods> paymentMethods = new List<PaymentMethods>();

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var paymentMethod = new PaymentMethods();
                        paymentMethod.idPaymentMethod = Int32.Parse(dr["idPaymentMethod"].ToString());
                        paymentMethod.Name = dr["Name"].ToString();
                        paymentMethod.idUser = Int32.Parse(dr["idUser"].ToString());
                        
                        paymentMethods.Add(paymentMethod);
                    }

                    return paymentMethods;
                }
            }
        }
        catch (Exception ex)
        {
            return new List<PaymentMethods>();
        }
    }
    public void AddPaymentMethod(PaymentMethods oPaymentMethod)
    {
        string queryString = "INSERT INTO PaymentsMethods (Name, idUser) VALUES (@Name, @idUser)";

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Name", oPaymentMethod.Name);
                command.Parameters.AddWithValue("@idUser", oPaymentMethod.idUser);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //return new List<Categories>();
        }
    }
}