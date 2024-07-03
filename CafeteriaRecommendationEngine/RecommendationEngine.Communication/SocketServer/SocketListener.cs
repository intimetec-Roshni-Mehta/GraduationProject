using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ConsoleTableExt;
using Microsoft.Extensions.DependencyInjection;
using RecomendationEngine.Services.Implementation;
using RecomendationEngine.Services.Interfaces;
using RecommendationEngine.DAL.Repositories.Implementation;
using RecommendationEngine.DataModel.Models;

namespace RecommendationEngine.Communication.SocketServer
{
    public class SocketListener
    {
        private static ConcurrentDictionary<string, (Socket, User)> activeSessions = new ConcurrentDictionary<string, (Socket, User)>();
        private static IAuthService authService;
        private static IAdminService adminService;
        private static IChefService chefService;
        private static IEmployeeService employeeService;

        public static async Task StartServer(IServiceProvider services)
        {
            var ipAddress = IPAddress.Parse("172.16.2.4");
            var localEndPoint = new IPEndPoint(ipAddress, 1234);

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    var handler = await listener.AcceptAsync();

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var buffer = new byte[1024];
                            while (true)
                            {
                                var bytesReceived = await handler.ReceiveAsync(buffer, SocketFlags.None);
                                if (bytesReceived == 0)
                                    break;

                                var data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                                using (var scope = services.CreateScope())
                                {
                                    InitializeServices(scope.ServiceProvider);

                                    var response = await ProcessData(data, handler);
                                    var msg = Encoding.ASCII.GetBytes(response);
                                    await handler.SendAsync(msg, SocketFlags.None);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing request: {ex.Message}");
                        }
                        finally
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server error: {e}");
            }
        }

        private static void InitializeServices(IServiceProvider serviceProvider)
        {
            authService = serviceProvider.GetRequiredService<IAuthService>();
            adminService = serviceProvider.GetRequiredService<IAdminService>();
            chefService = serviceProvider.GetRequiredService<IChefService>();
            employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
        }

        private static async Task<string> ProcessData(string data, Socket handler)
        {
            try
            {
                Console.WriteLine($"Received data: {data}");
                var parts = data.Split(';');
                var command = parts[0];
                var username = parts.Length > 1 ? parts[1] : string.Empty;
                var password = parts.Length > 2 ? parts[2] : string.Empty;

                if (command == "login")
                {
                    if (activeSessions.ContainsKey(username))
                    {
                        return "Already logged in";
                    }

                    var user = await authService.Authenticate(username, password);
                    if (user == null)
                    {
                        return "Login failed";
                    }

                    activeSessions.TryAdd(username, (handler, user));

                    var response = $"Login successful; Role: {user.Role.RoleName}";
                    return response;
                }

                if (command == "logout")
                {
                    if (activeSessions.TryRemove(username, out _))
                    {
                        return "Logout successful";
                    }
                    else
                    {
                        return "Logout failed: User not logged in";
                    }
                }

                if (activeSessions.ContainsKey(username))
                {
                    var session = activeSessions[username];
                    var user = session.Item2;

                    Console.WriteLine($"Processing command '{command}' for user '{username}' with role '{user.Role.RoleName}'");

                    switch (user.Role.RoleName.ToLower())
                    {
                        case "admin":
                            return await HandleAdminCommands(command, parts);
                        case "chef":
                            return await HandleChefCommands(command, parts);
                        case "employee":
                            return await HandleEmployeeCommands(command, parts);
                        default:
                            return "Unknown role";
                    }
                }

                return "Unknown command";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing data: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        private static async Task<string> HandleAdminCommands(string command, string[] parts)
        {
            switch (command)
            {
                case "1": // Add Item
                    if (parts.Length < 6)
                    {
                        return "Invalid command format for Add Item. Expected: 1;username;itemName;itemPrice;itemStatus;mealTypeId";
                    }

                    string itemName = parts[2];
                    if (!decimal.TryParse(parts[3], out decimal itemPrice))
                    {
                        return "Invalid item price";
                    }
                    string itemStatus = parts[4];
                    if (!int.TryParse(parts[5], out int mealTypeId))
                    {
                        return "Invalid mealType ID";
                    }

                    await adminService.AddItem(itemName, itemPrice, itemStatus, mealTypeId);
                    return "Add Item selected";

                case "2": // Update Item
                    if (parts.Length < 5)
                    {
                        return "Invalid command format for Update Item. Expected: 2;itemId;itemPrice;itemStatus";
                    }

                    if (!int.TryParse(parts[2], out int updateItemId))
                    {
                        return "Invalid item ID";
                    }
                    if (!decimal.TryParse(parts[3], out decimal updateItemPrice))
                    {
                        return "Invalid item price";
                    }
                    string updateItemStatus = parts[4];

                    await adminService.UpdateItem(updateItemId, updateItemPrice, updateItemStatus);
                    return "Update Item selected";

                case "3": // Delete Item
                    if (parts.Length < 2)
                    {
                        return "Invalid command format for Delete Item. Expected: 3;itemId";
                    }

                    if (!int.TryParse(parts[2], out int deleteItemId))
                    {
                        return "Invalid item ID";
                    }

                    await adminService.DeleteItem(deleteItemId);
                    return "Delete Item selected";

                case "4": // View Items
                    var items = await adminService.GetItems();
                    if (items == null || items.Count == 0)
                    {
                        return "No items found";
                    }

                    var tableData = items.Select(item => new
                    {
                        ID = item.ItemId,
                        Name = item.ItemName,
                        Price = item.Price,
                        Status = item.AvailabilityStatus,
                        MealTypeID = item.MealTypeId
                    }).ToList();

                    var tableString = ConsoleTableBuilder
                        .From(tableData)
                        .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                        .Export()
                        .ToString();

                    return tableString;

                default:
                    return "Invalid command for admin.";
            }
        }


        private static async Task<string> HandleChefCommands(string command, string[] parts)
        {
            switch (command)
            {
                case "getItems":
                    var itemsList = await GetItemsList();
                    return itemsList;

                case "1": // Rollout Menu
                    if (parts.Length < 4)
                    {
                        return "Invalid command format for Rollout Menu. Expected: 1;username;date;itemId1,itemId2,...";
                    }

                    var date = parts[2];
                    var itemIds = parts[3].Split(',').Select(int.Parse).ToList();
                    var rolloutResult = await chefService.RolloutMenu(date, itemIds);
                    return rolloutResult;

                case "2": // Get Rolled Out Menu
                    var rolledOutItems = await chefService.GetRolledOutMenu(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    if (rolledOutItems == null || rolledOutItems.Count == 0)
                    {
                        return "No menu items found for tomorrow";
                    }

                    var itemDetails = rolledOutItems.Select(item =>
                    {
                        var mealTypeName = item.MealType?.MealTypeName ?? "Unknown";
                        return $"ID: {item.ItemId}, Name: {item.ItemName}, Price: {item.Price}, Status: {item.AvailabilityStatus}, MealType: {mealTypeName}";
                    }).Aggregate((current, next) => current + "\n" + next);

                    return $"Rolled out menu for tomorrow:\n{itemDetails}";

                default:
                    return "Unknown chef command";
            }
        }

        private static async Task<string> HandleEmployeeCommands(string command, string[] parts)
        {
            switch (command)
            {
                case "1": // Give Feedback
                    if (parts.Length < 5)
                    {
                        return "Invalid command format for Feedback. Expected: 1;username;itemId;rating;comment";
                    }

                    var username = parts[1];
                    var itemId = int.Parse(parts[2]);
                    var rating = int.Parse(parts[3]);
                    var comment = parts[4];
                    var userId = await authService.GetUserIdByUsername(username);
                    if (userId == null)
                    {
                        return "User not found";
                    }
                    return await employeeService.GiveFeedback(userId.Value, itemId, rating, comment);

                case "2": // View Menu
                    var rolledOutItems = await chefService.GetRolledOutMenu(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    if (rolledOutItems.Count == 0)
                    {
                        return "No menu items found for tomorrow";
                    }

                    var itemDetails = rolledOutItems.Select(item =>
                    {
                        var mealTypeName = item.MealType?.MealTypeName ?? "Unknown";
                        return $"ID: {item.ItemId}, Name: {item.ItemName}, Price: {item.Price}, Status: {item.AvailabilityStatus}, MealType: {mealTypeName}, Votes: {item.Recommendations?.FirstOrDefault()?.Voting ?? 0}";
                    }).Aggregate((current, next) => current + "\n" + next);

                    return $"Rolled out menu for tomorrow:\n{itemDetails}";

                case "3": // Vote for Item
                    if (parts.Length < 3)
                    {
                        return "Invalid command format for Vote. Expected: 3;username;itemId";
                    }

                    var voteUsername = parts[1];
                    var voteItemId = int.Parse(parts[2]);

                    // Check if the item is in the rolled-out menu
                    var rolledOutItemsForVote = await chefService.GetRolledOutMenu(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    var itemToVote = rolledOutItemsForVote.FirstOrDefault(item => item.ItemId == voteItemId);
                    if (itemToVote == null)
                    {
                        return "Item not found in the rolled-out menu";
                    }

                    var voteUserId = await authService.GetUserIdByUsername(voteUsername);
                    if (voteUserId == null)
                    {
                        return "User not found";
                    }
                    return await employeeService.VoteForItem(voteUserId.Value, voteItemId);

                default:
                    return "Unknown employee command";
            }
        }


        private static async Task<string> GetItemsList()
        {
            var items = await adminService.GetItems();
            if (items == null || items.Count == 0)
            {
                return "No items found";
            }

            var filteredItems = items.ToList();

            var tableData = filteredItems.Select(item => new
            {
                ID = item.ItemId,
                Name = item.ItemName,
                Price = item.Price,
                Status = item.AvailabilityStatus,
                MealTypeID = item.MealTypeId
            }).ToList();

            var tableString = ConsoleTableBuilder
                .From(tableData)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .Export()
                .ToString();

            return tableString;
        }
    }
}