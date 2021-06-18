using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;

namespace vikar_app
{
	public class DataHandler
	{
		//Methode til hashing af passwords

		private string CreateHashSHA256(string passwordRaw)
		{
			//Vi gør brug af SHA256 klassen fra System.Security.Cryptography
			using (SHA256 hash = SHA256.Create())
			{
				byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(passwordRaw));

				StringBuilder strBuilder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					//Brug af stringbuilder til at opbygge vores hash som en string hvor
					//Tostring får formatet x2, som er hexidecimaler
					strBuilder.Append(bytes[i].ToString("x2"));
				}
				return strBuilder.ToString();
			}
		}

		//denne funktion redigerer i databasen og indsætter værdierne fra rediger-profil-siden
		public void AddValues(string fnavn, string enavn, int alder, int tlf, string email, int område, string bio)
		{
			//først har vi en try-catch, for at fange eventuelle fejl
			try
			{
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);

				//vi åbner for forbindelsen
				connection.Open();
				
				//Her er vores query. Queryen er lavet til at opdatere felderne, hvor emailen passer til den indsendte email.
				string queryAddValue =
					"UPDATE Profil SET Navn = @navn, Efternavn = @enavn, Alder = @alder, Tlf = @tlf, Område = @område, Bio = @bio WHERE Email='" + email + "'";

				//Herefter lavet vi et SQL-command object med vores query og connection
				SqlCommand cmd = new SqlCommand(queryAddValue, connection);

				//her indsætter vi argumenterne fra funktionen ind i pladsholderne i queryen.
				cmd.Parameters.AddWithValue("@navn", fnavn);
				cmd.Parameters.AddWithValue("@enavn", enavn);
				cmd.Parameters.AddWithValue("@alder", alder);
				cmd.Parameters.AddWithValue("@tlf", tlf);
				cmd.Parameters.AddWithValue("@område", område);
				cmd.Parameters.AddWithValue("@bio", bio);

				//her executer vi queryen
				cmd.ExecuteNonQuery();

				//her skriver vi til konsollen for at vise, at queryen var succesfuld
				System.Diagnostics.Debug.WriteLine("Addvalues query Succesfull");

				//til sidst lukker vi for forbindelsen
				connection.Close();
			}
			catch (Exception ex)
			{
				//hvis vi får en fejl, bliver den printet i konsollen
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
			}  
		}
		
		//Metode til at tilføje brugerinformationer til databasen via SqlClient
		public void AddUser(string navn, string efternavn, string email, string kodeord)
		{
			try
			{
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
				connection.Open();
				string queryAddUser = "INSERT INTO Profil (Navn, Efternavn, Email, Kodeord) VALUES(@navn, @efternavn, @email, @kodeord);";
				SqlCommand cmd = new SqlCommand(queryAddUser, connection);

				//Her bruger vi vores HASH-metode, som vi har skrevet til at kryptere kodeordet.
				string passwordHash = CreateHashSHA256(kodeord);

				cmd.Parameters.AddWithValue("@navn", navn);
				cmd.Parameters.AddWithValue("@efternavn", efternavn);
				cmd.Parameters.AddWithValue("@email", email);
				cmd.Parameters.AddWithValue("@kodeord", passwordHash);
				cmd.ExecuteNonQuery();

				System.Diagnostics.Debug.WriteLine("Signup query Succesfull");

				connection.Close();
			}
			catch (Exception ex)
			{	//Print eventuelle fejl ud i Debug konsollen
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
			}
		}

		//Her har vi en metode, som tjekker om en email findes i systemet allerede. Vi bruger den til at sørge for, at der ikke bliver lavet 2 brugere med samme email.
		public bool CheckEmail(string email)
		{
			try
			{
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
				connection.Open();
				string checkuser = "SELECT count(*) FROM Profil WHERE Email='" + email + "'";
				SqlCommand cmd = new SqlCommand(checkuser, connection);

				//vi tjekker om der findes en matchende email ved at bruge metoden ExecuteScalar, som returnerer antal hits i databasen. Denne laver vi om til en int
				int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());

				System.Diagnostics.Debug.WriteLine("Check Email Query Succesfull");
				connection.Close();

				//Temp er antal matchende emails
				//Da emails i databasen er unikke, kan der maks være én
				//Hvis det er, returnerer metoden true, som svarer til at der allerede findes en mail 
				if (temp == 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				//her fanger vi eventuelle fejl og skriver dem ud i konsollen
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
				return true;
			}
		}

		public bool CheckLogin(string email, string password)
		{
			try
			{
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
				connection.Open();
				string passwordHash = CreateHashSHA256(password);
				string checkuser = "SELECT count(*) FROM Profil WHERE Email='" + email + "' AND Kodeord='" + passwordHash + "'";
				SqlCommand cmd = new SqlCommand(checkuser, connection);

				int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());

				System.Diagnostics.Debug.WriteLine("Check Login Query Succesfull");
				connection.Close();

				//Her er temp antal matchende logins
				//Igen kan der højst være én
				if (temp == 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
				return true;
			}
		}

		//Metode til at få fat på alle informationer om en profil på baggrund af en email
		public Profil GetProfil(string email) {
			try
			{
				//Her laver vi et objekt af typen Profil, som svarer til samme type som profil-tablet i vores database
				var profil = new Profil();
				
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
				connection.Open();
				
				//her er vores query, som selecter alt fra profil-tablet, hvor emailen passer.
				string getprofil = "SELECT * FROM Profil WHERE Email='" + email + "'";

				SqlCommand cmd = new SqlCommand(getprofil, connection);
				
				//herefter bruger vi SQLDataReader til at indsætte alle dataene fra queryen ind i objektet fra før.
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						profil.Bruger_Id = reader.GetInt32(0);
						profil.Navn = reader.GetString(1);
						profil.Efternavn = reader.GetString(2);
						//Ved nogle af dataene tjekker vi om de er "null" inden vi forsøger at lave dem om til strings eller ints, 
						//da GetInt32 og GetString fejler, hvis de får "null" som input, som bare betyder at felterne er tomme.
						if (!Convert.IsDBNull(reader[3])) profil.Alder = reader.GetInt32(3);
						if (!Convert.IsDBNull(reader[4])) profil.Køn = reader.GetInt32(4);
						if (!Convert.IsDBNull(reader[5])) profil.Område = reader.GetInt32(5);
						if (!Convert.IsDBNull(reader[6])) profil.Bio = reader.GetString(6);
						if (!Convert.IsDBNull(reader[7])) profil.Profilbillede = (byte[])reader[7];
						if (!Convert.IsDBNull(reader[8])) profil.Tlf = reader.GetString(8);
						profil.Email = reader.GetString(9);
						profil.Kodeord = "*****";
					}
				}

				System.Diagnostics.Debug.WriteLine("GetProfil Query Succesfull");
				connection.Close();

				return profil;
			}
			catch (Exception ex)
			{
				//hvis der er en fejl, bliver den vist i konsollen
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
				return null;
			}
		}

		//Metode til at søge på registrerede vikarere
		public List<Profil> SearchByName(string navn)
		{
			try
			{
				List<Profil> profiler = new List<Profil>();
				
				//vores connectionstring hentes fra Web.config
				SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
				connection.Open();

				//Hvis ikke man har udfyldt søgefeltet, hentes de første 100 profiler i databasen
				//get profil er SQL querien til at hente profiler
				string getprofil;
				if (navn.Length <= 0)
                {
				getprofil = "SELECT TOP 100 * FROM Profil";
                } else
                {	//Vi henter alle profiler med matchende navn eller efternavn
					//Ves brug af like-operatoren i SQL, henter vi profiler, der indeholder navnet der søges efter
				getprofil = "SELECT * FROM Profil WHERE Navn='" + navn + "' OR Efternavn='" + navn + "' OR Navn LIKE '%" + navn + "%' OR Efternavn LIKE '%" + navn + "%'";
				}
				
				SqlCommand cmd = new SqlCommand(getprofil, connection);

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var profil = new Profil();

						profil.Bruger_Id = reader.GetInt32(0);
						profil.Navn = reader.GetString(1);
						profil.Efternavn = reader.GetString(2);
						if (!Convert.IsDBNull(reader[3])) profil.Alder = reader.GetInt32(3);
						if (!Convert.IsDBNull(reader[4])) profil.Køn = reader.GetInt32(4);
						if (!Convert.IsDBNull(reader[5])) profil.Område = reader.GetInt32(5);
						if (!Convert.IsDBNull(reader[6])) profil.Bio = reader.GetString(6);
						if (!Convert.IsDBNull(reader[7])) profil.Profilbillede = (byte[])reader[7];
						if (!Convert.IsDBNull(reader[8])) profil.Tlf = reader.GetString(8);
						profil.Email = reader.GetString(9);

						profiler.Add(profil);
					}
				}

				System.Diagnostics.Debug.WriteLine("GetProfil Query Succesfull");
				connection.Close();

				return profiler;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
				return null;
			}
		}
	}
}