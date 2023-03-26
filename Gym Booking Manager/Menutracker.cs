using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gym_Booking_Manager
{
    internal class Menutracker : User
    {
        //MAIN MENUS
        public static void menu()
        {
            Console.WriteLine(logedInUser);
            Console.WriteLine("__________________________________");
            List<string> menuOptions = new List<string>()
                {
                    "View Logs (TBD)",
                    "Purchase Membership",
                    "Manage Account (NYI)",
                    "Purchase Day Pass",
                    "Cancel Reservation",
                    "View group Schedule",
                    "Manage group Schedule",
                    "Make Reservation",
                    "View Item",
                    "Restrict item",
                    "Add item(NYI)",
                    "Log out"
                };

            if (logedInUser != null && logedInUser.accessLevels == AccessLevels.PayingMember)
            {
                menuOptions.Remove("View Logs (TBD)");
                menuOptions.Remove("Purchase Membership");
                menuOptions.Remove("Purchase Day Pass");
                menuOptions.Remove("Manage group Schedule");
                menuOptions.Remove("Restrict item");
                menuOptions.Remove("Add item(NYI)");
            }
            else if (logedInUser != null && logedInUser.accessLevels == AccessLevels.Member)
            {
                menuOptions.Remove("View Logs (TBD)");
                menuOptions.Remove("Manage group Schedule");
                menuOptions.Remove("Add item");
                menuOptions.Remove("Restrict item");
                menuOptions.Remove("Make Reservation");
                menuOptions.Remove("View Item");
                menuOptions.Remove("Group Activity");
            }

            else if (logedInUser != null && logedInUser.accessLevels == AccessLevels.DayPassUser)
            {
                menuOptions.Remove("View Logs (TBD)");
                menuOptions.Remove("Purchase Day Pass");
                menuOptions.Remove("Add item(NYI)");
                menuOptions.Remove("Restrict item");
                menuOptions.Remove("Manage group Schedule");
            }

            else if (logedInUser != null && logedInUser.accessLevels == AccessLevels.Staff)
            {
                menuOptions.Remove("View Logs (TBD)");
            }

            else if (logedInUser != null && logedInUser.accessLevels == AccessLevels.NonPayingNonMember)
            {
                menuOptions.Remove("View Logs (TBD)");
                menuOptions.Remove("Manage Account (NYI)");
                menuOptions.Remove("Manage group Schedule");
                menuOptions.Remove("Restrict item");
                menuOptions.Remove("Add item(NYI)");
            }

            for (int i = 0; i < menuOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menuOptions[i]}");
            }
        }
        public static void MainMenu(int db)
        {
            Console.WriteLine("-------------Main Menu:-------------");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create account");
            Console.WriteLine("3. Non member access");
            Console.WriteLine("4. Quit");
            Console.WriteLine("------------------------------------\n");

			string input = Console.ReadLine().Trim();
			if (int.TryParse(input, out int command))
            {
                switch (command)
                {
                    case 1:
                        Console.Clear();
                        LoginMenu();
                        break;
                    case 2:
                        // Create New Account
                        Console.Clear();
                        Console.WriteLine("------CREATE A NEW ACCOUNT------");
                        Console.WriteLine("Enter the name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter the phone number: ");
                        string phone = Console.ReadLine();
                        Console.WriteLine("Enter the email: ");
                        string email = Console.ReadLine();
                        Customer customer = Customer.customerList.Find(c => c.name == name && c.phone == phone && c.email == email);
                        if (customer == null)
                        {
                            customer = new Customer(name, phone, email, AccessLevels.Member);                           
                        }
                        
                        Console.WriteLine("Member details:");                        
                        Console.WriteLine(customer.ToString());
                        Console.WriteLine("Press Enter to continue.");
                        Console.ReadLine();
                        User.logedInUser = customer;

                        Console.Clear();
                        UserMenu();
                        break;
                                            
                    case 3:						
						logedInUser = new Customer("No member", "No Phone", "No Email", AccessLevels.NonPayingNonMember);
                        Console.Clear();
						UserMenu();	
						break;
					case 4:
                        Console.WriteLine("\nExiting program...");
                        if (db == 1)
                        {
                            CsvHandler csvHandler = new CsvHandler();
                            csvHandler.WriteFile(Space.spaceList, "Spaces.txt");
                            csvHandler.WriteFile(Equipment.equipmentList, "Equipment.txt");
                            csvHandler.WriteFile(PersonalTrainer.personalTrainers, "PersonalTrainer.txt");
                            csvHandler.WriteFile(GroupSchedule.groupScheduleList, "GroupActivity.txt");
							Environment.Exit(0);
						}
                        else if (db == 2)
                        {
                            Database database = new Database();
                            database.WriteData("Equipment");
							database.WriteData("Spaces");
							database.WriteData("PersonalTrainer");
							database.WriteData("GroupActivity");
							Environment.Exit(0);
                        }

                        break;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }

            }
            else
            {
				Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear();
			}
        }
        public static void LoginMenu()
        {
            //As the loginsystem is not complete we have to set an given user on what login-type you are using.

            //These all does get added everytime you log in, not great
            //This can be changed betewen PayingMember/DayPassUser
            Customer testCustomer = new Customer("Test Customer", "0987321", "testCustomer@test.se", AccessLevels.PayingMember) { uniqueID = 10 };

            User testStaff = new Staff("Test Staff 1", "1234", "test1@gmail.com", AccessLevels.Staff) { uniqueID = 20 };
            User testAdmin = new Admin("Test Admin", "098873", "testAdmin@gmail.com", AccessLevels.Admin);
            User testService = new Service("Test Service", "0899991", "testService@gomail.com", AccessLevels.Service);

            // Test for the customerlist
            Customer testCustomer1 = new Customer("Test Customer1", "012-123-434", "Testar@test.se", AccessLevels.NonPayingNonMember);
            Customer testCustomer2 = new Customer("Test Customer2", "321-452-123", "Testar2@test.com", AccessLevels.NonPayingNonMember);

            Console.WriteLine("-------------Member Access Menu:-------------");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Staff");
            Console.WriteLine("3. Service");
            Console.WriteLine("4. Member");
            Console.WriteLine("5. Go Back");
            Console.WriteLine("---------------------------------------------\n");
            try
            {
                int command = int.Parse(Console.ReadLine());
                switch (command)
                {
                    case 1:
                        Console.Clear();
                        User.logedInUser = testAdmin;
                        AdminMenu();
                        break;
                    case 2:
                        Console.Clear();
                        User.logedInUser = testStaff;
                        StaffMenu();
                        break;
                    case 3:
                        Console.Clear();
                        User.logedInUser = testService;
                        ServiceMenu();
                        break;
                    case 4:
                        Console.Clear();
                        User.logedInUser = testCustomer;
                        if (logedInUser != null && logedInUser.accessLevels == AccessLevels.DayPassUser)
                        {
                            DayPassMenu();
                        }
                        else if (logedInUser != null && logedInUser.accessLevels == AccessLevels.PayingMember)
                        {
                            PayingMemberMenu();
                        }
                        if (logedInUser != null && logedInUser.accessLevels == AccessLevels.Member)
                        {
                            UserMenu();
                        }
                        break;
                    case 5:
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
        }

        //ADMIN
        public static void AdminMenu()
        {
            while (true)
            {
                //Console.Clear();
                Console.WriteLine("--------------Admin-------------");
                menu();
                Console.WriteLine("---------------------------------\n");
                int command;
                try
                {
                    command = int.Parse(Console.ReadLine());
                    switch (command)
                    {
                        case 1:
                            Console.Clear();
                            // View logs 
                            //ListAllLogs();
                            break;
                        case 2:
						    // Purchase membership
						    PurchaseMembershipForCustomer();
						    break;
                        case 3:
                            Console.Clear();
                            // Manage Account
                            //Console.WriteLine("Enter the log index to view: ");
                            //int logIndex = int.Parse(Console.ReadLine());
                            //ViewLog(logIndex);
                            break;
                        case 4:
                            Console.Clear();
						    // Purchase daypass
						    PurchaseDayPassForCustomer();
						    break;
                        case 5:
                            Console.Clear();
						    // Cancel reservation
						    CancelReservationForCustomer();
						    break;
                        case 6:
                            Console.Clear();
                            GroupSchedule.showActivities();
                            Console.WriteLine("Press Enter to continue!");
                            Console.ReadLine();
                            Console.Clear();                            
                            break;
                        case 7:
                            Console.Clear();
                            manageSchedule();
                            break;
                        case 8:
                            Console.Clear();
						    // Make reservation
						    MakeReservationForCustomer();
						    break;
                        case 9:
                            Console.Clear();
                            // View items
                            ViewItems();
                            break;
                        case 10:
                            Console.Clear();
                            RestrictItem();
                            break;
                        case 11:
                            Console.Clear();
                            // TODO: Add item
                            break;
                        case 12:
                            Console.Clear();
						    return;
                        default:
                            Console.WriteLine("Invalid input. Press Enter to try again!");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                    }
                }
                catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
            }
        }

        //STAFF
        public static void StaffMenu()
        {
            while (true)
            {
                Console.WriteLine("--------------Staff Main Menu-------------");
                menu();
                Console.WriteLine("------------------------------------------\n");
                int command;

                try
                {
                    command = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input, please enter a valid number.");
                    continue;
                }
                switch (command)
                {
                    case 1:
						// Purchase membership
						PurchaseMembershipForCustomer();
						break;
                    case 2:
                        // TODO: Manage Account
                        Console.Clear();
                        StaffMenu();
                        break;
                    case 3:
						PurchaseDayPassForCustomer();
                        break;
                    case 4:
                        // Cancel reservation
                        CancelReservationForCustomer();
                        break;
                    case 5:
                        Console.Clear();
                        GroupSchedule.showActivities();
                        Console.WriteLine("Press Enter to continue!");
                        Console.ReadLine();
                        Console.Clear();                        
                        break;
                    case 6:
                        Console.Clear();
                        manageSchedule();
                        break;
                    case 7:
						// Make reservation
						ViewCustomers();    
                        MakeReservationForCustomer();
                        Console.Clear();
                        break;
                    case 8:
                        // View items
                        ViewItems();
                        Console.Clear();
                        break;
                    case 9:
                        Console.Clear();
                        RestrictItem();
                        break;
                    case 10:
                        Console.Clear();
                        // TODO: Add item
                        break;
                    case 11:
                        Console.Clear();
						return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
        }

        //CUSTOMER
        public static void UserMenu()
        {
            //Console.Clear();
            if (logedInUser.accessLevels == AccessLevels.NonPayingNonMember)
            {
                Console.WriteLine("-------------None Member Access Menu-------------");                
            }
            else
            {
                Console.WriteLine("------------- Member Access Menu-------------");
            }
            
            menu();
            //Console.WriteLine(Convert.ToString(mamma.Length) * "_");
            Console.WriteLine("-----------------------------------\n");

            try
            {
                int command = int.Parse(Console.ReadLine());                
                switch (command)
                {                   
                    case 1:
                        // Purchase Membership
                        // Somewhere here the customer should be able to edit their details if they aren't a member.
                        logedInUser.accessLevels = AccessLevels.PayingMember;
                        Console.WriteLine("You have bought a Pro Membership Deluxe");                        
                        Console.ReadLine();
                        Console.Clear();
                        PayingMemberMenu();
                        break;
                    case 2:
						// Purchase daypass
						// Somewhere here the customer should be able to edit their details if they aren't a member.
						logedInUser.accessLevels = AccessLevels.DayPassUser;
                        Console.WriteLine("You have now been upgraded to Day Pass User!");
                        Console.ReadLine();
                        Console.Clear();
                        DayPassMenu();
                        break;
                    case 3:
                        // Cancel reservation | Not quite functional, removes item from customer but not customer from item.
						// Probably due to problem i saving in MakeReservatio
                        Customer.CancelReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                           break;
                    case 4:
                        Console.Clear();
                        GroupSchedule.showActivities();                        
                        Console.WriteLine("Press Enter to continue!");
                        Console.ReadLine();
                        Console.Clear();
                        UserMenu();
                        break;
                    case 5:
                        // Make Reservation
                        Console.Clear();
                        ReserveMenu(logedInUser.accessLevels);
                        UserMenu();
                        break;
                    case 6:
                        //TODO View Item
                        Console.Clear();
                        ViewItems();
                        UserMenu();
                        break;
                    case 7:
                        Console.Clear();
                        //Should find a way to remove the NonMember from the customerlist if they didn't reserve anything
                        // Meh
                        //if (logedInUser.reservedItems == null)
                        //{
                        //    foreach (var customer in Customer.customerList)
                        //    {
                        //        if (logedInUser.uniqueID == customer.uniqueID && logedInUser.name == customer.name)
                        //        {
                        //            Customer.customerList.Remove(customer);
                        //        }
                        //    }
                        //}
                        return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
        }
        public static void DayPassMenu()
        {
            Console.WriteLine("-------------Day Pass-------------");
            menu();
            Console.WriteLine("----------------------------------\n");
            try
            {
                int command = int.Parse(Console.ReadLine());

                switch (command)
                {
                    case 1:
                        Console.Clear();
						// Purchase membership
						logedInUser.accessLevels = AccessLevels.PayingMember;
						Console.WriteLine("You have bought a Pro Membership Deluxe");
						Console.ReadLine();
						Console.Clear();
						PayingMemberMenu();
						break;
                    case 2:
                        Console.Clear();
                        // TODO: Manage Account
                        break;
                    case 3:
						// Cancel reservation | Not quite functional, removes item from customer but not customer from item.
						// Probably due to problem i saving in MakeReservation()
						Customer.CancelReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                        Console.Clear();
                        break;
                    case 4:
                        // View Group Schedule
                        Console.Clear();
                        GroupSchedule.showActivities();
                        Console.WriteLine("Press Enter to continue!");
                        Console.ReadLine();
                        Console.Clear();                        
                        break;
                    case 5:
						// Make reservation | Saving is scuffed on the item
						Console.Clear();
                        ReserveMenu(logedInUser.accessLevels);
                        break;
                    case 6:
                        Console.Clear();
                        // View Items
                        ViewItems();
                        break;
                    case 7:
                        Console.Clear();
						return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }

        }
        public static void PayingMemberMenu()
        {
            while (true)
            {
                Console.WriteLine("-------------Paying Member-------------");
                menu();
                Console.WriteLine("---------------------------------------");
                int command;

                try
                {
                    command = int.Parse(Console.ReadLine());
                    switch (command)
                    {

                        case 1:
                            Console.Clear();
                            // TODO: Manage Account
                            break;
                        case 2:
                            Console.Clear();
                            // Cancel reservation | Not quite functional, removes item from customer but not customer from item.
                            // Probably due to problem i saving in MakeReservation()
                            Customer.CancelReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                            break;
                        case 3:
                            Console.Clear();
                            GroupSchedule.showActivities();
                            Console.WriteLine("Press Enter to continue!");
                            Console.ReadLine();
                            Console.Clear();                            
                            break;
                        case 4:
                            Console.Clear();
                            // Make reservation | Saving is scuffed on the item
                            ReserveMenu(logedInUser.accessLevels);
                            break;
                        case 5:
                            Console.Clear();
                            // View items
                            ViewItems();
                            Console.Clear();
                            break;
                        case 6:
                            Console.Clear();
						    return;
                        default:
                            Console.WriteLine("Invalid input. Press Enter to try again!");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                    }
                }
                catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
            }
        }

        //SERVICE
        public static void ServiceMenu()
        {
			Console.WriteLine("--------------Service-------------");
			Console.WriteLine(logedInUser);
			Console.WriteLine("__________________________________");
			Console.WriteLine("1. Item Repair");
            Console.WriteLine("2. Log out");
            Console.WriteLine("----------------------------------\n");
            try
            {
                int command = int.Parse(Console.ReadLine());

                switch (command)
                {
                    case 1:
                        // TODO: Item Repair
                        Console.Clear();
                        Equipment.RepairEquipment();
                        ServiceMenu();
                        break;
                    case 2:
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
        }

        //OTHER MENU OPTIONS
        public static void manageSchedule()
        {
            Console.Clear();
            Console.WriteLine("--------------Manage Group Schedule-------------");
            Console.WriteLine("1. Create New Group Activity");
            Console.WriteLine("2. Edit Group Activity");
            Console.WriteLine("3. Delete Group Activity");
            Console.WriteLine("4. Go Back");
            Console.WriteLine("------------------------------------------------\n");
            try
            {
                int command = int.Parse(Console.ReadLine());

                switch (command)
                {
                    case 1:
                        Console.Clear();
                        GroupSchedule.addActivity();
                        break;
                    case 2:
                        //TODO Edit Group Activity
                        GroupSchedule.editActivity();
                        
                        break;
                    case 3:
                        //TODO Delete Group Activity
                        Console.Clear();
                        GroupSchedule.deleteActivity();
                        break;
                    case 4:
                        //Console.Clear();
                        //Program.MainMenu();
                        return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            catch { Console.WriteLine("Invalid input. Press Enter to try again!"); Console.ReadLine(); Console.Clear(); }
        }
        public static void ViewItems()
        {
            Console.Clear();
            Console.WriteLine("---------------------------------------");
			Console.WriteLine("What type of Item you you like to see?\n" +
                "1. Equipments\n" +
                "2. Spaces\n" +
                "3. Personal Trainers");
			Console.WriteLine("---------------------------------------");
			int i = int.Parse(Console.ReadLine());
            Console.Clear();

            switch (i)
            {
                case 1:
					Console.WriteLine("---------------------------------------");
					Equipment.ShowAvailable();
                    Console.WriteLine("Press enter to go back");
                    Console.ReadLine();
					Console.WriteLine("---------------------------------------");
					break;
                case 2:
					Console.WriteLine("---------------------------------------");
					Space.ShowAvailable();
					Console.WriteLine("Press enter to go back");
					Console.ReadLine();
					Console.WriteLine("---------------------------------------");
					break;
                case 3:
					Console.WriteLine("---------------------------------------");
					PersonalTrainer.ShowAvailable();
					Console.WriteLine("Press enter to go back");
					Console.ReadLine();
					Console.WriteLine("---------------------------------------");
					break;
            }
        }
        public static void ReserveMenu(AccessLevels accessLevels)
        {
            Console.WriteLine("What would you like to reserve?");
            List<string> reservationOptions = new List<string>()
            {
                "Equipment",
                "Space",
                "Personal Trainer",
                "Group Activity",
                "Go Back"
            };
            if (accessLevels == AccessLevels.NonPayingNonMember)
            {
                reservationOptions.Remove("Space");
                reservationOptions.Remove("Personal Trainer");
                reservationOptions.Remove("Group Activity");
            }
            for (int i = 0; i < reservationOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {reservationOptions[i]}");
            }
            int n = int.Parse(Console.ReadLine());

            if (accessLevels == AccessLevels.NonPayingNonMember)
            {
                switch (n)
                {
                    case 1:
                        // Equipment
                        Equipment myEquipment = new Equipment();
                        myEquipment.MakeReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                        break;
                    case 2:
                        // Go Back
                        break;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
            }
            else
            {     
                switch (n)
                {
                    case 1:
                        // Equipment
                        Equipment myEquipment = new();
                        myEquipment.MakeReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                        break;
                    case 2:
                        // Space
                        Space mySpace = new();
                        mySpace.MakeReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                        break;
                    case 3:
                        // Personal Trainer
                        PersonalTrainer myTrainer = new();
                        myTrainer.MakeReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
                        break;
                    case 4:
                        Console.Clear();
                        GroupSchedule.showActivities();
                        Console.WriteLine("What group activity do you want to participate in? (Enter the type of activity)");
                        string activityChoice = Console.ReadLine();
                        for (int i = 0; i < GroupSchedule.groupScheduleList.Count; i++)
                        {
                            if (GroupSchedule.groupScheduleList[i].typeOfActivity.ToLower().Contains(activityChoice.ToLower()))
                            {
                                Customer addToAcitivy = new Customer(logedInUser.name, logedInUser.phone, logedInUser.email, logedInUser.accessLevels);
                                GroupSchedule.addCustomerToActivity(addToAcitivy, GroupSchedule.groupScheduleList[i]);
                            }
                        }
                        break;
                    case 5:
                        // Go Back
                        break;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again!");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }

            }

        }
        public static void RestrictItem()
        {
            Console.Clear();
            Console.WriteLine("----------Restrict Item---------");
            Console.WriteLine("1. Equipment");
            Console.WriteLine("2. Space");
            Console.WriteLine("3. Go back");
            Console.WriteLine("--------------------------------");
            int command = int.Parse(Console.ReadLine());
            switch (command)
            {
                case 1:
                    Equipment.RestrictEquipment();
                    break;
                case 2:
                    Space.RestrictSpace();
                    break;
                case 3:
                    StaffMenu();
                    break;
            }
        }
        public static void ViewCustomers()
        {
            Console.Clear();
			Console.WriteLine("--------------Customer List-------------");
			if (Customer.customerList.Count > 0)
            {
                for (int c = 0; c < Customer.customerList.Count; c++)
                {
                    Console.WriteLine($"{c+1}. {Customer.customerList[c]}\n");
                }
            }
            else
            {
                Console.WriteLine("There are no customers!");
                Console.ReadLine();
            }
            Console.WriteLine("----------------------------------------");

		}
        public static void MakeReservationForCustomer()
        {
			Console.WriteLine("--------------Make Reservation-------------");
			ViewCustomers();
			Console.WriteLine("-------------------------------------------\n");
			Console.WriteLine("For which customer does it concern?");
			int c = int.Parse(Console.ReadLine());
			User tempStaff = logedInUser;
			logedInUser = Customer.customerList[c - 1];
			ReserveMenu(logedInUser.accessLevels);
			logedInUser = tempStaff;
		}
        public static void CancelReservationForCustomer()
        {
			Console.WriteLine("--------------Cancel Reservation-------------");
			ViewCustomers();
			Console.WriteLine("---------------------------------------------\n");
			Console.WriteLine("For which customer does it concern?");
			int c = int.Parse(Console.ReadLine());
			User tempStaff = logedInUser;
            logedInUser = Customer.customerList[c - 1];
			Customer.CancelReservation(logedInUser.ID, logedInUser, logedInUser.accessLevels);
			logedInUser = tempStaff;
		}
        public static void PurchaseMembershipForCustomer()
        {
			Console.WriteLine("--------------Purchase Membership-------------");
			ViewCustomers();
			Console.WriteLine("----------------------------------------------\n");

			Console.WriteLine("For which customer does it concern?");
			int c;
			bool isValidIndex = int.TryParse(Console.ReadLine(), out c) && c > 0 && c <= Customer.customerList.Count;
			Console.Clear();
			if (isValidIndex)
			{
				Customer current = Customer.customerList[c - 1];

				if (current.accessLevels != AccessLevels.PayingMember)
				{
					Console.WriteLine($"You are upgrading \n{current} \nto Pro Membership Deluxe.\n\n" +
						$"Is this Correct? Y/N");
					string confirm = Console.ReadLine().ToUpper();
					if (confirm == "Y")
					{
						User tempUser = logedInUser;

						logedInUser = current;
						logedInUser.accessLevels = AccessLevels.PayingMember;
						Console.Clear();
						Console.WriteLine($"Customer: {logedInUser.uniqueID} | {logedInUser.name} now has a Pro Membership Deluxe!\n" +
							$"Press enter to continue...");
						logedInUser = tempUser;
					}
					else
					{
						PurchaseMembershipForCustomer();
					}
				}
				else
				{
					Console.WriteLine($"Customer: {current.uniqueID} | {current.name} already has a Pro Membership Deluxe!\n" +
						$"Press enter to continue...");
				}
				Console.ReadLine();
				Console.Clear();
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid customer index.");
			}
		}
        public static void PurchaseDayPassForCustomer()
        {
			Console.WriteLine("--------------Purchase DayPass-------------");
			ViewCustomers();
			Console.WriteLine("-------------------------------------------\n");

			Console.WriteLine("For which customer does it concern?");
			int c;
			bool isValidIndex = int.TryParse(Console.ReadLine(), out c) && c > 0 && c <= Customer.customerList.Count;
			Console.Clear();
			if (isValidIndex)
			{
				Customer current = Customer.customerList[c - 1];

				if (current.accessLevels != AccessLevels.PayingMember && current.accessLevels != AccessLevels.DayPassUser)
				{
					Console.WriteLine($"You are adding a DayPass to: \n{current} \n\n" +
						$"Is this Correct? Y/N");
					string confirm = Console.ReadLine().ToUpper();
					if (confirm == "Y")
					{
						User tempUser = logedInUser;

						logedInUser = current;
						logedInUser.accessLevels = AccessLevels.DayPassUser;
						Console.Clear();
						Console.WriteLine($"Customer: {logedInUser.uniqueID} | {logedInUser.name} now has a DayPass!\n" +
							$"Press enter to continue...");
						logedInUser = tempUser;
					}
					else
					{
						PurchaseDayPassForCustomer();
					}
				}
				else
				{
					Console.WriteLine($"Customer: {current.uniqueID} | {current.name} already has a DayPass!\n" +
						$"Press enter to continue...");
				}
				Console.ReadLine();
				Console.Clear();
			}
			else
			{
				Console.WriteLine("Invalid input. Please enter a valid customer index.");
			}
		}
        public int ChooseDb()
        {
            Console.WriteLine("Do you want to use CSV or database?\n");
            Console.WriteLine("1. CSV");
            Console.WriteLine("2. Database");
            int cmd;
            try
            {
                if (int.TryParse( Console.ReadLine(), out cmd))
                {
                    Console.Clear();
                    return cmd;
                }
                else 
                {
					Console.WriteLine("Try again!");
                    Console.Clear();
					return 0;
				}

            }
            catch (Exception e) { Console.WriteLine("Try again!"); return 0; }
        }
	}
}
