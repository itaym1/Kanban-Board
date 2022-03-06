using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class Board : DalObject
    {
        public List<Column> columns { get; set; }
        public int nextTaskId { get; set; }
        public string email { get; set; }
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="columns"> list of boards columns </param>
        /// <param name="nextTaskId"> number of created tasks </param>
        /// <param name="email"> email of the board owner </param>
        public Board(List<Column> columns, int nextTaskId, string email)
        {
            this.columns = columns;
            this.nextTaskId = nextTaskId;
            this.email = email;
        }

        /// <summary>
        /// Constroctur used for loading from database
        /// </summary>
        /// <param name="userReader">contains the users table</param>
        /// <param name="da"></param>
        /// <param name="con"></param>
        public Board(SQLiteDataReader userReader, DalController da, SQLiteConnection con)
        {
            this.email = (string)userReader["Email"];
            this.columns = new List<Column>();
            string columnQuery = String.Format("SELECT * FROM columns WHERE useremail = {0}{1}{2}", "'", email, "'");
            SQLiteDataReader columnReader = da.runQueryWithReturn(columnQuery, con);
            while (columnReader.Read())
            {
                columns.Add(new Column(columnReader, da, con));
            }
            this.columns = columns.OrderBy(column => column.position).ToList(); //order columns in the list by their position.
        }

        /// <summary>
        /// insert a new board to database
        /// </summary>
        /// <param name="dal"></param>
        /// <returns></returns>
        public override int InsertToDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            foreach (Column col in columns)
            {
                col.insertToDB(dal, con, email);
            }
            con.Close();
            return -1; //not in db
        }

        /// <summary>
        /// updates board to database
        /// </summary>
        /// <param name="dal"></param>
        public override void UpdateToDB(DalController dal)
        {
            SQLiteConnection con = dal.getConnection();
            con.Open();
            foreach (Column col in columns)
            {
                col.UpdateToDB(dal);
            }
            con.Close();
        }

        public override void DeleteFromDB(DalController dal)
        {
            throw new NotImplementedException(); //not needed
        }
    }
}
