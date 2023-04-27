using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace FraudulentBank.Customer
{


    public class Transaction
    {
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public Transaction(string accountNumber, string transactionType, decimal amount, DateTime date)
        {
            AccountNumber = accountNumber;
            TransactionType = transactionType;
            Amount = amount;
            Date = date;
        }
    }


    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Password { get; set; }
        public List<Transaction>? Transactions { get; set; }

        public Customer(string name, string accountNumber,  string hashedPassword, string email, decimal balance)
        {
            Name = name;
            Email = email;
            Password = hashedPassword;
            AccountNumber = accountNumber;
            Balance = balance;
            Transactions = new List<Transaction>();
        }


        public static void SignUp()
        {
            Console.Write("Kindly Enter Your Name: ");
            string name = Console.ReadLine();


            Console.Write("Kindly Enter Your Email: ");
            string email = Console.ReadLine();

            Console.Write("Kindly Enter Your Password: ");
            string password = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.KeyChar != '\u0000' && !char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            string accountNumber = GenerateAccountNumber();

            decimal balance = 0;

            List<Transaction> transactions = new List<Transaction>();
            
            string hashedPassword = HashPassword(password);

            var customer = new Customer(name, hashedPassword, email, accountNumber, balance);

            
            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{customer.AccountNumber}.txt";
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"Name: {customer.Name}");              
                writer.WriteLine($"Email: {customer.Email}");
                writer.WriteLine($"Account Number: {customer.AccountNumber}");
                writer.WriteLine($"Password: {customer.Password}");
                writer.WriteLine($"Balance: {customer.Balance}");
                writer.WriteLine("Transaction History:");
            }           
            Console.WriteLine($"Congratulations {name}! Your account has been created with account number {accountNumber}.");
            Console.WriteLine("Press 1 to login, or any other key to exit.");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Login();
            }
            else
            {
                Console.WriteLine("Goodbye! Thanks for using our banking application.");
                Environment.Exit(0);

            }
        }



        public static void SaveCustomersToFile(List<Customer> Customers)
        {
            string fileName = "Customers.txt";
            string filePath = @"C:\Users\Oluwafemi.Dally\Documents\" + fileName;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Customer customer in Customers)
                {
                    writer.WriteLine($"{customer.Name},{customer.AccountNumber},{customer.Password},{customer.Email},{customer.Balance}");
                }
            }
        }


        static string GenerateAccountNumber()
        {
            List<Customer> Customers = new List<Customer>();
            Random random = new Random();
            string accountNumber;
            do
            {
                accountNumber = "20" + random.Next(10000000, 100000000).ToString();
            } while (Customers.Any(c => c.AccountNumber == accountNumber));

            return accountNumber;
        }

        static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }

        public static void Login()
        {
            Console.WriteLine("Enter your account number: ");
            string accountNumber = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string password = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.KeyChar != '\u0000' && !char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            string hashedPassword = Customer.HashPassword(password);

            // Load customer data from file with account number as file name
            string fileName = $"{accountNumber}.txt";
            Customer c = Customer.LoadCustomerFromFile(accountNumber);

            if (c != null && c.Password == hashedPassword)
            {
                Console.WriteLine("Login Successful.");
                bool isLoggedIn = true;

                while (isLoggedIn)
                {
                    Console.WriteLine("\nPlease select an option:");
                    Console.WriteLine("1. Deposit");
                    Console.WriteLine("2. Withdraw");
                    Console.WriteLine("3. View Balance");
                    Console.WriteLine("4. Transaction History");
                    Console.WriteLine("5. Exit");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Deposit(accountNumber);
                            break;

                        case "2":
                            Withdraw(accountNumber);
                            break;

                        case "3":
                            ViewBalance();
                            break;

                        case "4":
                            DisplayTransactionHistory();
                            break;

                        case "5":
                            isLoggedIn = false;
                            Console.WriteLine("Goodbye! Thanks for using our banking application.");
                            Environment.Exit(5);
                            break;

                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }

                }
            }
            else
            {
                Console.WriteLine("Invalid account number or password.");
               
            }
        }

        public static bool AuthenticateCustomer(string accountNumber, string hashedPassword)
        {
            var Customers = new List<Customer>();
            foreach (Customer customer in Customers)
            {
                if (customer.AccountNumber == accountNumber && customer.Password == hashedPassword)
                {
                    return true;
                }
            }

            return false;
        }

        //public static List<Customer> LoadCustomersFromFile()
        //{
        //    List<Customer> customers = new List<Customer>();

        //    try
        //    {
        //        string fileName = "Customers.txt";
        //        string filePath = @"C:\Users\Oluwafemi.Dally\Documents\" + fileName;
        //        if (File.Exists(filePath))
        //        {
        //            string[] lines = File.ReadAllLines(filePath);
        //            foreach (string line in lines)
        //            {
        //                string[] fields = line.Split(',');
        //                string name = fields[0];
        //                string hashedPassword = fields[1];
        //                string email = fields[2];
        //                string accountNumber = fields[3];
        //                decimal balance = decimal.Parse(fields[4]);

        //                Customer customer = new Customer(name, hashedPassword, email, accountNumber, balance);
        //                customers.Add(customer);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred while loading customers from file: {ex.Message}");
        //    }

        //    return customers;
        //}

        public static Customer LoadCustomerFromFile(string accountNumber)
        {
            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}.txt";

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length < 5)
            {
                throw new FormatException("File has incorrect format.");
            }

            string name = lines[0].Substring(6);
            string loadedAccountNumber = lines[2].Substring(16);
            string hashedPassword = lines[3].Substring(10);
            string email = lines[1].Substring(7);
            decimal balance = decimal.Parse(lines[4].Substring(9));

            Customer customer = new Customer(name, accountNumber, hashedPassword, email, balance);

            return customer;
        }






        public static void Deposit(string accountNumber)
        {
            Console.Write("Enter the deposit amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());


            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}.txt";
            Customer c = Customer.LoadCustomerFromFile(accountNumber);

            if (c.AccountNumber != accountNumber)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            // Create a new transaction object and add it to the customer's transactions list
            var transaction = new Transaction(accountNumber, "Deposit", amount, DateTime.Now);
            

            // Update the customer's balance
            c.Balance += amount;
           

            // Append transaction to transaction history file
            string transactionFileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}_transaction.txt";
            using (StreamWriter writer = File.AppendText(transactionFileName))
            {
                writer.WriteLine($"{transaction.AccountNumber}|{transaction.TransactionType}|{transaction.Amount}|{transaction.Date}");
            }

            Console.WriteLine($"Transaction successful. Your new balance is {c.Balance:C}");
        }




        public static void SaveTransactionToFile(string accountNumber, Transaction transaction)
        {
            string fileName = "Transaction.txt";
            string filePath = @"C:\Users\Oluwafemi.Dally\Documents\" + fileName;

            // Write the transaction data to the file
            using (StreamWriter writer = File.AppendText(filePath))
            {
                string transactionData = $"{accountNumber},{transaction.TransactionType},{transaction.Amount},{transaction.Date}";
                writer.WriteLine(transactionData);
            }
        }

        public static List<Transaction> LoadTransactionHistoryFromFile(string accountNumber)
        {
            string fileName = $@"C:\Users\Oluwafemi.Dally\Documents\{accountNumber}_transaction.txt";

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            List<Transaction> transactions = new List<Transaction>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime date = DateTime.ParseExact(fields[3], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    string TransactionType = fields[1];
                    decimal amount = decimal.Parse(fields[2]);
                    Transaction transaction = new Transaction(accountNumber, TransactionType, amount, date);
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }




        public static void Withdraw(string accountNumber)
        {
            Console.Write("Enter the withdrawal amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            List<Customer> Customers = new List<Customer>();

            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}.txt";
            Customer c = Customer.LoadCustomerFromFile(accountNumber);

            if (c.AccountNumber != accountNumber)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (amount > c.Balance)
            {
                Console.WriteLine("Insufficient funds.");
                return;
            }

            var transaction = new Transaction(accountNumber, "Withdrawal", amount, DateTime.Now);
            
            // Update the customer's balance
            c.Balance -= amount;

            // Append transaction to transaction history file
            string transactionFileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}_transaction.txt";
            using (StreamWriter writer = File.AppendText(transactionFileName))
            {
                writer.WriteLine($"{transaction.AccountNumber}|{transaction.TransactionType}|{transaction.Amount}|{transaction.Date}");
            }

            Console.WriteLine($"Transaction successful. Your new balance is {c.Balance:C}");
        }


        public static void DisplayTransactionHistory()
        {
            Console.WriteLine("Enter your account number:");
            string accountNumber = Console.ReadLine();

            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}.txt";
            string transactionFileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}_transaction.txt";

            Customer c = Customer.LoadCustomerFromFile(accountNumber);

            if (c.AccountNumber != accountNumber)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            else
            {
                // Display the customer's transaction history
                Console.WriteLine($"Transaction history for {c.Name} ({c.AccountNumber}):");
                List<Transaction> transactions = LoadTransactionHistoryFromFile(accountNumber);
                foreach (Transaction transaction in transactions)
                {
                    Console.WriteLine($"Date: {transaction.Date}\tType: {transaction.TransactionType}\tAmount: {transaction.Amount:C}");
                }
            }
        }


        public static void ViewBalance()
        {
            Console.Write("Account number: ");
            string accountNumber = Console.ReadLine();

            string fileName = @"C:\Users\Oluwafemi.Dally\Documents\" + $"{accountNumber}.txt";
            Customer c = Customer.LoadCustomerFromFile(accountNumber);

            if (c.AccountNumber != accountNumber)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }
            else
            {
                Console.WriteLine($"Balance: NGN{c.Balance:C}");
            }


        }

    }
}

