using System;
using NotificationApp.Entities;
using NotificationApp.Interfaces;
using NotificationApp.Services;

namespace NotificationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserRepository userRepository = new DummyUserRepository();
            INotifier notifier = new ConsoleNotifier();

            var notificationService = new NotificationService(userRepository, notifier);

            Console.WriteLine("Enter user ID:");
            int userId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter message to send:");
            string message = Console.ReadLine();

            try
            {
                notificationService.NotifyUser(userId, message);
                Console.WriteLine("Notification sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class DummyUserRepository : IUserRepository
    {
        public User GetUserById(int id)
        {
            return new User
            {
                Id = id,
                Email = "user" + id + "@example.com",
                IsActive = (id % 2 == 0) 
            };
        }
    }

    public class ConsoleNotifier : INotifier
    {
        public void Send(string email, string message)
        {
            Console.WriteLine($"Sending message to {email}: {message}");
        }
    }
}
