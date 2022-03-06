using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Board : Saveable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string DEF_COL_NAME_1 = "backlog";
        const string DEF_COL_NAME_2 = "in progress";
        const string DEF_COL_NAME_3 = "done";
        const int DEFAULT_MAX_TASKS = int.MaxValue;
        const int MIN_COLUMS = 2;
        const int MAX_COL_NAME = 15;
        private List<Column> columns;
        public List<Column> Columns
        {
            get { return columns; }
        }

        public string email { get; set; }
        /// <summary>
        /// simple board constructor
        /// </summary>
        /// <param name="email"> email of the user which the board belongs</param>
        public Board(string email)
        {
            this.email = email;
            this.columns = new List<Column>();
            columns.Add(new Column(DEF_COL_NAME_1, DEFAULT_MAX_TASKS));
            columns.Add(new Column(DEF_COL_NAME_2, DEFAULT_MAX_TASKS));
            columns.Add(new Column(DEF_COL_NAME_3, DEFAULT_MAX_TASKS));
            log.Info(String.Format("Created a new board for user: {0}", email));
        }
        /// <summary>
        /// board constroctor from a DAL object
        /// </summary>
        /// <param name="board">DAL board</param>
        public Board(DataAccessLayer.Board board)
        {
            email = board.email;
            columns = new List<Column>();
            foreach (DataAccessLayer.Column col in board.columns)
            {
                Column newCol = new Column(col);
                foreach(DataAccessLayer.Task task in col.tasks)
                {
                    newCol.AddTask(new Task(task));
                }
                columns.Add(newCol);
            }
        }

        /// <summary>        
        /// Adds a task to the board
        /// </summary>
        /// <param name="title">The title of the new task</param>
        /// <param name="description">The description of the new task</param>
        /// <param name="dueDate">The due date of the new task</param>
        /// <returns>the next task id</returns>
        public Task AddTask(string title, string description, DateTime dueDate, out int columnId)
        {
            Column todoColumn = columns[0];
            Task task = new Task(title, description, dueDate, 0);
            todoColumn.AddTask(task);
            columnId = todoColumn.Id;
            return task;
        }

        /// <summary>
        /// get a coloumn by its name
        /// </summary>
        /// <param name="name">name of the column to get</param>
        /// <returns></returns>
        private Column GetColumnByName(string name)
        {
            foreach (Column column in columns)
            {
                if (column.Name.ToString().Equals(name))
                {
                    return column;
                }
            }
            return null;
        }

        /// <summary>
        /// updates the task description
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column where the task is at</param>
        /// <param name="taskId"> the id of the task to edit</param>
        /// <param name="newDescription">the new description to update the task</param>
        public Task UpdateTaskDescription(int columnOrdinal, int taskId, string newDescription)
        {
            canAdvanceFromColumn(columnOrdinal);
            Column column = columns[columnOrdinal];
            return column.UpdateTaskDescription(taskId, newDescription);
        }

        /// <summary>
        /// updates the task title
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column where the task is at</param>
        /// <param name="taskId"> the id of the task to edit</param>
        /// <param name="newTitle">the new title to update the task</param>
        public Task UpdateTaskTitle(int columnOrdinal, int taskId, string newTitle)
        {
            canAdvanceFromColumn(columnOrdinal);
            Column column = columns[columnOrdinal];
            return column.UpdateTaskTitle(taskId, newTitle);
        }
        /// <summary>
        /// update the task due date
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column where the task is at</param>
        /// <param name="taskId"> the id of the task to edit</param>
        /// <param name="dueDate">the new due date to update the task</param>
        public Task UpdateTaskDueDate(int columnOrdinal, int taskId, DateTime dueDate)
        {
            canAdvanceFromColumn(columnOrdinal);
            Column column = columns[columnOrdinal];
            return column.UpdateTaskDueDate(taskId, dueDate);
        }
        /// <summary>
        /// throws exception if column is out of boundries or the last one
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column where the task is at</param>
        private void canAdvanceFromColumn (int columnOrdinal)
        {
            if (columnOrdinal < 0 || columnOrdinal >= columns.Count)
            {
                throw new Exception("Ordinal out of boundaries.");
            }
            if (columnOrdinal == columns.Count - 1)
            {
                throw new Exception("Tasks in the last column cannot be changed!");
            }
        }

        /// <summary>
        /// advance the task to the next column as long as its not in done column
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column where the task is at</param>
        /// <param name="taskId"> the id of the task to advance</param>
        public Task AdvanceTask(int columnOrdinal, int taskId, out int columnId)
        {
            if (columnOrdinal >= columns.Count - 1) //last column
            {
                throw new Exception("You cannot advance a task from this column");
            }
            Column column = columns[columnOrdinal];
            Task task = column.RemoveTask(taskId);
            if (task == null)
            {
                throw new Exception(String.Format("task with id: {0} does not exist", taskId));
            }
            columns[columnOrdinal + 1].AddTask(task);
            columnId = columns[columnOrdinal + 1].Id;
            return task;
        }

        /// <summary>
        /// get all the columns names as string
        /// </summary>
        /// <returns>list<string> which holds all the columns names</returns>
        public List<string> GetColumnsAsStrings()
        {
            List<string> list = new List<string>();
            foreach(Column c in columns)
            {
                list.Add(c.Name.ToString());
            }
            return list;
        }

        /// <summary>
        /// limits the number of tasks the column
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column to edit</param>
        /// <param name="limit">the max tasks in the column</param>
        public Column LimitColumnTasks(int columnOrdinal, int limit)
        {
            if(columnOrdinal >= columns.Count)
            {
                throw new Exception("column ordinal out of boundries.");
            }
            Column c = columns[columnOrdinal];
            c.MaxTasks = limit;
            return c;
        }

        /// <summary>
        /// gets all the tasks in a column
        /// </summary>
        /// <param name="columnName"> the name of the column to get from</param>
        /// <returns>read only collection with all the tasks in the column</returns>
        public IReadOnlyCollection<Task> GetColumnTasks(string columnName)
        {
            Column c = GetColumnByName(columnName);
            if (c == null)
            {
                throw new Exception("there is no column by the name of" + columnName);
            }
            return c.Tasks;
        }

        /// <summary>
        /// gets all the tasks in a column
        /// </summary>
        /// <param name="columnOrdinal">the ordinal of the column to get from</param>
        /// <returns>read only collection with all the tasks in the column</returns>
        public IReadOnlyCollection<Task> GetColumnTasks(int columnOrdinal)
        {
            if (columnOrdinal >= columns.Count)
            {
                throw new Exception("column ordinal out of boundries.");
            }
            return columns[columnOrdinal].Tasks;
        }

        /// <summary>
        /// gets the max tasks in a column
        /// </summary>
        /// <param name="name"> the name of the column to get from</param>
        /// <returns>the max number of tasks in the column</returns>
        public int GetColumnLimit(string name)
        {
            return GetColumnByName(name).MaxTasks;
        }

        /// <summary>
        /// gets the max tasks in a column
        /// </summary>
        /// <param name="ordinal">the ordinal of the column to get from </param>
        /// <returns>the max number of tasks in the column</returns>
        public int GetColumnLimit(int ordinal)
        {
            if (ordinal >= columns.Count)
            {
                throw new Exception("Column ordinal out of boundaries.");
            }
            return columns[ordinal].MaxTasks;
        }

        /// <summary>
        /// gets the name of the column
        /// </summary>
        /// <param name="ordinal">the ordinal of the column to get from</param>
        /// <returns>name of column</returns>
        public string GetColumnName(int ordinal)
        {
            if(ordinal >= columns.Count)
            {
                throw new Exception("Column ordinal out of boundaries.");
            }
            return columns[ordinal].Name.ToString();
        }

        /// <summary>
        /// converts the board to DAL board
        /// </summary>
        /// <returns>the same board but DalObject</returns>
        public override DalObject ToDalObject()
        {
            List<DataAccessLayer.Column> dataColumns = new List<DataAccessLayer.Column>();
            int pos = 0;
            foreach (Column c in columns)
            {
                List<DataAccessLayer.Task> dataTasks = new List<DataAccessLayer.Task>();
                foreach (Task t in c.Tasks)
                {
                    dataTasks.Add(new DataAccessLayer.Task(t.Title, t.Description, t.CreationDate, t.DueDate, t.Id));
                }
                DataAccessLayer.Column dataColumn = new DataAccessLayer.Column(c.Name, c.MaxTasks, dataTasks, pos, c.Id);
                dataColumns.Add(dataColumn);
                pos++;
            }
            return new DataAccessLayer.Board(dataColumns, 0, email);
        }
        /// <summary>
        /// remove column at ordinalColumn.
        /// </summary>
        /// <param name="columnOrdinal">column to remove</param>
        /// <returns>the removed column</returns>
        public Column RemoveColumn(int columnOrdinal)
        {
            if (columnOrdinal < 0 | columnOrdinal >= columns.Count)
                throw new ArgumentOutOfRangeException("Illegal column ordinal input");
            if (columns.Count <= MIN_COLUMS)
                throw new Exception("Can not remove more columns. Minimum number of columns is: " + MIN_COLUMS);
            int index;
            if(columnOrdinal == 0)
                index = columnOrdinal + 1;
            else
                index = columnOrdinal - 1;
            if (columns[columnOrdinal].Tasks.Count + columns[index].Tasks.Count > columns[index].MaxTasks)
                throw new Exception("Can not remove column: " + columns[columnOrdinal].Name + ". Column " + columns[index].Name + " have a maximum of " + columns[index].MaxTasks + " tasks limit");
            foreach (Task t in columns[columnOrdinal].Tasks)
            {
                columns[index].AddTask(t);
            }
            Column c = columns[columnOrdinal];
            columns.RemoveAt(columnOrdinal);
            return c;
        }
        /// <summary>
        /// add column at specified place
        /// </summary>
        /// <param name="columnOrdinal">where to add the column</param>
        /// <param name="name">column name</param>
        /// <returns>Added column</returns>
        public Column AddColumn(int columnOrdinal, string name)
        {
            if (columnOrdinal < 0 | columnOrdinal > columns.Count)
                throw new ArgumentOutOfRangeException("Illegal column ordinal input");
            if (name == null || name.Length == 0 | name.Length > MAX_COL_NAME)
                throw new ArgumentOutOfRangeException("Illegal column name input");
            foreach(Column col in columns) {
                if (col.Name.Equals(name))
                    throw new ArgumentException("Column name is taken");
            }
            Column c = new Column(name, DEFAULT_MAX_TASKS);
            columns.Insert(columnOrdinal, c);
            return c;
        }
        /// <summary>
        /// move the column at the ordinal specifiend to the next ordinal. do nothing for last column.
        /// </summary>
        /// <param name="columnOrdinal">column to move</param>
        public void MoveColumnRight(int columnOrdinal)
        {
            if(columnOrdinal < 0 | columnOrdinal >= columns.Count-1)
                throw new ArgumentOutOfRangeException("Illegal column ordinal input");
            Column tmpC = columns[columnOrdinal];
            columns[columnOrdinal] = columns[columnOrdinal + 1];
            columns[columnOrdinal + 1] = tmpC;
        }
        /// <summary>
        /// move the column at the ordinal specifiend to the previous ordinal. do nothing for first column.
        /// </summary>
        /// <param name="columnOrdinal">column to move</param>
        public void MoveColumnLeft(int columnOrdinal)
        {
            if (columnOrdinal < 1 | columnOrdinal >= columns.Count)
                throw new ArgumentOutOfRangeException("Illegal column ordinal input");
            Column tmpC = columns[columnOrdinal];
            columns[columnOrdinal] = columns[columnOrdinal - 1];
            columns[columnOrdinal - 1] = tmpC;
        }
    }
}
