using System;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Task : DalObject
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime dueDate { get; set; }
        public int id { get; set; }

        /// <summary>
        /// simple constructor
        /// </summary>
        /// <param name="title"> task title </param>
        /// <param name="description"> task description </param>
        /// <param name="creationDate"> task creation date</param>
        /// <param name="dueDate"> task due date </param>
        /// <param name="id"> task unique id </param>
        public Task(string title, string description, DateTime creationDate, DateTime dueDate, int id)
        {
            this.title = title;
            this.description = description;
            this.creationDate = creationDate;
            this.dueDate = dueDate;
            this.id = id;
        }

        /// <summary>
        /// constroctur used while loading data from database
        /// </summary>
        /// <param name="taskReader"></param>
        public Task(SQLiteDataReader taskReader)
        { 
            this.id = Convert.ToInt32(taskReader["id"]);
            this.title = (string)taskReader["title"];
            this.description = (string)taskReader["description"];
            this.creationDate = DateTime.Parse((string)taskReader["creationDate"]);
            this.dueDate = DateTime.Parse((string)taskReader["dueDate"]);
        }

        /// <summary>
        /// inserts this task to the database
        /// </summary>
        /// <param name="dal"></param>
        /// <param name="con"></param>
        /// <param name="ColumnId"></param>
        public void insertToDB(DalController dal, SQLiteConnection con, int ColumnId)
        {
            string query = String.Format("INSERT INTO TASKS (ColumnId, Title, Description, CreationDate, DueDate) VALUES ({0}, {1}, {2}, {3}, {4})", "'" + ColumnId + "'", "'" + title + "'", "'" + description + "'", "'" + creationDate + "'", "'" + dueDate + "'");
            dal.runQuery(query, con);
        }

        /// <summary>
        /// insers this task to the dataBase
        /// </summary>
        /// <param name="dal"></param>
        /// <param name="ColumnId"></param>
        /// <returns>returns the id of the task</returns>
        public int InsertToDB(DalController dal, int ColumnId)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            string query = String.Format("INSERT INTO TASKS (ColumnId, Title, Description, CreationDate, DueDate) VALUES ({0}, {1}, {2}, {3}, {4})", "'" + ColumnId + "'", "'" + title + "'", "'" + description + "'", "'" + creationDate + "'", "'" + dueDate + "'");
            dal.runQuery(query, con);
            int id = (int)con.LastInsertRowId;
            con.Close();
            return id;
        }

        /// <summary>
        /// updates this task to the database when moving columns
        /// </summary>
        /// <param name="dal"></param>
        /// <param name="con"></param>
        /// <param name="ColumnId"></param>
        public void UpdateToDB(DalController dal, SQLiteConnection con, int ColumnId)
        {
            string query = String.Format("UPDATE TASKS SET ColumnID = {0}, Title = {1}, Description = {2}, CreationDate = {3}, DueDate = {4} WHERE ID = {5}", "'" + ColumnId + "'","'" + title + "'", "'" + description + "'", "'" + creationDate + "'","'" + dueDate + "'", "'" + id + "'");
            dal.runQuery(query, con);
        }

        /// <summary>
        /// updates this task to the database
        /// </summary>
        /// <param name="dal"></param>
        public override void UpdateToDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            string query = String.Format("UPDATE TASKS SET Title = {0}, Description = {1}, CreationDate = {2}, DueDate = {3} WHERE ID = {4}", "'" + title + "'", "'" + description + "'", "'" + creationDate + "'", "'" + dueDate + "'", "'" + id + "'");
            dal.runQuery(query, con);
            con.Close();
        }

        /// <summary>
        /// deletes this task from the database
        /// </summary>
        /// <param name="dal"></param>
        public override void DeleteFromDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            SQLiteCommand command = new SQLiteCommand(null, con);
            command.CommandText = "DELETE FROM TASKS WHERE id = @param0";
            SQLiteParameter param0 = new SQLiteParameter(@"param0", id);
            command.Parameters.Add(param0);
            command.Prepare();
            command.ExecuteNonQuery();
            con.Close();
        }

        public override int InsertToDB(DalController dal)
        {
            throw new NotImplementedException();
        }
    }
}
