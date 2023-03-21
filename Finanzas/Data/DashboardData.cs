using System.Data.SqlClient;
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
                        if (dr["Name"].ToString() ==
                            (DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US"))))
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

    public List<Icons> GetIcons()
    {
        string queryString = "SELECT * FROM Icons";

        List<Icons> Icons = new List<Icons>();

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
                        var Icon = new Icons();
                        Icon.idIcons = Int32.Parse(dr["idIcons"].ToString());
                        Icon.Code = dr["Code"].ToString();
                        Icon.CodeHTML = dr["CodeHTML"].ToString();

                        Icons.Add(Icon);
                    }

                    return Icons;
                }
            }
        }
        catch (Exception ex)
        {
            return new List<Icons>();
        }
    }

    public Balance GetBalance(int idUser, int idMonth)
    {
        Balance oBalance = new Balance();

        string queryString = "SELECT * FROM Balance WHERE idUser = " + idUser + " AND idMonth = " + idMonth;

        var bills = GetBills(idUser, idMonth);


        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);


                using (var dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        oBalance.idBalance = Int32.Parse(dr["idBalance"].ToString());
                        oBalance.Income = dr["Income"].ToString();
                        oBalance.Expenses = dr["Expenses"].ToString();
                        oBalance.Remaining = dr["Remaining"].ToString();
                        oBalance.idUser = Int32.Parse(dr["idUser"].ToString() ?? "");
                        oBalance.idMonth = Int32.Parse(dr["idMonth"].ToString() ?? "");

                        return oBalance;
                    }
                    else
                    {
                        try
                        {
                            double expenses = bills.Sum(b => b.Cost);
                            double remaining = 0 - expenses;

                            queryString =
                                "INSERT INTO Balance (Income, Expenses, Remaining, idUser, idMonth) VALUES (0, " +
                                expenses + ", " + remaining + " , " + idUser + ", " + idMonth + ")";
                            cn = new Connection();
                            using (var connection2 = new SqlConnection(cn.getCadenaSQL()))
                            {
                                connection2.Open();
                                var command2 = new SqlCommand(queryString, connection2);
                                command2.ExecuteNonQuery();

                                return GetBalance(idUser, idMonth);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return oBalance;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return oBalance;
        }

        UpdateBalance(idUser, idMonth);
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

    public List<Bills> GetBills(int idUser, int idMonth)
    {
        string queryString = "SELECT * FROM Bills WHERE idUser = " + idUser + " AND idMonth = " + idMonth;

        List<Bills> Bills = new List<Bills>();

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
                        var bills = new Bills();
                        bills.idBill = Int32.Parse(dr["idBill"].ToString() ?? "");
                        bills.idIcons = Int32.Parse(dr["idIcons"].ToString() ?? "");
                        bills.Name = dr["Name"].ToString() ?? "";
                        bills.Cost = Double.Parse(dr["Cost"].ToString() ?? "");
                        bills.PaymentDate = dr["PaymentDate"].ToString() ?? "";
                        bills.Paid = dr["Paid"].ToString() ?? "";
                        bills.Recurring = dr["Recurring"].ToString() ?? "";
                        bills.idMonth = Int32.Parse(dr["idMonth"].ToString() ?? "");
                        bills.idUser = Int32.Parse(dr["idUser"].ToString() ?? "");
                        bills.idCategory = Int32.Parse(dr["idCategory"].ToString() ?? "");
                        bills.idPaymentMethod = Int32.Parse(dr["idPaymentMethod"].ToString() ?? "");

                        Bills.Add(bills);
                    }

                    return Bills;
                }
            }
        }
        catch (Exception ex)
        {
            return new List<Bills>();
        }
    }

    public void AddBill(Bills Bills)
    {
        string queryString =
            $"INSERT INTO Bills (idIcons, Name, Cost, PaymentDate, Paid, Recurring, idMonth, idUser, idCategory, idPaymentMethod) VALUES ({Bills.idIcons}, '{Bills.Name}', {Bills.Cost}, '{Bills.PaymentDate}', '{Bills.Paid}', '{Bills.Recurring}', {Bills.idMonth}, {Bills.idUser}, {Bills.idCategory}, {Bills.idPaymentMethod} )";

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //return new List<Categories>();
        }
    }

    public void SaveBills(List<Bills> Bills)
    {
        foreach (var Bill in Bills)
        {
            string queryString = "UPDATE Bills SET idIcons = " + Bill.idIcons + ", Name = '" + Bill.Name +
                                 "', Cost = " + Bill.Cost + ", PaymentDate = '" + Bill.PaymentDate + "', Paid = '" +
                                 Bill.Paid + "', Recurring = '" + Bill.Recurring + "' idMonth = " + Bill.idMonth +
                                 ", idUser = " + Bill.idUser +
                                 ", idCategory = " + Bill.idCategory + ", idPaymentMethod = " + Bill.idPaymentMethod +
                                 " WHERE idBill = " + Bill.idBill;

            try
            {
                var cn = new Connection();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    connection.Open();
                    var command = new SqlCommand(queryString, connection);
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

    public void SaveBalance(Balance Balance)
    {
        var bills = GetBills(Balance.idUser, Balance.idMonth);

        double expenses = bills.Sum(c => c.Cost);
        double remaining = Double.Parse(Balance.Income) - expenses;

        string queryString = "UPDATE Balance SET Income = " + Balance.Income + ", Expenses = " + expenses +
                             ", Remaining = " + remaining + " WHERE idUser = " + Balance.idUser + " AND idMonth = " +
                             Balance.idMonth;

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //return new List<Categories>();
        }
    }

    public void DeleteBill(int idBill)
    {
        string queryString = "DELETE FROM Bills WHERE idBill = " + idBill;

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //return new List<Categories>();
        }
    }

    public void UpdateBalance(int idUser, int idMonth)
    {
        var bills = GetBills(idUser, idMonth);
        var balance = GetBalance(idUser, idMonth);
        balance.Expenses = bills.Sum(b => b.Cost).ToString();
        var remaining = Double.Parse(balance.Income) - Double.Parse(balance.Expenses);
        balance.Remaining = remaining.ToString();

        string queryString = "UPDATE Balance SET Expenses = " + balance.Expenses + ", Remaining = " +
                             balance.Remaining + " WHERE idUser = " + idUser + " AND idMonth = " + idMonth;

        try
        {
            var cn = new Connection();
            using (var connection = new SqlConnection(cn.getCadenaSQL()))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
        }
    }
}