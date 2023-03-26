using Gym_Booking_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using System.Xml.Linq;
using Gym_Booking_Manager.Interfaces;
using static Gym_Booking_Manager.Space;
using System.ComponentModel;

namespace Gym_Booking_Manager
{
    public class CsvHandler
    {
        private static int initialized = 0;

        public static void ReadFile(string fileName)
        { 
            string filePath = Path.Combine("CSV", fileName);            

            if (initialized != 1)
            {
                if (fileName == "Spaces.txt")
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] values = line.Split(',');
                        string name = "";
                        Space.SpaceCategory spaceCategory = 0;
                        Space.Availability availability = 0;
                        //IReservingEntity owner = null;
                        //string timeSlot = "";
                        //Calendar calendar = null;
                        foreach (string value in values)
                        {
                            string[] parts = value.Split(':');
                            if (parts[0] == "spaceCategory")
                            {
                                Enum.TryParse(parts[1], out spaceCategory);
                            }
                            else if (parts[0] == "name")
                            {
                                name = parts[1];
                            }
                            else if (parts[0] == "spaceAvailability")
                            {
                                Enum.TryParse(parts[1], out availability);
                            }
                        }
                        Space.spaceList.Add(new Space(name, spaceCategory, availability));
                    }
                }
                else if (fileName == "Equipment.txt")
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] values = line.Split(',');
                        string name = "";
                        Equipment.EquipmentCategory equipmentCategory = 0;
                        Equipment.EquipmentType equipmentType = 0;
                        Equipment.Availability availability = 0;

                        var reservedTimeSlot = "";
                        
                        foreach (string value in values)
                        {
                            string[] parts = value.Split(':');
                            if (parts[0] == "equipmentCategory")
                            {
                                Enum.TryParse(parts[1], out equipmentCategory);
                            }
                            else if (parts[0] == "name")
                            {
                                name = parts[1];
                            }
                            else if (parts[0] == "equipmentAvailability")
                            {
                                Enum.TryParse(parts[1], out availability);
                            }
                            else if (parts[0] == "equipmentType")
                            {
                                Enum.TryParse(parts[1], out equipmentType);
                            }
                            else if (parts[0] == "reservedTimeSlot")
                            {
                                reservedTimeSlot = parts[1];
                            }

                        }
                        Equipment.equipmentList.Add(new Equipment(name, equipmentType, equipmentCategory, availability, reservedTimeSlot));
                    }
                }

                else if (fileName == "PersonalTrainer.txt")
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] values = line.Split(',');
                        string name = "";
                        PersonalTrainer.TrainerCategory trainerCategory = 0;
                        PersonalTrainer.Availability availability = 0;
                        foreach (string value in values)
                        {
                            string[] parts = value.Split(':');
                            if (parts[0] == "trainerCategory")
                            {
                                Enum.TryParse(parts[1], out trainerCategory);
                            }
                            else if (parts[0] == "name")
                            {
                                name = parts[1];
                            }
                            else if (parts[0] == "trainerAvailability")
                            {
                                Enum.TryParse(parts[1], out availability);
                            }
                        }
                        PersonalTrainer.personalTrainers.Add(new PersonalTrainer(name, trainerCategory, availability));
                    }
                }

                else if (fileName == "GroupActivity.txt")
                {                    
                    var lines = File.ReadAllLines(filePath);

                    foreach (var line in lines)
                    {
                        var personalTrainers = new List<PersonalTrainer>();
                        var equipment = new List<Equipment>();
                        var participants = new List<Customer>();
                        
                        var typeOfActivity = "";
                        var activtyId = 0;
                        var participantLimit = 0;
                        var timeSlot = "";
                        var space = new Space("");

                        var values = line.Split(',');

                        foreach (var value in values)
                        {
                            var key = "";
                            var val = "";
                            var val2 = "";
                            var val3 = "";
                            var parts = value.Split(':');
                            if (parts.Length == 2)
                            {
                                key = parts[0];
                                val = parts[1];                                                                
                            }
                            else if (parts.Length > 2)
                            {
                                key = parts[0];
                                val = parts[1];
                                val2 = parts[2];
                                val3 = parts[3];
                            }                               

                            switch (key)
                            {
                                case "typeOfActivity":
                                    typeOfActivity = val;
                                    break;
                                case "activtyId":
                                    activtyId = int.Parse(val);
                                    break;
                                case "participantLimit":
                                    participantLimit = int.Parse(val);
                                    break;
                                case "timeSlot":
                                    timeSlot = $"{val}:{val2}:{val3}";
                                    break;
                                case "space":
                                    space = Space.FindByName(val);
                                    break;
                                case "personalTrainer":
                                    var trainerNames = val.Split(';');
                                    foreach (var trainerName in trainerNames)
                                    {                                       
                                        var existingTrainer = personalTrainers.FirstOrDefault(t => t.name == trainerName);
                                        
                                        if (existingTrainer == null)
                                        {
                                            existingTrainer = new PersonalTrainer(trainerName);
                                            personalTrainers.Add(existingTrainer);
                                        }
                                    }
                                    break;
                                case "equipment":                                    
                                    var EQ = val.Split(';');
                                    foreach (var input in EQ)
                                    {                                       
                                        var existingEquipment = equipment.FirstOrDefault(t => t.name == input);
                                        
                                        if (existingEquipment == null)
                                        {
                                            existingEquipment = new Equipment(input);
                                            equipment.Add(existingEquipment);
                                        }
                                    }
                                    
                                    break;
                                case "participants":                                    

                                    string [] participantNames = new string[participantLimit];                                    
                                    
                                    if (val.Contains(";"))
                                    {
                                        participantNames = val.Split(';');                                       
                                    }
                                    else
                                    {
                                        participantNames[0] = val;
                                    }

                                    foreach (var participantName in participantNames)
                                    {
                                        
                                        if (participantNames.Length > 1)
                                        {
                                            participants.Add(new Customer(participantName, "1", "email"));
                                        }
                                        else if (participantName == "")
                                        {
                                            participants.Clear();
                                        }
                                    }
                                    break;
                            }
                        }
                        GroupSchedule.groupScheduleList.Add(new GroupActivity(
                                                                            personalTrainer: personalTrainers,
                                                                            typeOfActivity: typeOfActivity,
                                                                            activtyId: activtyId,
                                                                            participantLimit: participantLimit,
                                                                            timeSlot: timeSlot,
                                                                            participants: participants,
                                                                            space: space,
                                                                            equipment: equipment
                                                                            )
                        );
                    }
                }
            }
        }

        public void WriteFile<T>(List<T> objects, string fileName) where T : ICSVable
        {
            string filePath = Path.Combine("CSV", fileName);

            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    foreach (var obj in objects)
                    {
                        writer.WriteLine(obj.CSVify());
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (var obj in objects)
                    {
                        writer.WriteLine(obj.CSVify());
                    }
                }
            }
        }

        public static void CreateCSV()
        {
            if (!Directory.Exists("CSV"))
            {
                Directory.CreateDirectory("CSV");

                Space.spaceList.Add(new Space("Hall", Space.SpaceCategory.hall, Space.Availability.available));
                Space.spaceList.Add(new Space("Lane", Space.SpaceCategory.lane, Space.Availability.available));
                Space.spaceList.Add(new Space("Studio", Space.SpaceCategory.studio, Space.Availability.available));

                Equipment.equipmentList.Add(new Equipment("Treadmill", Equipment.EquipmentType.large, Equipment.EquipmentCategory.treadmill));
                Equipment.equipmentList.Add(new Equipment("TennisRacket", Equipment.EquipmentType.sport, Equipment.EquipmentCategory.tennis_racket));
                Equipment.equipmentList.Add(new Equipment("RowingMachine", Equipment.EquipmentType.large, Equipment.EquipmentCategory.rowing_machine));

                PersonalTrainer.personalTrainers.Add(new PersonalTrainer("Yanus Yoga", PersonalTrainer.TrainerCategory.yoga_instructor));
                PersonalTrainer.personalTrainers.Add(new PersonalTrainer("Gurra Gymbro", PersonalTrainer.TrainerCategory.gym_instructor));
                PersonalTrainer.personalTrainers.Add(new PersonalTrainer("Tomas Tennis", PersonalTrainer.TrainerCategory.tennis_teacher));

                GroupActivity temp = new GroupActivity(
                                PersonalTrainer.personalTrainers, //Personal Trainer
                                GroupSchedule.TypeOfActivity[0], //Type Of Activity
                                23, //Unique ID set to an random number. Is this needed?
                                1, //Particpant Limit
                                GroupSchedule.TimeSlot[1], //Time Slot
                                null, //List of Participants. This is not added here but rather under another menu-choice
                                Space.spaceList[0], //What space is used for this session
                                Equipment.equipmentList //What Equipment is used for this session
                                );

                GroupSchedule.groupScheduleList.Add(temp);


                CsvHandler csvHandler = new CsvHandler();
                csvHandler.WriteFile(Space.spaceList, "Spaces.txt");
                csvHandler.WriteFile(Equipment.equipmentList, "Equipment.txt");
                csvHandler.WriteFile(PersonalTrainer.personalTrainers, "PersonalTrainer.txt");
                csvHandler.WriteFile(GroupSchedule.groupScheduleList, "GroupActivity.txt");

                initialized = 1;
            }            
        }
    }
}
