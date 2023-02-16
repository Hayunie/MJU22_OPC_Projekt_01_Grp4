﻿using Gym_Booking_Manager.Interfaces;
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
[assembly: InternalsVisibleTo("Tests")]
#endif
namespace Gym_Booking_Manager
{
    // IComparable<T> interface requires implementing the CompareTo(T other) method.
    // This interface/method is used by for instance SortedSet<T>.Add(T) (and other sorted collections).
    // There is also the non-generic IComparable interface for a CompareTo(Object obj) implementation.
    //
    // The current database class implementation uses SortedSet, and thus classes and objects that we want to store
    // in it should inherit the IComparable<T> interface.
    //
    // As alluded to from previous paragraphs, implementing IComparable<T> is not exhaustive to cover all "comparisons".
    // Refer to official C# documentation to determine what interface to implement to allow use with
    // the class/method/operator that you want.
    internal class Space : Resources, IReservable, ICSVable, IComparable<Space>
    {
        //private static readonly List<Tuple<Category, int>> hourlyCosts = InitializeHourlyCosts(); // Costs may not be relevant for the prototype. Let's see what the time allows.
        public SpaceCategory spaceCategory { get; set; }
        private Availability spaceAvailability;
        public string timeSlot;	
		public List<string> reservedTimeSlot { get; set; }
        int index = 0;
		public static List<string> TimeSlot = new List<string>()
		{
			"12:00-13:00",
			"13:00-14:00",
			"14:00-15:00"
		};
		public Space(string name = "", SpaceCategory spaceCategory = 0, Availability availability = 0, IReservingEntity owner = null, string timeSlot = "") :base(name,TimeSlot,"",null)
        {
            this.spaceCategory = spaceCategory;
            this.spaceAvailability = availability;
            this.timeSlot = timeSlot;
            this.owner = owner;
            this.reservedTimeSlot= new List<string>();
        }
        public int CompareTo(Space? other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;
            // Sort primarily on category.
            if (this.spaceCategory != other.spaceCategory) return this.spaceCategory.CompareTo(other.spaceCategory);
            // When category is the same, sort on name.
            return this.name.CompareTo(other.name);
        }
        public override string ToString()
        {
            return $"Namn: {name}, Category: {spaceCategory}, Availability: {spaceAvailability}"; // TODO: Don't use CSVify. Make it more readable.
        }

        // Every class C to be used for DbSet<C> should have the ICSVable interface and the following implementation.
        public string CSVify()
        {
            return $"{nameof(spaceCategory)}:{spaceCategory.ToString()},{nameof(name)}:{name},{nameof(spaceAvailability)}:{spaceAvailability.ToString()}";
        }        
        public enum SpaceCategory
        {
            Hall = 1,
            Lane,
            Studio
        }
        public enum Availability
        {
            Available,
            Unavailable,
            Reserved
        }
        public Availability SetAvailability(Availability availability)
        {
            return this.spaceAvailability = availability;
        }
        public static void ShowAvailable(string timeslot = null)
        {
			spaceList = spaceList.OrderBy(x => x.spaceAvailability != Availability.Available).ToList();
			spaceList = spaceList.OrderBy(x => x.reservedTimeSlot.Contains(timeslot)).ToList();
			int index = 0;
			for (int i = 0; i < spaceList.Count; i++)
			{
				if (spaceList[i].spaceAvailability == Availability.Available && !spaceList[i].reservedTimeSlot.Contains(timeslot))
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
				if (spaceList[i].spaceAvailability == Availability.Unavailable)
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
				if (space.spaceAvailability == Availability.Available)
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
                spaceList[n - 1].spaceAvailability = Availability.Unavailable;
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
				if (spaceList[i].spaceAvailability == Availability.Available && !spaceList[i].reservedTimeSlot.Contains(TimeSlot[timeSlotChoice - 1]))
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
                    temp[n - 1].timeSlot = TimeSlot[timeSlotChoice - 1];
					customer.reservedItems.Add(new Space(temp[n - 1].name, temp[n - 1].spaceCategory, 0, null, temp[n - 1].timeSlot));

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

		// Consider how and when to add a new Space to the database.
		// Maybe define a method to persist it? Any other reasonable schemes?

		//private static List<Tuple<Category, int>> InitializeHourlyCosts()
		//{
		//    // TODO: fetch from "database"
		//    var hourlyCosts = new List<Tuple<Category, int>>
		//    {
		//        Tuple.Create(Category.Hall, 500),
		//        Tuple.Create(Category.Lane, 100),
		//        Tuple.Create(Category.Studio, 400)
		//    };
		//    return hourlyCosts;
		//}

		static public string input(string prompt)
		{
			Console.Write(prompt);
			return Console.ReadLine();
		}
        public static Space FindByName(string name)
        {
            var space = spaceList.FirstOrDefault(s => s.name == name);            
            return space;
        }
    }
}
