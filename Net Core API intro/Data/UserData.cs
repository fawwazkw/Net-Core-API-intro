using System.Data.SqlClient;
using Net_Core_API_intro.Models;
using Task = Net_Core_API_intro.Models.Task;

namespace Net_Core_API_intro.Data
{
    public class UserData
    {
        private readonly string ConnectionString = "Data Source=DESKTOP-P12BD8U\\SQLEXPRESS;Database=Intro;Integrated Security=True";

        public bool InsertData(Task task, User user)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand command1 = new SqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText = "INSERT INTO Users VALUES (@name)";

                    command1.Parameters.AddWithValue("@name", user.name);

                    var result1 = command1.ExecuteNonQuery();


                    SqlCommand ident = new SqlCommand();
                    ident.Connection = connection;
                    ident.Transaction = transaction;
                    ident.Parameters.Clear();

                    ident.CommandText = "SELECT IDENT_CURRENT ('Users')";
                    int fkid = Convert.ToInt32(ident.ExecuteScalar());

                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.Parameters.Clear();

                    command2.CommandText = "INSERT INTO Tasks VALUES (@taskDetail, @fkUserID)";

                    command2.Parameters.AddWithValue("@taskDetail", task.task_detail);
                    command2.Parameters.AddWithValue("@fkUserID", fkid.ToString());

                    var goIdent = ident.ExecuteNonQuery();
                    var result2 = command2.ExecuteNonQuery();

                    transaction.Commit();

                    result = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            string query = "SELECT * from Users";

            using SqlConnection connection = new SqlConnection(ConnectionString);
            {
                using SqlCommand command = new SqlCommand(query, connection);
                {
                    try
                    {
                        connection.Open();

                        using SqlDataReader reader = command.ExecuteReader();
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    name = reader["name"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return users;
        }

        public User? GetByName(string Name)
        {
            User? user = null;

            string query = "SELECT * FROM Users WHERE name = @input";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new())
                {
                    try
                    {
                        command.Connection = connection;
                        command.CommandText = query;

                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@input", Name);
                        connection.Open();

                        using SqlDataReader reader = command.ExecuteReader();
                        {
                            while (reader.Read())
                            {
                                user = new User
                                {
                                    name = reader["name"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
            return user;
        }
    }
}
