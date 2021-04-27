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
        public void AddValues(string navn, string efternavn, string alder, string telefonnummer, string email, int område, string bio)
        {

            try
            {
                //vores connectionstring hentes fra Web.config
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
                connection.Open();
                string queryAddValue = "INSERT INTO Profil (Navn, Efternavn, Alder, Tlf, Email, Område, Bio) VALUES(@navn, @efternavn, @alder, @tlf, @email, @område, @bio);";
                SqlCommand cmd = new SqlCommand(queryAddValue, connection);

                cmd.Parameters.AddWithValue("@navn", navn);
                cmd.Parameters.AddWithValue("@efternavn", efternavn);
                cmd.Parameters.AddWithValue("@alder", alder);
                cmd.Parameters.AddWithValue("@tlf", telefonnummer);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@område", område);
                cmd.Parameters.AddWithValue("@bio", bio);
                cmd.ExecuteNonQuery();

                System.Diagnostics.Debug.WriteLine("Addvalues query Succesfull");

                connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
            }  
        }
        //Methode til at tilføje brugerinformationer til databasen via SqlClient
        public void AddUser(string navn, string efternavn, string email, string kodeord)
        {
            System.Diagnostics.Debug.WriteLine(navn);
            System.Diagnostics.Debug.WriteLine(efternavn);
            System.Diagnostics.Debug.WriteLine(email);
            System.Diagnostics.Debug.WriteLine(CreateHashSHA256(kodeord));


            try
            {
                //vores connectionstring hentes fra Web.config
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
                connection.Open();
                string queryAddUser = "INSERT INTO Profil (Navn, Efternavn, Email, Kodeord) VALUES(@navn, @efternavn, @email, @kodeord);";
                SqlCommand cmd = new SqlCommand(queryAddUser, connection);

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
            {
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

                int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                System.Diagnostics.Debug.WriteLine("Check Email Query Succesfull");
                connection.Close();

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

        public Profil GetProfil(string email) {
            try
            {
                var profil = new Profil();
                
                //vores connectionstring hentes fra Web.config
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Entities"].ConnectionString);
                connection.Open();
                string getprofil = "SELECT * FROM Profil WHERE Email='" + email + "'";
                SqlCommand cmd = new SqlCommand(getprofil, connection);
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
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
                        profil.Kodeord = "*****";
                    }
                }

                System.Diagnostics.Debug.WriteLine("GetProfil Query Succesfull");
                connection.Close();

                return profil;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR EXCEPTION:" + ex);
                return null;
            }
        }
    }
}

/*
 protected void Page_Load(object sender, EventArgs e)  
        {  
           if(IsPostBack)  
            {  
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegiConnectionString"].ConnectionString);  
                conn.Open();  
                string checkuser = "select count(*) from RegisterDataBase where StudentName='"+TextBox1.Text+"'";  
                SqlCommand cmd = new SqlCommand(checkuser, conn);  
                int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());  
  
                if (temp == 1)  
                {  
                    Response.Write("Student Already Exist");  
                }  
  
                conn.Close();  
            }  
               
            }  
  
        protected void Button1_Click(object sender, EventArgs e)  
        {  
            try  
            {  
  
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegiConnectionString"].ConnectionString);  
                conn.Open();  
                string insertQuery = "insert into RegisterDataBase(StudentName,Passwords,EmailId,Department,College)values (@studentname,@passwords,@emailid,@department,@college)";  
                SqlCommand cmd = new SqlCommand(insertQuery, conn);  
                cmd.Parameters.AddWithValue("@studentname", TextBox1.Text);  
                cmd.Parameters.AddWithValue("@passwords", TextBox2.Text);  
                cmd.Parameters.AddWithValue("@emailid", TextBox3.Text);  
                cmd.Parameters.AddWithValue("@department", TextBox4.Text);  
                cmd.Parameters.AddWithValue("@college", TextBox5.Text);  
                cmd.ExecuteNonQuery();  
  
                Response.Write("Student registeration Successfully!!!thank you");  
  
                conn.Close();  
  
            }  
            catch (Exception ex)  
            {  
                Response.Write("error" + ex.ToString());  
            }  
        }  
    }  
} 
 */