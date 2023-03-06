using System;
using System.Data.SqlClient;
using System.Data;
using Finanzas.Models;

namespace Finanzas.Data
{
    public class UsersData
    {
        public Tuple<bool, string, Users> LoginUser(Users oUser)
        {
            if (!VerifyEmail(oUser.Email))
            {
                return new Tuple<bool, string, Users>(false, "El correo no se encuentra registrado", new Users());
            }

            string queryString = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";

            var User = new Users();

            try
            {
                var cn = new Connection();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    connection.Open();
                    var command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@Email", oUser.Email);
                    command.Parameters.AddWithValue("@Password", oUser.Password);


                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oUser.Email = dr["Email"].ToString();
                            oUser.Name = dr["Name"].ToString();
                            oUser.idUser = Int32.Parse(dr["idUser"].ToString());
                            
                            return Tuple.Create(true, "Usuario logueado correctamente", oUser);
                            
                        }
                    }
                    
                    
                    

                    return new Tuple<bool, string, Users>(false, "Usuario o contraseña incorrectos", new Users());
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string, Users>(false, ex.Message, new Users());
            }
        }

        public Tuple<bool, string> RegisterUser(Users oUser)
        {
            bool result = VerifyEmail(oUser.Email);

            if (!result)
            {
                string queryString = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";

                try
                {
                    var cn = new Connection();

                    using (var connection = new SqlConnection(cn.getCadenaSQL()))
                    {
                        connection.Open();

                        var command = new SqlCommand(queryString, connection);

                        command.Parameters.AddWithValue("@Name", oUser.Name);
                        command.Parameters.AddWithValue("@Email", oUser.Email);
                        command.Parameters.AddWithValue("@Password", oUser.Password);
                        command.ExecuteNonQuery();
                        connection.Close();
                        return new Tuple<bool, string>(true, "Usuario registrado correctamente");
                    }
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string>(false, ex.Message);
                }
            }

            return new Tuple<bool, string>(false, "El correo ya se encuentra registrado");
        }

        public bool VerifyEmail(string email)
        {
            string queryString = "SELECT * FROM Users WHERE Email = @Email";
            try
            {
                var cn = new Connection();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    connection.Open();
                    var command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}