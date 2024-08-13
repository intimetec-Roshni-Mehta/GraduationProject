using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RecommendationEngine.Communication.SocketClient
{
    public class SocketMessenger
    {
        public static void StartClient()
        {
            try
            {
                var remoteEP = new IPEndPoint(IPAddress.Parse("172.20.10.14"), 1010);

                using (Socket sender = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    ConnectToServer(sender, remoteEP);
                    SendLoginData(sender);
                    StartReceivingNotifications(sender);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void ConnectToServer(Socket sender, IPEndPoint remoteEP)
        {
            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine($"Socket connected to {sender.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to server: {ex.Message}");
                throw;
            }
        }

        private static void SendLoginData(Socket sender)
        {
            try
            {
                string username = PromptUser("Enter username: ");
                string password = PromptUser("Enter password: ");
                string message = $"login;{username};{password}";

                SendMessage(sender, message);
                ReceiveResponse(sender, username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send or receive data: {ex.Message}");
            }
        }

        private static void StartReceivingNotifications(Socket sender)
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        byte[] bytes = new byte[2048];
                        int bytesRec = sender.Receive(bytes);
                        var notification = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine($"Notification: {notification}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving notification: {ex.Message}");
                }
            });
        }

        private static string PromptUser(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        private static void SendMessage(Socket sender, string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            sender.Send(msg);
        }

        private static void ReceiveResponse(Socket sender, string username)
        {
            try
            {
                byte[] bytes = new byte[2048];
                int bytesRec = sender.Receive(bytes);
                var response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                Console.WriteLine("Server response = {0}", response);

                HandleRoleOptions(sender, response, username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving response: {ex.Message}");
            }
        }

        private static void HandleRoleOptions(Socket sender, string response, string username)
        {
            switch (response)
            {
                case "Login successful; Role: Admin":
                    Console.WriteLine("Options:\n1. Add Item\n2. Update Item\n3. Delete Item\n4. View Items\n5. View Discard Menu Item List\n6. Remove Food Item from Menu\n7. Get Detailed Feedback\n");
                    ProcessAdminOptions(sender, username);
                    break;
                case "Login successful; Role: Chef":
                    Console.WriteLine("Options:\n1. Rollout Menu\n2. Get Rolled Out Menu\n3. View Voted Items\n4. Finalize Menu\n5. View Discard Menu Item List\n6. Remove Food Item from Menu\n7. Get Detailed Feedback\n");
                    ProcessChefOptions(sender, username);
                    break;
                case "Login successful; Role: Employee":
                    Console.WriteLine("Options:\n1. Give Feedback\n2. View Recommended Menu\n3. Vote For Item\n4. Get Finalized Menu\n");
                    ProcessEmployeeOptions(sender, username);
                    break;
                default:
                    Console.WriteLine("Unknown role or invalid response");
                    break;
            }
        }

        private static void ProcessAdminOptions(Socket sender, string username)
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                Console.Write("Enter option (or 'logout' to exit): ");
                var option = Console.ReadLine();

                if (option.ToLower() == "logout")
                {
                    SendMessage(sender, $"logout;{username};");
                    loggedIn = false;
                }
                else if (option == "1")
                {
                    string itemName = PromptUser("Enter item name: ");
                    string itemPrice = PromptUser("Enter item price: ");
                    string itemStatus = PromptUser("Enter item status: ");
                    string mealTypeId = PromptUser("Enter meal type ID: ");

                    SendMessage(sender, $"{option};{username};{itemName};{itemPrice};{itemStatus};{mealTypeId}");
                    ReceiveServerResponse(sender);
                }
                else if (option == "2")
                {
                    string itemId = PromptUser("Enter item id to update: ");
                    string itemPrice = PromptUser("Enter new item price: ");
                    string itemStatus = PromptUser("Enter new item status: ");

                    SendMessage(sender, $"{option};{username};{itemId};{itemPrice};{itemStatus}");
                    ReceiveServerResponse(sender);
                }
                else if (option == "3")
                {
                    string itemId = PromptUser("Enter item ID to delete: ");

                    SendMessage(sender, $"{option};{username};{itemId}");
                    ReceiveServerResponse(sender);
                }
                else if (option == "4")
                {
                    SendMessage(sender, $"{option};{username};");
                    ReceiveServerResponse(sender);
                }
                else if (option == "5")
                {
                    SendMessage(sender, $"{option};{username};");
                    ReceiveServerResponse(sender);
                }
                else if (option == "6")
                {
                    string itemId = PromptUser("Enter food item ID to remove: ");
                    SendMessage(sender, $"{option};{username};{itemId}");
                    ReceiveServerResponse(sender);
                }
                else if (option == "7")
                {
                    string itemId = PromptUser("Enter item ID to get detailed feedback: ");
                    SendMessage(sender, $"{option};{username};{itemId}");
                    ReceiveServerResponse(sender);
                }
                else
                {
                    SendMessage(sender, $"{option};{username};");
                    ReceiveServerResponse(sender);
                }
            }
        }

        private static void ProcessChefOptions(Socket sender, string username)
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                Console.Write("Enter option (or 'logout' to exit): ");
                var option = Console.ReadLine();

                if (option.ToLower() == "logout")
                {
                    SendMessage(sender, $"logout;{username};");
                    loggedIn = false;
                }
                else if (option == "1")
                {
                    SendMessage(sender, $"getItems;{username};");
                    var itemsListResponse = ReceiveServerResponse(sender);
                    Console.WriteLine(itemsListResponse);

                    Console.WriteLine("Rolling out menu for " + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    string itemIdsInput = PromptUser("Enter item IDs separated by commas: ");
                    List<int> itemIds = new List<int>();
                    while (!TryParseItemIds(itemIdsInput, out itemIds))
                    {
                        Console.WriteLine("Invalid item IDs format. Please enter integers separated by commas.");
                        itemIdsInput = PromptUser("Enter item IDs separated by commas: ");
                    }

                    string formattedDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    string formattedItemIds = string.Join(",", itemIds);

                    SendMessage(sender, $"{option};{username};{formattedDate};{formattedItemIds}");
                    var rolloutResponse = ReceiveServerResponse(sender);
                    ProcessRecommendations(rolloutResponse);
                }
                else if (option == "2")
                {
                    SendMessage(sender, $"{option};{username};");
                    var rolledOutMenuResponse = ReceiveServerResponse(sender);
                    ProcessRecommendations(rolledOutMenuResponse);
                }
                else if (option == "3")
                {
                    SendMessage(sender, $"{option};{username};");
                    var votedItemsResponse = ReceiveServerResponse(sender);
                    Console.WriteLine(votedItemsResponse);
                }
                else if (option == "4")
                {
                    string date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    string itemIdsInput = PromptUser("Enter comma-separated item IDs to finalize: ");

                    var itemIds = itemIdsInput.Split(',')
                                              .Select(id => id.Trim())
                                              .ToList();

                    if (itemIds.Any())
                    {
                        var itemIdsString = string.Join(",", itemIds);
                        SendMessage(sender, $"{option};{username};{date};{itemIdsString}");

                        var finalizeResponse = ReceiveServerResponse(sender);
                        Console.WriteLine(finalizeResponse);
                    }
                    else
                    {
                        Console.WriteLine("No item IDs provided. Please enter valid item IDs.");
                    }
                }
                else if (option == "5")
                {
                    SendMessage(sender, $"{option};{username};");
                    ReceiveServerResponse(sender);
                }
                else if (option == "6")
                {
                    string itemId = PromptUser("Enter food item ID to remove: ");
                    SendMessage(sender, $"{option};{username};{itemId}");
                    ReceiveServerResponse(sender);
                }
                else if (option == "7")
                {
                    string itemId = PromptUser("Enter item ID to get detailed feedback: ");
                    SendMessage(sender, $"{option};{username};{itemId}");
                    ReceiveServerResponse(sender);
                }
                else
                {
                    SendMessage(sender, $"{option};{username};");
                    ReceiveServerResponse(sender);
                }
            }
        }

        private static bool TryParseItemIds(string input, out List<int> itemIds)
        {
            itemIds = new List<int>();
            var parts = input.Split(',');
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int id))
                {
                    itemIds.Add(id);
                }
                else
                {
                    itemIds = null;
                    return false;
                }
            }
            return true;
        }

        private static void ProcessEmployeeOptions(Socket sender, string username)
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                Console.Write("Enter option (or 'logout' to exit): ");
                var option = Console.ReadLine();

                if (option.ToLower() == "logout")
                {
                    SendMessage(sender, $"logout;{username};");
                    loggedIn = false;
                }
                else if (option == "1")
                {
                    Console.Write("Enter additional details separated by semicolon (itemId;rating;comment or itemId): ");
                    var details = Console.ReadLine();
                    SendMessage(sender, $"{option};{username};{details}");
                }
                else if (option == "5") // Update Profile
                {
                    UpdateProfile(sender, username);
                }
                else
                {
                    SendMessage(sender, $"{option};{username};");
                }

                var serverResponse = ReceiveServerResponse(sender);
                Console.WriteLine(serverResponse);
            }
        }

        private static void UpdateProfile(Socket sender, string username)
        {
            Console.WriteLine("Please answer these questions to know your preferences:");

            string dietPreference = PromptUser("1) Please select one - Vegetarian, Non Vegetarian, Eggetarian: ");
            string spiceLevel = PromptUser("2) Please select your spice level - High, Medium, Low: ");
            string cuisinePreference = PromptUser("3) What do you prefer most? - North Indian, South Indian, Other: ");
            string sweetTooth = PromptUser("4) Do you have a sweet tooth? - Yes, No: ");

            string profileData = $"{dietPreference};{spiceLevel};{cuisinePreference};{sweetTooth}";
            SendMessage(sender, $"updateProfile;{username};{profileData}");
        }

        private static void ProcessRecommendations(string response)
        {
            var items = response.Split(';'); 
            Console.WriteLine("Recommended Items for you:");
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }


        private static string ReceiveServerResponse(Socket sender)
        {
            try
            {
                byte[] bytes = new byte[2048];
                int bytesRec = sender.Receive(bytes);
                var response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving response: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
