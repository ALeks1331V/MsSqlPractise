using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataBaseInteraction base1 = new DataBaseInteraction();
            base1.openAndCheckConnection();
            while (true)
            {
                Console.WriteLine("Choose option:\n" +
                    "1 - insert recorded data in DB\n" +
                    "2 - insert your data in DB\n" +
                    "3 - select all data from db\n" +
                    "4 - select special data from db");
                string userAnswer = Console.ReadLine();

                switch (userAnswer)
                {
                    case "1":
                        base1.insertSomeDataInDB();
                        break;
                    case "2":
                        base1.insertUserData();
                        break;
                    case "3":
                        base1.selectAllDataFromDB(); 
                        break; 
                    case "4":
                        Console.WriteLine("Do you want to set the selection conditions from columns?  y/n");
                        userAnswer = Console.ReadLine();
                        if (userAnswer == "y") 
                        {
                            Console.WriteLine("Enter the query according to SQL starting with WHERE");
                            string userQuery = Console.ReadLine();
                            base1.SelectSpecialData(userQuery);
                        }
                        else if (userAnswer == "n")
                        {
                            base1.SelectSpecialData();
                        }
                        else { Console.WriteLine("Incorrect option"); }
                        break;
                    default: Console.WriteLine("Unknown operation. Try again."); break;

                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    class DataBaseInteraction
    {
        private SqlConnection connection = null;

        public void openAndCheckConnection()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionTestDB"].ConnectionString);
            connection.Open();
            if (connection.State == ConnectionState.Open)
                Console.WriteLine("The connection to the database is established");
            else Console.WriteLine("Error: Connection to the database is NOT established");
        }

        public void insertSomeDataInDB()
        {
            SqlCommand command = new SqlCommand("INSERT INTO [BankClientInfo] (ClientName, MoneyOnBalance) VALUES ('Ivan', '100')", connection);

            if (command.ExecuteNonQuery().ToString() == "1")
                Console.WriteLine("Data was inserted correctly");
            else Console.WriteLine("Error: Data wasn't inserted");
        }

        private void userInputToFloat(string columnName, SqlCommand fnCommand)
        {
            float result = 0;
            ref float result1 = ref result;
            string input = Console.ReadLine();
            try
            {
                result = float.Parse(input);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{input}'");
            }
            fnCommand.Parameters.AddWithValue(columnName, result);
        }
        public void insertUserData()
        {
            Console.WriteLine("Enter client data: ClientName, MoneyOnBalance, " +
                "Dept, cashSavings, TermOfCooperation, loanAmount");
            SqlCommand command = new SqlCommand("INSERT INTO [BankClientInfo] (ClientName, MoneyOnBalance, " +
                "Dept, cashSavings, TermOfCooperation, loanAmount) VALUES (@ClientName, @MoneyOnBalance, " +
                "@Dept, @cashSavings, @TermOfCooperation, @loanAmount)", connection);

            command.Parameters.AddWithValue("ClientName", Console.ReadLine());
            userInputToFloat("MoneyOnBalance", command);
            userInputToFloat("Dept", command);
            userInputToFloat("cashSavings", command);
            userInputToFloat("TermOfCooperation", command);
            userInputToFloat("loanAmount", command);

            if (command.ExecuteNonQuery().ToString() == "1")
                Console.WriteLine("Данные успешно внесены");
            else Console.WriteLine("Data error");
        }

        public void selectAllDataFromDB()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM BankClientInfo", connection);
            SqlDataReader reader = command.ExecuteReader();
            WriteDbInfoInConsole(reader, 6, 8);
            reader.Close();
        }

        public void SelectSpecialData()
        {
            Console.WriteLine("enter column names using Enter. As soon as you finish typing, write \"OK\"");
            string userStream = "";
            string userInput = "";
            int numberOfColumns = 0;
            while (true)
            {
                userInput = Console.ReadLine();
                if (userInput == "OK")
                {
                    userStream = userStream.Remove(userStream.Length-2);
                    break;
                }
                userStream += userInput + ", ";
                numberOfColumns += 1;
            }
            try
            {
                SqlCommand command = new SqlCommand($"SELECT {userStream} FROM BankClientInfo", connection);
                SqlDataReader reader = command.ExecuteReader();
                WriteDbInfoInConsole(reader, numberOfColumns, (14 + numberOfColumns * 2));
                reader.Close();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                Console.WriteLine("Error. Incorrect input");
            }
            
        }

        public void SelectSpecialData(string request)
        {
            Console.WriteLine("enter column names using Enter. As soon as you finish typing, write \"OK\"");
            string userStream = "";
            int numberOfColumns = 0;
            while (true)
            {
                string userInput = Console.ReadLine();
                if (userInput == "OK")
                {
                    userStream = userStream.Remove(userStream.Length - 2);
                    break;
                }
                userStream += userInput + ", ";
                numberOfColumns += 1;
            }
            try
            {
                SqlCommand command = new SqlCommand($"SELECT {userStream} FROM BankClientInfo {request}", connection);
                SqlDataReader reader = command.ExecuteReader();
                WriteDbInfoInConsole(reader, numberOfColumns, (14 + numberOfColumns * 2));
                reader.Close();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                Console.WriteLine("Error. Incorrect input");
            }
        }

        private void WriteDbInfoInConsole(SqlDataReader reader, int numberOfColumns, int yCursorPosition)
        {
            if (reader.HasRows)
            {
                int xCursorPosition = 0;
                for (int i = 0; i < numberOfColumns; i++)
                {
                    string columnName = reader.GetName(i);
                    Console.SetCursorPosition(xCursorPosition, yCursorPosition);
                    Console.WriteLine(columnName);

                    int wordLength = columnName.Length;
                    xCursorPosition += numberOfColumns + wordLength + 3;
                }
                xCursorPosition = 0;
                yCursorPosition += 1;
                while (reader.Read())
                {
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        object obj = reader.GetValue(i);
                        Console.SetCursorPosition(xCursorPosition, yCursorPosition);
                        Console.WriteLine(obj);

                        int wordLength = (reader.GetName(i)).Length;
                        xCursorPosition += numberOfColumns + wordLength + 3;
                    }
                    xCursorPosition = 0;
                    yCursorPosition += 1;
                }
            }
        }
    }
}

    
