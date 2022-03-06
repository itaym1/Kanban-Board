using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DalController
    {
        const string DBNAME = "KanbanDatabase.sqlite";
        /// <summary>
        /// Creates a new DalController and initiates the database
        /// </summary>
        public DalController()
        {
            initDataBase();
        }
        /// <summary>
        /// Loads all users from the users table in the database
        /// </summary>
        /// <returns>List of loaded users</returns>
        public List<User> LoadUsers()
        {
            List<User> list = new List<User>();
            string query = "SELECT * FROM users";
            SQLiteConnection con = getConnection();
            con.Open();
            SQLiteDataReader d = runQueryWithReturn(query, con);
            while (d.Read())
            {
                User user = new User(d["Email"].ToString(), d["Nickname"].ToString(), d["Password"].ToString());
                list.Add(user);
            }
            con.Close();
            return list;
        }
        /// <summary>
        /// Loads all columns and tasks and creates boards out of them by email
        /// </summary>
        /// <returns>A list of loaded boards</returns>
        public List<Board> LoadBoards()
        {
            List<Board> list = new List<Board>();
            string query = "SELECT Email FROM users";
            SQLiteConnection con = getConnection();
            con.Open();
            SQLiteDataReader userReader = runQueryWithReturn(query, con);
            while (userReader.Read())
            {
                Board board = new Board(userReader, this, con);
                list.Add(board);
            }
            con.Close();
            return list;
        }

        /// <summary>
        /// Initiates the database if it isn't intiated already by creating the database 
        /// and creating the needed tables.
        /// </summary>
        public void initDataBase()
        {
            if (!System.IO.File.Exists(DBNAME))
            {
                SQLiteConnection.CreateFile(DBNAME);
            }
            //Create columns table
            string query1 = "CREATE TABLE IF NOT EXISTS `Columns` (`id`	INTEGER,`UserEmail`	TEXT,`Name`	TEXT,`MaxTasks`	INTEGER,`Position`	INTEGER,PRIMARY KEY(`id`)); ";

            //Create tasks table
            string query2 = "CREATE TABLE IF NOT EXISTS `Tasks` (`id`	INTEGER,`ColumnId`	INTEGER,`Title`	TEXT,`Description`	TEXT,`CreationDate`	TEXT,`DueDate`	TEXT,PRIMARY KEY(`id`)); ";

            //Create users table
            string query3 = "CREATE TABLE IF NOT EXISTS `Users` (`Email`	TEXT,`Nickname`	TEXT,`Password`	TEXT,PRIMARY KEY(`Email`));";

            runQueries(new string[] { query1, query2, query3 });
        }

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns>The connection object to the database</returns>
        public SQLiteConnection getConnection()
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", DBNAME));
            return m_dbConnection;
        }

        /// <summary>
        /// Recieves a database command and a connection and runs the command using that connection
        /// </summary>
        /// <param name="query">The command to run on the database</param>
        /// <param name="con">The connection to use to run the command with</param>
        public void runQuery(string query, SQLiteConnection con)
        {
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a connection and executes a command with that connection
        /// </summary>
        /// <param name="query">The command to run</param>
        public void runQuery(string query)
        {
            SQLiteConnection con = getConnection();
            con.Open();
            runQuery(query, con);
            con.Close();
        }

        /// <summary>
        /// Recieves a query and a connection, runs the query and returns its results.
        /// </summary>
        /// <param name="query">The query to run</param>
        /// <param name="con">The connection to use</param>
        /// <returns>The result of executing the query</returns>
        public SQLiteDataReader runQueryWithReturn(string query, SQLiteConnection con)
        {
            return (new SQLiteCommand(query, con)).ExecuteReader();
        }


        /// <summary>
        /// Recieves an array of commands and executes them.
        /// </summary>
        /// <param name="queries">The commands to execute</param>
        public void runQueries(string[] queries)
        {
            SQLiteConnection con = getConnection();
            con.Open();

            foreach (string query in queries)
            {
                runQuery(query, con);
            }

            con.Close();
        }

        /// <summary>
        /// Deletes all users from the database
        /// </summary>
        public void deleteAllUsers()
        {
            runQuery("DELETE FROM users");
        }

        /// <summary>
        /// Deletes all boards, columns and tasks from the database.
        /// </summary>
        public void deleteAllBoards()
        {
            runQueries(new string[] { "DELETE FROM columns", "DELETE FROM tasks" });
        }

        /// <summary>
        /// Inserts an object to the database using that objects implementation for insertion
        /// </summary>
        /// <param name="dalObj">The object to insert to the database</param>
        /// <returns></returns>
        public int InsertObjectToDB(DalObject dalObj)
        {
            return dalObj.InsertToDB(this);
        }

        /// <summary>
        /// Updates an object to the database using that objects implementation for update
        /// </summary>
        /// <param name="dalObj">The object to update to the database.</param>
        public void UpdateObjectToDB(DalObject dalObj)
        {
            dalObj.UpdateToDB(this);
        }


        /// <summary>
        /// Updates what column a tasks belongs to in the database
        /// </summary>
        /// <param name="task">The task to change its column</param>
        /// <param name="newColumnId">The new column the task should belong to</param>
        public void UpdateTaskColumn(Task task, int newColumnId)
        {
            SQLiteConnection con = getConnection();
            con.Open();
            task.UpdateToDB(this, con, newColumnId);
            con.Close();
        }

        /// <summary>
        /// Inserts a task to the database with a specified columnid value for that task
        /// </summary>
        /// <param name="task">The task to insert to the database</param>
        /// <param name="columnId">The columnid value for the task in the database</param>
        /// <returns>The inserted task's id</returns>
        public int InsertTaskToColumn(Task task, int columnId)
        {
            return task.InsertToDB(this, columnId);
        }

        /// <summary>
        /// Inserts a column to the database with a specified email value
        /// </summary>
        /// <param name="c">The row to insert to the database</param>
        /// <param name="email">The email value for the new column in the database</param>
        /// <returns>The id of the inserted column</returns>
        public int InsertColumn(Column c, string email)
        {
            SQLiteConnection con = getConnection();
            con.Open();
            c.insertToDB(this, con, email);
            int id = (int)con.LastInsertRowId;
            con.Close();
            return id;
        }

        /// <summary>
        /// Deletes an object from the database
        /// </summary>
        /// <param name="dalObj"></param>
        public void DeleteFromDB(DalObject dalObj)
        {
            dalObj.DeleteFromDB(this);
        }
    }
}