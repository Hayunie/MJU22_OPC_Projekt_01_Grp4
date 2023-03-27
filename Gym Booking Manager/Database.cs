using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using static Gym_Booking_Manager.Equipment;
using static Gym_Booking_Manager.Space;
using static Gym_Booking_Manager.PersonalTrainer;

namespace Gym_Booking_Manager
{
	internal class Database
	{
		public static void ReadData(string table)
		{
			var cs = "Host=localhost;Username=postgres;Password=123;Database=Gymbooking";
			using var con = new NpgsqlConnection(cs);
			con.Open();

			string sql = $"SELECT * FROM {table}";
			using var cmd = new NpgsqlCommand(sql, con);
			using NpgsqlDataReader rdr = cmd.ExecuteReader();

			if (table.ToLower() == "equipment")
			{
				while (rdr.Read())
				{
					Equipment equipment = new Equipment();

					var categoryValue = rdr.GetString(rdr.GetOrdinal("equipmentcategory"));
					equipment.equipmentCategory = (EquipmentCategory)Enum.Parse(typeof(EquipmentCategory), categoryValue);

					var typeValue = rdr.GetString(rdr.GetOrdinal("equipmenttype"));
					equipment.equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), typeValue);

					equipment.name = rdr.GetString(rdr.GetOrdinal("name"));

					var availabilityValue = rdr.GetString(rdr.GetOrdinal("equipmentavailability"));
					equipment.equipmentAvailability = (Equipment.Availability)Enum.Parse(typeof(Equipment.Availability), availabilityValue);

					Equipment.equipmentList.Add(equipment);
				}
			}
			else if (table.ToLower() == "spaces")
			{
				while (rdr.Read())
				{
					Space space = new Space();

					var categoryValue = rdr.GetString(rdr.GetOrdinal("spacecategory"));
					space.spaceCategory = (SpaceCategory)Enum.Parse(typeof(SpaceCategory), categoryValue);

					space.name = rdr.GetString(rdr.GetOrdinal("name"));

					var availabilityValue = rdr.GetString(rdr.GetOrdinal("spaceavailability"));
					space.spaceAvailability = (Space.Availability)Enum.Parse(typeof(Space.Availability), availabilityValue);

					Space.spaceList.Add(space);
				}
			}
			else if(table.ToLower() == "personaltrainer")
			{
				while (rdr.Read())
				{
					PersonalTrainer trainer = new PersonalTrainer();

					var categoryValue = rdr.GetString(rdr.GetOrdinal("trainercategory"));
					trainer.trainerCategory = (TrainerCategory)Enum.Parse(typeof(TrainerCategory), categoryValue);

					trainer.name = rdr.GetString(rdr.GetOrdinal("name"));

					var availabilityValue = rdr.GetString(rdr.GetOrdinal("traineravailability"));
					trainer.trainerAvailability = (PersonalTrainer.Availability)Enum.Parse(typeof(PersonalTrainer.Availability), availabilityValue);

					PersonalTrainer.personalTrainers.Add(trainer);
				}
			}
			else if(table.ToLower() == "groupactivity")
			{
				while (rdr.Read())
				{
					GroupActivity activity = new GroupActivity();

					string trainerString = rdr.GetString(rdr.GetOrdinal("personalTrainer"));
					string[] trainerNames = trainerString.Split(';');
					var personalTrainers = new List<PersonalTrainer>();
					foreach (string trainerName in trainerNames)
					{
						var existingTrainer = personalTrainers.FirstOrDefault(t => t.name == trainerName);
						
						if (existingTrainer == null)
						{
							existingTrainer = new PersonalTrainer(trainerName);
							personalTrainers.Add(existingTrainer);
						}
					}
					activity.personalTrainer = personalTrainers;

					activity.typeOfActivity = rdr.GetString(rdr.GetOrdinal("typeOfActivity"));
					activity.activtyId = rdr.GetInt32(rdr.GetOrdinal("activtyId"));
					activity.participantLimit = rdr.GetInt32(rdr.GetOrdinal("participantLimit"));
					activity.timeSlot = rdr.GetString(rdr.GetOrdinal("timeSlot"));

					string participantsString = rdr.GetString(rdr.GetOrdinal("participants"));
					string[] participantNames = new string[activity.participantLimit];
					if (participantsString.Contains(";"))
					{
						participantNames = participantsString.Split(';');
					}
					else
					{
						participantNames[0] = participantsString;
					}
					var participants = new List<Customer>();
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
					activity.participants = participants;

					string spaceString = rdr.GetString(rdr.GetOrdinal("space"));
					var newSpace = new Space(spaceString);
					activity.space = newSpace;

					var equipment = new List<Equipment>();
					string EQString = rdr.GetString(rdr.GetOrdinal("equipment"));
					var EQ = EQString.Split(';');
					foreach (var input in EQ)
					{
						var existingEquipment = equipment.FirstOrDefault(t => t.name == input);

						if (existingEquipment == null)
						{
							existingEquipment = new Equipment(input);
							equipment.Add(existingEquipment);
						}
					}
					activity.equipment = equipment;

					GroupSchedule.groupScheduleList.Add(activity);
				}
			}
		}
		public void WriteData(string tableName)
		{
			var connectionString = "Host=localhost;Username=postgres;Password=123;Database=Gymbooking";
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			using var cmd = new NpgsqlCommand();
			cmd.Connection = connection;

			// drop table if exists
			cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
			cmd.ExecuteNonQuery();

			if (tableName == "Equipment")
			{
				cmd.CommandText = @"CREATE TABLE Equipment(equipmentCategory equipment_category, equipmentType equipment_type, name TEXT, equipmentAvailability availability)";
				cmd.ExecuteNonQuery();

				foreach (Equipment e in Equipment.equipmentList)
				{

					cmd.CommandText = $"INSERT INTO Equipment(equipmentCategory, equipmentType, name, equipmentAvailability) VALUES (@equipmentCategory, @equipmentType, @name, @equipmentAvailability)";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@equipmentCategory", e.equipmentCategory);
					cmd.Parameters.AddWithValue("@equipmentType", e.equipmentType);
					cmd.Parameters.AddWithValue("@name", e.name);
					cmd.Parameters.AddWithValue("@equipmentAvailability", e.equipmentAvailability);
					cmd.ExecuteNonQuery();				
				}
			}
			else if (tableName == "Spaces")
			{
				cmd.CommandText = @"CREATE TABLE Spaces(spaceCategory space_category, name TEXT, spaceAvailability availability)";
				cmd.ExecuteNonQuery();

				foreach (Space s in Space.spaceList)
				{

					cmd.CommandText = "INSERT INTO Spaces (spaceCategory, name, spaceAvailability) VALUES (@spaceCategory, @name, @spaceAvailability)";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@spaceCategory", s.spaceCategory);
					cmd.Parameters.AddWithValue("@name", s.name);
					cmd.Parameters.AddWithValue("@spaceAvailability", s.spaceAvailability);
					cmd.ExecuteNonQuery();

				}
			}
			else if (tableName == "PersonalTrainer")
			{
				cmd.CommandText = @"CREATE TABLE PersonalTrainer(trainerCategory trainer_category, name TEXT, trainerAvailability availability)";
				cmd.ExecuteNonQuery();

				foreach (PersonalTrainer p in PersonalTrainer.personalTrainers)
				{

					cmd.CommandText = "INSERT INTO PersonalTrainer(trainerCategory, name, trainerAvailability) VALUES (@trainerCategory, @name, @trainerAvailability)";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@trainerCategory", p.trainerCategory);
					cmd.Parameters.AddWithValue("@name", p.name);
					cmd.Parameters.AddWithValue("@trainerAvailability", p.trainerAvailability);
					cmd.ExecuteNonQuery();

				}
			}
			else if (tableName == "GroupActivity")
			{
				cmd.CommandText = @"CREATE TABLE GroupActivity(personalTrainer TEXT, typeOfActivity TEXT, activtyId INT, participantLimit INT, timeSlot TEXT, participants TEXT, space TEXT, equipment TEXT )";
				cmd.ExecuteNonQuery();

				foreach (GroupActivity g in GroupSchedule.groupScheduleList)
				{
					cmd.CommandText = "INSERT INTO GroupActivity(personalTrainer, typeOfActivity, activtyId, participantLimit, timeSlot, participants, space, equipment) " +
						"VALUES (@personalTrainer, @typeOfActivity, @activtyId, @participantLimit, @timeSlot, @participants, @space, @equipment)";

					string trainers = string.Join(",", g.personalTrainer.Select(t => t.name));
					string equipment = string.Join(",", g.equipment.Select(e => e.name));
					string participants = string.Join(",", g.participants.Select(p => p.name));

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@personalTrainer", trainers);
					cmd.Parameters.AddWithValue("@typeOfActivity", g.typeOfActivity);
					cmd.Parameters.AddWithValue("@activtyId", g.activtyId);
					cmd.Parameters.AddWithValue("@participantLimit", g.participantLimit);
					cmd.Parameters.AddWithValue("@timeSlot", g.timeSlot);
					cmd.Parameters.AddWithValue("@participants", participants);
					cmd.Parameters.AddWithValue("@space", g.space.ToString());
					cmd.Parameters.AddWithValue("@equipment", equipment);
					cmd.ExecuteNonQuery();
				}
			}

		}
		public void CreateDatabase()
		{
			var cs = "Host=localhost;Username=postgres;Password=123;Database=Gymbooking";
			using var con = new NpgsqlConnection(cs);
			con.Open();

			using var cmd = new NpgsqlCommand();
			cmd.Connection = con;

			// Types
			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE equipment_category AS ENUM('treadmill', 'tennis_racket', 'rowing_machine');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE equipment_category AS ENUM('Treadmill', 'TennisRacket', 'RowingMachine')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE equipment_type AS ENUM('large', 'sport');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE equipment_type AS ENUM('Large', 'Sport')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE space_category AS ENUM('hall', 'lane', 'studio');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE space_category AS ENUM('Hall', 'Lane', 'Studio')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE trainer_category AS ENUM('yoga_instructor', 'gym_instructor', 'tennis_teacher');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE trainer_category AS ENUM('YogaInstructor', 'GymInstructor', 'TennisTeacher')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE availability AS ENUM('available', 'service', 'planned_purchase', 'reserved', 'unavailable');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE availability AS ENUM('Available', 'Service', 'PlannedPurchase', 'Reserved', 'Unavailable')";
			cmd.ExecuteNonQuery();

			// Equipment
			cmd.CommandText = "CREATE TABLE IF NOT EXISTS Equipment(equipmentCategory equipment_category, equipmentType equipment_type, name TEXT, equipmentAvailability availability)";
			cmd.ExecuteNonQuery();

			// Spaces
			cmd.CommandText = "CREATE TABLE IF NOT EXISTS Spaces(spaceCategory space_category, name TEXT, spaceAvailability availability)";
			cmd.ExecuteNonQuery();

			// Personal Trainers
			cmd.CommandText = "CREATE TABLE IF NOT EXISTS PersonalTrainer(trainerCategory trainer_category, name TEXT, trainerAvailability availability)";
			cmd.ExecuteNonQuery();

			// Group activity
			cmd.CommandText = "CREATE TABLE IF NOT EXISTS GroupActivity(personalTrainer TEXT, typeOfActivity TEXT, activtyId INT, participantLimit INT, timeSlot TEXT, participants TEXT, space TEXT, equipment TEXT )";
			cmd.ExecuteNonQuery();


		}
		public void ResetDatabase()
		{
			var cs = "Host=localhost;Username=postgres;Password=123;Database=Gymbooking";
			using var con = new NpgsqlConnection(cs);
			con.Open();

			using var cmd = new NpgsqlCommand();
			cmd.Connection = con;

			cmd.CommandText = "DROP TABLE IF EXISTS Equipment";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DROP TABLE IF EXISTS Spaces";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DROP TABLE IF EXISTS PersonalTrainer";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "DROP TABLE IF EXISTS GroupActivity";
			cmd.ExecuteNonQuery();


			// Types
			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE equipment_category AS ENUM('treadmill', 'tennis_racket', 'rowing_machine');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE equipment_category AS ENUM('Treadmill', 'TennisRacket', 'RowingMachine')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE equipment_type AS ENUM('large', 'sport');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE equipment_type AS ENUM('Large', 'Sport')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE space_category AS ENUM('hall', 'lane', 'studio');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE space_category AS ENUM('Hall', 'Lane', 'Studio')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE trainer_category AS ENUM('yoga_instructor', 'gym_instructor', 'tennis_teacher');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE trainer_category AS ENUM('YogaInstructor', 'GymInstructor', 'TennisTeacher')";
			cmd.ExecuteNonQuery();

			cmd.CommandText = @"
				DO $$
				BEGIN
			CREATE TYPE availability AS ENUM('available', 'service', 'planned_purchase', 'reserved', 'unavailable');
			EXCEPTION
				WHEN duplicate_object THEN null;
			END $$";
			//cmd.CommandText = "CREATE TYPE availability AS ENUM('Available', 'Service', 'PlannedPurchase', 'Reserved', 'Unavailable')";
			cmd.ExecuteNonQuery();


			// Equipment
			cmd.CommandText = @"CREATE TABLE Equipment(equipmentCategory equipment_category, equipmentType equipment_type, name TEXT, equipmentAvailability availability)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Equipment (equipmentCategory, equipmentType, name, equipmentAvailability) " +
				"VALUES (@equipmentCategory, @equipmentType, @name, @equipmentAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@equipmentCategory", EquipmentCategory.rowing_machine);
			cmd.Parameters.AddWithValue("@equipmentType", EquipmentType.large);
			cmd.Parameters.AddWithValue("@name", "RowingMachine");
			cmd.Parameters.AddWithValue("@equipmentAvailability", Equipment.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Equipment (equipmentCategory, equipmentType, name, equipmentAvailability) " +
				"VALUES (@equipmentCategory, @equipmentType, @name, @equipmentAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@equipmentCategory", EquipmentCategory.tennis_racket);
			cmd.Parameters.AddWithValue("@equipmentType", EquipmentType.sport);
			cmd.Parameters.AddWithValue("@name", "TennisRacket");
			cmd.Parameters.AddWithValue("@equipmentAvailability", Equipment.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Equipment (equipmentCategory, equipmentType, name, equipmentAvailability) " +
				"VALUES (@equipmentCategory, @equipmentType, @name, @equipmentAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@equipmentCategory", EquipmentCategory.treadmill);
			cmd.Parameters.AddWithValue("@equipmentType", EquipmentType.large);
			cmd.Parameters.AddWithValue("@name", "Treadmill1");
			cmd.Parameters.AddWithValue("@equipmentAvailability", Equipment.Availability.available);
			cmd.ExecuteNonQuery();


			// Spaces
			cmd.CommandText = @"CREATE TABLE Spaces(spaceCategory space_category, name TEXT, spaceAvailability availability)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Spaces (spaceCategory, name, spaceAvailability)" +
			"VALUES(@spaceCategory, @name, @spaceAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@spaceCategory", SpaceCategory.hall);
			cmd.Parameters.AddWithValue("@name", "Hall");
			cmd.Parameters.AddWithValue("@spaceAvailability", Space.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Spaces (spaceCategory, name, spaceAvailability)" +
			"VALUES(@spaceCategory, @name, @spaceAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@spaceCategory", SpaceCategory.lane);
			cmd.Parameters.AddWithValue("@name", "Lane");
			cmd.Parameters.AddWithValue("@spaceAvailability", Space.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Spaces (spaceCategory, name, spaceAvailability)" +
			"VALUES(@spaceCategory, @name, @spaceAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@spaceCategory", SpaceCategory.studio);
			cmd.Parameters.AddWithValue("@name", "Studio");
			cmd.Parameters.AddWithValue("@spaceAvailability", Space.Availability.available);
			cmd.ExecuteNonQuery();

			// Personal Trainers
			cmd.CommandText = @"CREATE TABLE PersonalTrainer(trainerCategory trainer_category, name TEXT, trainerAvailability availability)";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO PersonalTrainer (trainerCategory, name, trainerAvailability)" +
				"VALUES(@trainerCategory, @name, @trainerAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@trainerCategory", TrainerCategory.yoga_instructor);
			cmd.Parameters.AddWithValue("@name", "Yanus Yoga");
			cmd.Parameters.AddWithValue("@trainerAvailability", PersonalTrainer.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO PersonalTrainer (trainerCategory, name, trainerAvailability)" +
				"VALUES(@trainerCategory, @name, @trainerAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@trainerCategory", TrainerCategory.gym_instructor);
			cmd.Parameters.AddWithValue("@name", "Gurra Gymbro");
			cmd.Parameters.AddWithValue("@trainerAvailability", PersonalTrainer.Availability.available);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO PersonalTrainer (trainerCategory, name, trainerAvailability)" +
				"VALUES(@trainerCategory, @name, @trainerAvailability)";

			cmd.Parameters.Clear();
			cmd.Parameters.AddWithValue("@trainerCategory", TrainerCategory.tennis_teacher);
			cmd.Parameters.AddWithValue("@name", "Tomas Tennis");
			cmd.Parameters.AddWithValue("@trainerAvailability", PersonalTrainer.Availability.available);
			cmd.ExecuteNonQuery();

			// Group activity
			cmd.CommandText = @"CREATE TABLE GroupActivity(personalTrainer TEXT, typeOfActivity TEXT, activtyId INT, participantLimit INT, timeSlot TEXT, participants TEXT, space TEXT, equipment TEXT )";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO GroupActivity " +
				"VALUES('Yanus Yoga;Gurra Gymbro;Tomas Tennis', 'Spinning Class', 23, 1, '13:00-14:00', '', 'Hall', 'Treadmill;TennisRacket;RowingMachine')";
			cmd.ExecuteNonQuery();

		}
	}
}
