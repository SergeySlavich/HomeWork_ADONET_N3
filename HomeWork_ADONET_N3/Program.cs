using System;
using System.Configuration;
using System.Data.SqlClient;

namespace HomeWork_ADONET_N3
{
    //Задание.
	//Есть 2 таблицы:
	//	- Клиент(id, имя, возраст)
	//	- Заказ(order) (id, описание заказа, client id) - заказ связан с клиентом

	//Задание: реализовать в виде отдельных процедур с использованием C# и ADO.NET следующие операции:
	//	1) Добавить нового клиента
	//	2) Добавить новый заказ
	//	3) Вывести список клиентов
	//	4) Вывести список заказов определенного клиента
	//	5) Вывести список имен клиентов и количество заказов у каждого из них
	//	6) Вывести количество клиентов
	//Для тестирование в отдельной процедуре создать 5 клиентов и от 2 до 5 заказов каждому из них(на C#).

	//Приветствуется использование sql-параметров, вынос строки подключения в конфиг, использование вспомогательных процедур.
    internal class Program
    {
		//Вспомогательные процедура: 1.Подключение к базе данных
		static SqlConnection OpenDbConnection()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			return connection;
		}
		//Вспомогательная процедура: 2.Вывод табличного результата
		static void TableResult(SqlDataReader reader)
		{
            bool exist = true;
            while (reader.Read())
            {
                //title
                if (exist)
                {
                    for (int i = 0; i < reader.FieldCount - 1; i++)
                    {
                        Console.Write($"{reader.GetName(i)} - ");
                    }
                    Console.WriteLine(reader.GetName(reader.FieldCount - 1));
                }
                exist = false;
                //data
                for (int j = 0; j < reader.FieldCount - 1; j++)
                {
                    Console.Write($"{reader[j]} - ");
                }
                Console.WriteLine(reader[reader.FieldCount - 1]);
            }
            if (exist) Console.WriteLine("Data none.");
        }
        //	1) Добавить нового клиента
		static void AddNewClient(string name, int age)
		{
			SqlConnection connection = null;
			try
			{
				connection = OpenDbConnection();
				SqlCommand cmd = new SqlCommand();
				cmd.Parameters.AddWithValue("parameterName", name);
				cmd.Parameters.AddWithValue("parameterAge", age);
				string sqlRequest = @"insert into Clients (name, age) values (@parameterName, @parameterAge)";
				cmd.Connection = connection;
				cmd.CommandText = sqlRequest;
				cmd.ExecuteNonQuery();
				Console.WriteLine($"Client: {name} added successful.");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error!: {e.Message}");
			}
			finally
			{
				connection?.Close();
			}
		}
        //	2) Добавить новый заказ
		static void AddNewOrder(string description, int client_id)
		{
            SqlConnection connection = null;
            try
            {
                connection = OpenDbConnection();
				SqlCommand cmd = new SqlCommand();
				cmd.Parameters.AddWithValue("parameterDescription", description);
				cmd.Parameters.AddWithValue("parameterClient_id", client_id);
				string sqlRequest = @"insert into Orders (description_f, client_id) values (@parameterDescription, @parameterClient_id)";
                cmd.Connection = connection;
                cmd.CommandText = sqlRequest;
                cmd.ExecuteNonQuery();
				Console.WriteLine($"Order: {description} added successful.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error!: {e.Message}");
            }
            finally
            {
                connection?.Close();
            }
        }
        //	3) Вывести список клиентов
		static void ClientList()
		{
			SqlConnection connection = null;
			SqlDataReader reader = null;
			try
			{
				connection = OpenDbConnection();
				SqlCommand query = new SqlCommand($"select * from Clients", connection);
				reader = query.ExecuteReader();
				TableResult(reader);
			}
			catch(Exception e)
			{
				Console.WriteLine($"Error! {e.Message}");
			}
			finally
			{
				connection?.Close();
				reader?.Close();
			}
		}
        //	4) Вывести список заказов определенного клиента
		static void OrderListById(int  id)
		{
			SqlConnection connection = null;
			SqlDataReader reader = null;
			Console.WriteLine();
			try
			{
				connection = OpenDbConnection();
				SqlCommand query = new SqlCommand($"select description_f as [List of orders of client by id: {id}] from Orders where client_id = {id}", connection);
				reader = query.ExecuteReader();
				TableResult(reader);
			}
			catch(Exception e)
			{
				Console.WriteLine($"Error!!! {e.Message}");
			}
			finally
			{
				connection?.Close();
				reader?.Close();
			}
		}
        //	5) Вывести список имен клиентов и количество заказов у каждого из них
		static void ClientsAndOrders()
		{
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = OpenDbConnection();
                SqlCommand query = new SqlCommand(
					$"select name, count(orders.id) as count_of_orders from Clients, orders where Clients.id = orders.client_id group by name",
					connection);
                reader = query.ExecuteReader();
                TableResult(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
            }
            finally
            {
                connection?.Close();
                reader?.Close();
            }
        }
		//	6) Вывести количество клиентов
		static void ClientsCount()
		{
            SqlConnection connection = null;
            try
            {
                connection = OpenDbConnection();
                SqlCommand cmd = new SqlCommand("select count(name) from Clients", connection);
                int result = (int) cmd.ExecuteScalar();
				Console.WriteLine($"Count of clients = {result}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
            }
            finally
            {
                connection?.Close();
            }
        }
        static void Main(string[] args)
        {
			//	1) Добавить нового клиента
			Console.WriteLine("\n==== Add new client ====");
			AddNewClient("Holly", 45);

			//	2) Добавить новый заказ
			Console.WriteLine("\n==== Add new order ====");
			AddNewOrder("Bluetooth game mouse", 6);

			//	3) Вывести список клиентов
			Console.WriteLine("\n==== Show list of clients ====");
			ClientList();

			//	4) Вывести список заказов определенного клиента
			Console.WriteLine("\n==== Show list of orders of one client by id ====");
			OrderListById(1);
			OrderListById(100);

			//	5) Вывести список имен клиентов и количество заказов у каждого из них
			Console.WriteLine("\n==== Show list of clients with count order for each client ====");
			ClientsAndOrders();

			//	6) Вывести количество клиентов
			Console.WriteLine("\n==== Show count of clients ====");
			ClientsCount();
        }
    }
}
