using Gym_Booking_Manager.Interfaces;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Gym_Booking_Manager.Space;


#if DEBUG
[assembly: InternalsVisibleTo("SpaceTest")]
#endif
namespace Gym_Booking_Manager
{
    internal class Space : Resources, IReservable, ICSVable
    {
       
        public SpaceCategory spaceCategory { get; set; }
        public Availability spaceAvailability { get; set; }
        
		public List<string> reservedTimeSlot { get; set; }
        public static int index { get; set; } = 0;
		public static List<string> TimeSlot { get; set; } = new List<string>()
		{
			"12:00-13:00",
			"13:00-14:00",
			"14:00-15:00"
		};
		//public static List<string> spaceColumn { get; set; } = new List<string>() { "spaceCategory", "name", "spaceAvailability" };
		public Space(string name = "", SpaceCategory spaceCategory = 0, Availability availability = 0, IReservingEntity owner = null, string timeSlot = "") :base(name,TimeSlot,"",null)
        {
            this.spaceCategory = spaceCategory;
            this.spaceAvailability = availability;
            this.timeslot = timeSlot;
            this.owner = owner;
            this.reservedTimeSlot= new List<string>();
        }

        public override string ToString()
        {
            return $"Namn: {name}, Category: {spaceCategory}, Availability: {spaceAvailability}"; // TODO: Don't use CSVify. Make it more readable.
        }

        public string CSVify()
        {
            return $"{nameof(spaceCategory)}:{spaceCategory.ToString()},{nameof(name)}:{name},{nameof(spaceAvailability)}:{spaceAvailability.ToString()}";
        }        
        public enum SpaceCategory
        {
            hall,
            lane,
            studio
        }
        public enum Availability
        {
            available,
            unavailable,
            reserved
        }
        public Availability SetAvailability(Availability availability)
        {
            return this.spaceAvailability = availability;
        }
        public static void ShowAvailable(string timeslot = null)
        {
			spaceList = spaceList.OrderBy(x => x.spaceAvailability != Availability.available).ToList();
			spaceList = spaceList.OrderBy(x => x.reservedTimeSlot.Contains(timeslot)).ToList();
			int index = 0;
			for (int i = 0; i < spaceList.Count; i++)
			{
				if (spaceList[i].spaceAvailability == Availability.available && !spaceList[i].reservedTimeSlot.Contains(timeslot))
				{
					index++;
					Console.WriteLine(i + 1 + " " + spaceList[i].name);
				}
			}
		}
        public static void ShowUnavailable()
        {
			for (int i = 0; i < spaceList.Count; i++)
			{
				if (spaceList[i].spaceAvailability == Availability.unavailable)
				{
					Console.WriteLine(i + 1 + " " + spaceList[i].name);
				}
			}
		}

        // Perhaps a Unrestrict Space would be good?
        public static void RestrictSpace()
        {
			List<Space> temp = new List<Space>();
			foreach (var space in spaceList)
			{
				if (space.spaceAvailability == Availability.available)
				{
					temp.Add(space);
				}
			}
			Console.Clear();
            if (temp.Count > 0)
            {
                Console.WriteLine("Choose space");
                Space.ShowAvailable();
                int n = int.Parse(Console.ReadLine());

                Console.Clear();
                spaceList[n - 1].spaceAvailability = Availability.unavailable;
                Console.WriteLine($"{spaceList[n - 1].name} - set to {spaceList[n - 1].spaceAvailability}");
                Console.WriteLine("Press enter...");
                Console.ReadLine();
                Menutracker.RestrictItem();
            }
            else
            {
                Console.WriteLine("No available spaces!");
                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                Menutracker.RestrictItem();
            }
            
		}

		// Make reservation | Saving is scuffed on the item
		public void MakeReservation(IReservingEntity owner, User customer ,AccessLevels accessLevel)
		{
			Console.Clear();
			int index = 1;
			for (int i = 0; i < TimeSlot.Count; i++)
			{
				Console.WriteLine(index + " " + TimeSlot[i]);
				index++;
			}
			int timeSlotChoice = Convert.ToInt32(input("During which time would you like to reserve the space?\n"));

			List<Space> temp = new List<Space>();

			for (int i = 0; i < spaceList.Count; i++)
			{
				if (spaceList[i].spaceAvailability == Availability.available && !spaceList[i].reservedTimeSlot.Contains(TimeSlot[timeSlotChoice - 1]))
				{
					temp.Add(spaceList[i]);
				}
			}
            if (temp.Count > 0)
            {
			    Console.Clear();
                ShowAvailable(TimeSlot[timeSlotChoice -1]);
			    int n = Convert.ToInt32(input("What space would you like to reserve?\n"));
                Console.Clear();
			    string confirm = input($"You would like to reserve {temp[n - 1].name} during {TimeSlot[timeSlotChoice - 1]}.\n" +
				    $"Is this correct? Y / N\n").ToLower();

			    if (confirm == "y")
			    {
				    temp[n - 1].owner = owner;
					temp[n - 1].reservedTimeSlot.Add(TimeSlot[timeSlotChoice - 1]);
                    temp[n - 1].timeslot = TimeSlot[timeSlotChoice - 1];
					customer.reservedItems.Add(new Space(temp[n - 1].name, temp[n - 1].spaceCategory, 0, null, temp[n - 1].timeslot));

					Console.Clear();
                    Console.WriteLine($"You have reserved {temp[n - 1].name} during {TimeSlot[timeSlotChoice - 1]}");

					input("Press enter...");
					Console.Clear();
					Menutracker.ReserveMenu(accessLevel);
				}
                else if (confirm == "n")
                {
                    Menutracker.ReserveMenu(accessLevel);
                }

            }
            else
            {
                Console.WriteLine("There is no available space during your choosen time");
				Menutracker.ReserveMenu(accessLevel);
			}
		}

        public static Space FindByName(string name)
        {
            var space = spaceList.FirstOrDefault(s => s.name == name);            
            return space;
        }
    }
}
