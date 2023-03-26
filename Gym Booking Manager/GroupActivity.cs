using Gym_Booking_Manager;
using Gym_Booking_Manager.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gym_Booking_Manager.Equipment;
using static System.Net.Mime.MediaTypeNames;

namespace Gym_Booking_Manager
{
    internal class GroupActivity : ICSVable
    {
        public int activtyId { get; set; }
        public int participantLimit { get; set; }
        public string timeSlot { get; set; }
        public List<Customer> participants { get; set; }
        public List<PersonalTrainer> personalTrainer { get; set; }
        public Space space { get; set; }
        public List<Equipment> equipment { get; set; }
		public string typeOfActivity { get; set; }
		public static List<string> groupColumn { get; set; } = new List<string>() { "personalTrainer", "typeOfActivity", "activtyId", "participantLimit", "timeSlot", "participants", "space", "equipment" };
		public GroupActivity(
		List<PersonalTrainer> personalTrainer = null,
		string typeOfActivity = "",
		int activtyId = 0,
		int participantLimit = 0,
		string timeSlot = "",
		List<Customer> participants = null,
		Space space = null,
		List<Equipment> equipment = null
		)
		{
			this.typeOfActivity = typeOfActivity;
			this.activtyId = activtyId;
			this.participantLimit = participantLimit;
			this.timeSlot = timeSlot;
			this.participants = participants;
			this.personalTrainer = personalTrainer;
			this.space = space;
			this.equipment = equipment;
        }
        public override string ToString()
        {
            
            string participantNames = "";
            if (participants != null && participants.Count > 0)
            {
                foreach (var participant in participants)
                {
                    participantNames += participant.name + "\n";
                }
            }
            else
                participantNames = "No one is booked for this activity\n";            

            string equipmentNames = "";
            if (equipment.Count > 0)
            {
                foreach (var EQ in equipment)
                {
                    equipmentNames += EQ.name + ", ";
                }
            }
            else
                equipmentNames = "No Equipment for this session";

            string personalTrainerName = "";
            if (personalTrainer.Count > 0)
            {
                foreach (var PT in personalTrainer)
                {
                    personalTrainerName += PT.name + ", ";
                }
            }
            else
                personalTrainerName = "No Personal Trainer for this session";

            return $"---------------------------------------------\n" +
                $"Type Of Activity: {typeOfActivity}\n" +
                $"Activity ID: {activtyId}\n" +
                $"Perticipant Limit: {participantLimit}\n" +
                $"Timeslot: {timeSlot}\n" +
                $"Participants:\n" +
                $"-----------------\n" +
                $"{participantNames}" +
                $"-----------------\n" +
                $"Space: {space.name}\n" +
                $"Equipment: {equipmentNames}\n" +
                $"Personal Trainer: {personalTrainerName}\n" +
                $"---------------------------------------------";
        }
        public string CSVify()
        {
            var personalTrainerNames = "";
            if (personalTrainer != null && personalTrainer.Count > 0)
            {
                for (int i = 0; i < personalTrainer.Count; i++)
                {
                    personalTrainerNames += personalTrainer[i].name;
                    if (i != personalTrainer.Count - 1)
                    {
                        personalTrainerNames += ";";
                    }
                }
            }

            var participantNames = "";
            if (participants != null && participants.Count > 0)
            {
                for (int i = 0; i < participants.Count; i++)
                {
                    participantNames += participants[i].name;
                    if (i != participants.Count - 1)
                    {
                        participantNames += ";";
                    }
                }
            }

            var equipmentNames = "";
            if (equipment != null && equipment.Count > 0)
            {
                for (int i = 0; i < equipment.Count; i++)
                {
                    equipmentNames += equipment[i].name;
                    if (i != equipment.Count - 1)
                    {
                        equipmentNames += ";";
                    }
                }
            }

            return $"personalTrainer:{personalTrainerNames}," +
                   $"typeOfActivity:{typeOfActivity}," +
                   $"activtyId:{activtyId}," +
                   $"participantLimit:{participantLimit}," +
                   $"timeSlot:{timeSlot}," +
                   $"participants:{participantNames}," +
                   $"space:{space.name}," +
                   $"equipment:{equipmentNames}";
        }
    }
}