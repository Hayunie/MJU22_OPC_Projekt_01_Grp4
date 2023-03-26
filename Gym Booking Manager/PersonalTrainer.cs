using Gym_Booking_Manager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gym_Booking_Manager.Equipment;
using static Gym_Booking_Manager.Space;

namespace Gym_Booking_Manager
{
    internal class PersonalTrainer : Resources, ICSVable, IReservable
    {
        public IReservingEntity owner { get; set; }
        public TrainerCategory trainerCategory { get; set; }
        public Availability trainerAvailability { get; set; }
        public List<string> reservedTimeSlot { get; set; }
        public static int index { get; set; } = 0;
		public static List<string> TimeSlot { get; set; } = new List<string>()
		{
			"12:00-13:00",
			"13:00-14:00",
			"14:00-15:00"
		};
		//public static List<string> trainerColumn { get; set; } = new List<string>() { "trainerCategory", "name", "trainerAvailability" };
		public PersonalTrainer(string name = "", TrainerCategory trainerCategory = 0, Availability availability = Availability.available, IReservingEntity owner = null, string timeSlot = "")
        {
            this.owner = owner;
            this.name = name;
            this.trainerCategory = trainerCategory;
            this.reservedTimeSlot = new List<string>();
        }
        public enum TrainerCategory
        {
            yoga_instructor,
            gym_instructor,
            tennis_teacher
        }
        public enum Availability
        {
            available,
            reserved
        }
        public Availability SetAvailability(Availability availability)
        {
            return this.trainerAvailability = availability;
        }
        public static void ShowAvailable(string timeslot = "")
        {
            personalTrainers = personalTrainers.OrderBy(x => x.trainerAvailability != Availability.available).ToList();
            personalTrainers = personalTrainers.OrderBy(x => x.reservedTimeSlot.Contains(timeslot)).ToList();
            index = 0;
            for (int i = 0; i < personalTrainers.Count; i++)
            {
                if (personalTrainers[i].trainerAvailability == Availability.available && !personalTrainers[i].reservedTimeSlot.Contains(timeslot))
                {
                    index++;
                    Console.WriteLine(i + 1 + " " + personalTrainers[i].name);
                }
            }
        }
        public static void ReservTrainer(PersonalTrainer trainer, string timeslot, string customer)
        {
            if (trainer.trainerAvailability == Availability.available && !trainer.reservedTimeSlot.Contains(timeslot))
            {
                trainer.reservedTimeSlot.Add(timeslot);
                IReservingEntity activity = new ReservingEntity(customer);
                trainer.owner = activity;
            }
            else
            {
                Console.WriteLine("This personal trainer is not available for reservation during that timeslot.");
            }
        }
        public override string ToString()
        {
            return $"Namn: {name}, Category: {trainerCategory}, Avilability: {trainerAvailability}";
        }

		// Make reservation | Saving is scuffed on the item
		public void MakeReservation(IReservingEntity owner, User customer, AccessLevels accessLevel)
        {
            Console.Clear();
            int index = 1;
            for (int i = 0; i < TimeSlot.Count; i++)
            {
                Console.WriteLine(index + " " + TimeSlot[i]);
                index++;
            }
            int timeSlotChoice = Convert.ToInt32(input("During which time would you like to book the trainer?\n"));

            List<PersonalTrainer> temp = new List<PersonalTrainer>();

            for (int i = 0; i < personalTrainers.Count; i++)
            {
                if (personalTrainers[i].trainerAvailability == Availability.available && !personalTrainers[i].reservedTimeSlot.Contains(TimeSlot[timeSlotChoice - 1]))
                {
                    temp.Add(personalTrainers[i]);
                }
            }
            if (temp.Count > 0)
            {
                Console.Clear();
                ShowAvailable(TimeSlot[timeSlotChoice - 1]);
                int n = Convert.ToInt32(input("Which trainer would you like to book?\n"));
                Console.Clear();
                string confirm = input($"You would like to reserve {temp[n - 1].name} during {TimeSlot[timeSlotChoice - 1]}.\n" +
                    $"Is this correct? Y / N\n").ToLower();

                if (confirm == "y")
                {
                    temp[n - 1].owner = owner;
                    temp[n - 1].reservedTimeSlot.Add(TimeSlot[timeSlotChoice - 1]);
                    temp[n - 1].timeslot = TimeSlot[timeSlotChoice - 1];
					customer.reservedItems.Add(new PersonalTrainer(temp[n - 1].name, temp[n - 1].trainerCategory, 0, null, temp[n - 1].timeslot));
					// Save the equipment on the owner... Does the owners hava a list with reserved equipments?
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
                Console.WriteLine("There is no available trainer during your choosen time");
                Menutracker.ReserveMenu(accessLevel);
            }



        }

        public string CSVify()
        {
            return $"{nameof(trainerCategory)}:{trainerCategory.ToString()},{nameof(name)}:{name}, {nameof(trainerAvailability)}:{trainerAvailability.ToString()}";
        }        
    }
}
