using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Column : DalObject
    {
        public string name { get; set; }
        public int maxTasks { get; set; }
        public int position { get; set; }
        public int id { get; set; }
        public List<Task> tasks { get; set; }
        
        public Column()
        {
            tasks = new List<Task>();
        }
        /// <summary>
        /// simple constructor
        /// </summary>
        /// <param name="name"> column name </param>
        /// <param name="maxTasks"> maximum allowed tasks </param>
        /// <param name="tasks"> list of tasks </param>
        public Column(string name, int maxTasks, List<Task> tasks, int position, int id)
        {
            this.name = name;
            this.maxTasks = maxTasks;
            this.tasks = tasks;
            this.position = position;
            this.id = id;
        }

        /// <summary>
        /// Constroctur used while loading data
        /// </summary>
        /// <param name="columnReader">contains the columns table</param>
        /// <param name="da"></param>
        /// <param name="con"></param>
        public Column(SQLiteDataReader columnReader, DalController da, SQLiteConnection con)
        {
            this.id = Convert.ToInt32(columnReader["id"]);
            this.tasks = new List<Task>();
            this.name = (string)columnReader["name"];
            this.maxTasks = Convert.ToInt32(columnReader["maxtasks"]);
            this.position = Convert.ToInt32(columnReader["position"]);
            string taskQuery = String.Format("SELECT * FROM tasks where ColumnId = {0}{1}{2}", "'", id, "'");
            SQLiteDataReader taskReader = da.runQueryWithReturn(taskQuery, con);
            while (taskReader.Read())
            {
                tasks.Add(new Task(taskReader));
            }
        }

        /// <summary>
        /// insert this column to the database
        /// </summary>
        /// <param name="dal"></param>
        /// <param name="con"></param>
        /// <param name="UserEmail"></param>
        public void insertToDB(DalController dal, SQLiteConnection con, string UserEmail)
        {
            string query = String.Format("INSERT INTO COLUMNS (UserEmail, Name, MaxTasks, Position) VALUES ({0}, {1}, {2}, {3})", "'" + UserEmail + "'", "'" + name + "'", "'" + maxTasks + "'", "'" + position + "'");
            dal.runQuery(query, con);
            foreach (Task task in tasks)
            {
                task.insertToDB(dal, con, id);
            }
        }

        public override int InsertToDB(DalController dal)
        {
            throw new NotImplementedException(); //not needed for column
        }

        /// <summary>
        /// updates this column to the database
        /// </summary>
        /// <param name="dal"></param>
        public override void UpdateToDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            string query = String.Format("UPDATE COLUMNS SET Name = {0}, MaxTasks = {1}, Position = {2} WHERE ID = {3}", "'" + name + "'", "'" + maxTasks + "'", "'" + position + "'", "'" + id + "'");
            dal.runQuery(query, con);
            foreach (Task task in tasks)
            {
                task.UpdateToDB(dal, con, id);
            }
            con.Close();
        }

        /// <summary>
        /// deletes this column from the database
        /// </summary>
        /// <param name="dal"></param>
        public override void DeleteFromDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            SQLiteCommand command = new SQLiteCommand(null, con);
            command.CommandText = "DELETE FROM COLUMNS WHERE id = @param0";
            SQLiteParameter param0 = new SQLiteParameter(@"param0", id);
            command.Parameters.Add(param0);
            command.Prepare();
            command.ExecuteNonQuery();
            con.Close();
        }
    }
}
