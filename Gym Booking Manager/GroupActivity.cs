﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Booking_Manager
{
    internal class GroupActivity
    {
        public int activtyId { get; set; }
        public int participantLimit { get; set; }
        public string timeSlot { get; set; }
        public List<Customer> participants;
        public List<PersonalTrainer> personalTrainer;
        public List<Space> space { get; set; }
        public List<Equipment> equipment { get; set; }

        public GroupActivity(List<PersonalTrainer> personalTrainer, int activtyId = 0, int participantLimit = 0, string timeSlot = "", List<Customer> participants = null, List<Space> spaces = null, List<Equipment> equipment = null)
        {
            this.activtyId = activtyId;
            this.participantLimit = participantLimit;
            this.timeSlot = timeSlot;
            this.participants = new List<Customer>();
            this.personalTrainer = new List<PersonalTrainer>();
            this.space = spaces = new List<Space>();
            this.equipment = equipment;
        }
        public override string ToString()
        {
            string participantNames = "";
            if (participants.Count > 0)
            {
                foreach (var participant in participants)
                {
                    participantNames += participant.name + "\n";
                }
            }
            else
                participantNames = "No one is booked for this activity\n";
            return $"Activity ID: {activtyId}\n" +
                $"Perticipant Limit: {participantLimit}\n" +
                $"Timeslot: {timeSlot}\n" +
                $"Participants:\n" +
                $"-----------------\n" +
                $"{participantNames}" +
                $"-----------------\n" +
                $"Space: {space[0].name}\n" +
                $"Equipment: {equipment}\n" +
                $"Personal Trainer: {personalTrainer[0].name}";
        }
    }
}
