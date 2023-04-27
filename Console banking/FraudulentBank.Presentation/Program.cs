using FraudulentBank.Customer;
using System.Transactions;
using System.Xml.Linq;

//static void Main(string[] args)
{
   // while (true)
    {
        Console.WriteLine("**********WELCOME TO FRAUDULENT BANK*********");
        Console.WriteLine("1. Sign up");
        Console.WriteLine("2. Log in");
        Console.WriteLine("3. Exit");
        string choice = Console.ReadLine();
        

        switch (choice)
        {
            case "1":
                Customer.SignUp();
                break;
            case "2":
                Customer.Login();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}

    





