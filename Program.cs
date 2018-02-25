using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TDMySQL
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnect(string s, string db, string u, string pw)
        {
            Initialize(s, db, u, pw);
        }

        private void Initialize(string s, string db, string u, string pw)
        {
            server = s;
            database = db;
            uid = u;
            password = pw;
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE="
                + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch(ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server");
                        break;
                    case 1:
                        Console.WriteLine("Invalid username/password");
                        break;
                }
                return false;
            }
        }

        //close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //insert
        public void Insert(string tableau, string value)
        {

            string query = "INSERT INTO " + tableau + " VALUES "+value;
            //string query = "INSERT INTO detail(NCOM, NPRO, QCOM) VALUES (30199, \"T4242\", 56)";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //update
        public void Update(string tableau, string value, string where)
        {
            string query = "UPDATE " + tableau + " SET " + value + " WHERE " + where; 
            //string query = "UPDATE detail SET NCOM=30190,NPRO=\"C4242\",QCOM=37 WHERE NCOM=30199";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;

                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //delete
        public void Delete(string tableau, string value)
        {
            string query = "DELETE FROM "+tableau+" WHERE " + value;
            //string query = "DELETE FROM detail where NCOM=30188";

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //select
        public List<string>[] Select(string tableau, string value)
        {
            string query = "SELECT " + value + " FROM " + tableau;
            //string query = "SELECT * FROM detail";

            //List to store the result. Size = columns
            List<string>[] list = new List<string>[6];
            for (int i = 0; i < 6; i++)
                list[i] = new List<string>();

            if (this.OpenConnection())
            {
                //create and execute the command to get the information from reader
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //read the data and store it in the list
                switch (tableau)
                {
                    case "detail":
                        Console.Write("Detail");
                        while (dataReader.Read())
                        {
                            list[0].Add(dataReader["NCOM"].ToString());
                            list[1].Add(dataReader["NPRO"].ToString());
                            list[2].Add(dataReader["QCOM"].ToString());
                        }
                        break;
                    case "client":
                        while (dataReader.Read())
                        {
                            list[0].Add(dataReader["NCLI"].ToString());
                            list[1].Add(dataReader["NOM"].ToString());
                            list[2].Add(dataReader["ADRESSE"].ToString());
                            list[3].Add(dataReader["LOCALITE"].ToString());
                            list[4].Add(dataReader["(CAT)"].ToString());
                            list[5].Add(dataReader["COMPTE"].ToString());
                        }
                        break;
                    case "commande":
                        while (dataReader.Read())
                        {
                            list[0].Add(dataReader["NCOM"].ToString());
                            list[1].Add(dataReader["NCLI"].ToString());
                            list[2].Add(dataReader["DATECOM"].ToString());
                        }
                        break;
                    case "produit":
                        while (dataReader.Read())
                        {
                            list[0].Add(dataReader["NPRO"].ToString());
                            list[1].Add(dataReader["LIBELLE"].ToString());
                            list[2].Add(dataReader["PRIX"].ToString());
                            list[3].Add(dataReader["QSTOCK"].ToString());
                        }
                        break;
                }
                

                //close data reader and connection
                dataReader.Close();
                this.CloseConnection();
                return list;

            } else
            {
                return list;
            }
        }


    }

    class Program
    {
        static void Main(string[] args)
        {
            DBConnect db = new DBConnect("localhost", "clicom", "root", "");

            //Client list
            List<string>[] clients = db.Select("client", "*");
            Console.WriteLine("Liste des clients:");
            Console.WriteLine("NCLI     NOM        ADRESSE      LOCALITE  (CAT)  COMPTE");
            int columns = 6;
            for (int i = 0; i < clients[0].Count-1; i++)
            {
                int j;
                for (j = 0; j<columns; j++)
                {
                    Console.Write(clients[j][i].ToString() + "   ");
                }
                Console.Write('\n');
                j = 0;
            }
            
            //Product list
            List<string>[] produits = db.Select("produit", "*");
            Console.WriteLine("Liste des produits:");
            Console.WriteLine("NPRO        LIBELLE          PRIX      QSTOCK");
            columns = 4;
            for (int i = 0; i < produits[0].Count-1; i++)
            {
                int j;
                for (j = 0; j< columns; j++)
                {
                    Console.Write(produits[j][i].ToString() + "   ");
                }
                Console.Write('\n');
                j = 0;
            }

            //Order list
            List<string>[] commandes = db.Select("commande", "*");
            Console.WriteLine("Liste des commandes:");
            Console.WriteLine("NCLI     NCLI        DATECOM");
            columns = 3;
            for (int i = 0; i < commandes[0].Count - 1; i++)
            {
                int j;
                for (j = 0; j < columns; j++)
                {
                    Console.Write(commandes[j][i].ToString() + "   ");
                }
                Console.Write('\n');
                j = 0;
            }

            //Adding, modifying, deleting a client. Comment/Uncomment to test
            //db.Insert("`client`(`NCLI`, `NOM`, `ADRESSE`, `LOCALITE`, `(CAT)`, `COMPTE`)",
            //"(\"A042\", \"TOTO\", \"42 rue de la Rue\", \"Ville\", \"\", 420)");
            //db.Update("`client`", "`NCLI`=\"B420\"", "`NCLI`=\"A042\"");
            //db.Delete("`client`", "`NOM`=\"TOTO\"");

            //Adding, modifying, deleting a product. Comment/Uncomment to test
            //db.Insert("`produit`(`NPRO`, `LIBELLE`, `PRIX`, `QSTOCK`)",
            //"(\"A042\", \"TOTO\", 3, 420)");
            //db.Update("`produit`", "`NPRO`=\"B420\"", "`NPRO`=\"A042\"");
            //db.Delete("`produit`", "`LIBELLE`=\"TOTO\"");

            //Adding, modifying, deleting an order. Comment/Uncomment to test
            //db.Insert("`commande`(`NCOM`, `NCLI`, `DATECOM`)",
            //"(30190, \"A042\", NOW())");
            //db.Update("`commande`", "`NCLI`=\"B420\"", "`NCLI`=\"A042\"");
            //db.Delete("`commande`", "`NCLI`=\"B420\"");

            System.Threading.Thread.Sleep(5000);
        }
    }
}
