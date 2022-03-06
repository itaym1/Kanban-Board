using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class BoardController
    {
        private Dictionary<string, Board> boards = new Dictionary<string, Board>(StringComparer.InvariantCultureIgnoreCase);
        private DataAccessLayer.DalController DC = new DataAccessLayer.DalController();
        
        /// <summary>
        /// gets the board by email
        /// </summary>
        /// <param name="email">the desired board email</param>
        /// <returns>board with the email that matches</returns>
        private Board GetUserBoard(string email)
        {
            return boards[email];
        }

        /// <summary>
        /// gets the board by email
        /// </summary>
        /// <param name="email">the desired board email</param>
        /// <returns>read only collection of strings with the names of the columns in the board</returns>
        public IReadOnlyCollection<string> GetBoard(string email) {
            Board board = GetUserBoard(email);
            return board.GetColumnsAsStrings();
        }

        /// <summary>
        /// adds a new taks to board
        /// </summary>
        /// <param name="email">email of the board to add to</param>
        /// <param name="title">the title of the new task</param>
        /// <param name="description">the description of the new task</param>
        /// <param name="dueDate">the due date of the new task</param>
        /// <returns>the next task id</returns>
        public int AddTask(string email, string title, string description, DateTime dueDate)
        {
            int columnId;
            Task t = GetUserBoard(email).AddTask(title, description, dueDate, out columnId);
            int taskId = DC.InsertTaskToColumn((DataAccessLayer.Task)t.ToDalObject(), columnId);
            t.Id = taskId;
            return taskId;
        }

        /// <summary>
        /// limits the column max tasks
        /// </summary>
        /// <param name="email">email of the board to limit</param>
        /// <param name="columnOrdinal">ordinal of the column to limit</param>
        /// <param name="limit">the new max tasks</param>
        public void LimitColumnTasks(string email, int columnOrdinal, int limit)
        {
            Board board = GetUserBoard(email);
            Column c = board.LimitColumnTasks(columnOrdinal, limit);
            DC.UpdateObjectToDB(c.ToDalColumn(columnOrdinal));
        }
        
        /// <summary>
        /// updates the task description
        /// </summary>
        /// <param name="email">the email of the board to update</param>
        /// <param name="columnOrdinal">ordinal of the column to update</param>
        /// <param name="taskId">the id of the task to update</param>
        /// <param name="description">the new description</param>
        public void UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            Task t = GetUserBoard(email).UpdateTaskDescription(columnOrdinal, taskId, description);
            t.Update(DC);
        }

        /// <summary>
        /// updates the task title
        /// </summary>
        /// <param name="email">the email of the board to update</param>
        /// <param name="columnOrdinal">ordinal of the column to update</param>
        /// <param name="taskId">the id of the task to update</param>
        /// <param name="title">the new title</param>
        public void UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
            Task t = GetUserBoard(email).UpdateTaskTitle(columnOrdinal, taskId, title);
            t.Update(DC);
        }

        /// <summary>
        /// updates the task due date
        /// </summary>
        /// <param name="email">the email of the board to update</param>
        /// <param name="columnOrdinal">ordinal of the column to update</param>
        /// <param name="taskId">the id of the task to update</param>
        /// <param name="dueDate">the new due date</param>
        public void UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
            Task t = GetUserBoard(email).UpdateTaskDueDate(columnOrdinal, taskId, dueDate);
            t.Update(DC);
        }

        /// <summary>
        /// advance a tasks that is not done
        /// </summary>
        /// <param name="email">the email of the board to update</param>
        /// <param name="columnOrdinal">ordinal of the column to update</param>
        /// <param name="taskId">the task id to advance</param>
        public void AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            int columnId;
            Task t = GetUserBoard(email).AdvanceTask(columnOrdinal, taskId, out columnId);
            DC.UpdateTaskColumn((DataAccessLayer.Task) t.ToDalObject(), columnId);
        }

        /// <summary>
        /// gets a column by name
        /// </summary>
        /// <param name="email">the email of the board to get from</param>
        /// <param name="columnName">column to get tasks from</param>
        /// <returns>read only collection of service layer tasks</returns>
        public IReadOnlyCollection<ServiceLayer.Task> GetColumn(string email, string columnName)
        {
            Board board = GetUserBoard(email);
            IReadOnlyCollection<Task> businessTasks = board.GetColumnTasks(columnName);
            List<ServiceLayer.Task> serviceTasks = new List<ServiceLayer.Task>();
            foreach (Task task in businessTasks)
            {
                ServiceLayer.Task serviceTask = new ServiceLayer.Task(task.Id, task.CreationDate, task.Title, task.Description, task.DueDate);
                serviceTasks.Add(serviceTask);
            }
            return serviceTasks;
        }

        /// <summary>
        /// gets a column by ordinal
        /// </summary>
        /// <param name="email">the email of the board to get from</param>
        /// <param name="columnOrdinal">column to get tasks from</param>
        /// <returns>read only collection of service layer tasks</returns>
        public IReadOnlyCollection<ServiceLayer.Task> GetColumn(string email, int columnOrdinal)
        {
            Board board = GetUserBoard(email);
            IReadOnlyCollection<Task> businessTasks = board.GetColumnTasks(columnOrdinal);
            List<ServiceLayer.Task> serviceTasks = new List<ServiceLayer.Task>();
            foreach (Task task in businessTasks)
            {
                ServiceLayer.Task serviceTask = new ServiceLayer.Task(task.Id, task.CreationDate, task.Title, task.Description, task.DueDate);
                serviceTasks.Add(serviceTask);
            }
            return serviceTasks;
        }

        /// <summary>
        /// gets the max tasks in a column
        /// </summary>
        /// <param name="email">email of the board to get from</param>
        /// <param name="name">name of the column to get from</param>
        /// <returns></returns>
        public int GetColumnLimit(string email, string name)
        {
            Board board = GetUserBoard(email);
            return board.GetColumnLimit(name);
        }

        /// <summary>
        /// gets the max tasks in a column
        /// </summary>
        /// <param name="email">email of board to get from</param>
        /// <param name="ordinal">ordinal of the column to get from</param>
        /// <returns></returns>
        public int GetColumnLimit(string email, int ordinal)
        {
            Board board = GetUserBoard(email);
            return board.GetColumnLimit(ordinal);
        }
        
        /// <summary>
        /// gets the column name
        /// </summary>
        /// <param name="email">email of board to get from</param>
        /// <param name="ordinal">ordinal of column to get from</param>
        /// <returns></returns>
        public string GetColumnName(string email, int ordinal)
        {
            Board board = GetUserBoard(email);
            return board.GetColumnName(ordinal);
        }

        /// <summary>
        /// loads all the boards that are saved
        /// </summary>
        public void LoadData()
        {
            List<DataAccessLayer.Board> list = DC.LoadBoards();
            foreach (DataAccessLayer.Board board in list)
            {
                AddBoard(new Board(board));
            }
        }

        /// <summary>
        /// adds a board to the lists of  boards
        /// </summary>
        /// <param name="board">the board to add to list</param>
        private void AddBoard(Board board) 
        {
            boards.Add(board.email, board);
        }
        /// <summary>
        /// create and store a new board for new user
        /// </summary>
        /// <param name="email"> board email</param>
        public void CreateNewBoard(string email)
        {
            Board b = new Board(email);
            AddBoard(b);
            int pos = 0;
            foreach (Column c in b.Columns)
            {
                c.Id = DC.InsertColumn(c.ToDalColumn(pos), email);
                pos++;
            }
        }
        /// <summary>
        /// delete boards data and clear boards list
        /// </summary>
        public void DeleteData()
        {
            DC.deleteAllBoards();
            boards = new Dictionary<string, Board>(StringComparer.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// remove specify column from board and update data base
        /// </summary>
        /// <param name="email">board email</param>
        /// <param name="columnOrdinal">column ordinal</param>
        public void RemoveColumn(string email, int columnOrdinal)
        {
            Board board = GetUserBoard(email);
            Column c = board.RemoveColumn(columnOrdinal);
            c.Delete(DC);
            board.Update(DC); //update positions of columns
        }
        /// <summary>
        /// Add new column for board email at the columnOrdinal with the name specified
        /// </summary>
        /// <param name="email">board email</param>
        /// <param name="columnOrdinal">column ordinal</param>
        /// <param name="Name">column name</param>
        /// <returns></returns>
        public IReadOnlyCollection<ServiceLayer.Task> AddColumn(string email, int columnOrdinal, string Name)
        {
            Board board = GetUserBoard(email);
            Column c = board.AddColumn(columnOrdinal, Name);
            int id = DC.InsertColumn(c.ToDalColumn(columnOrdinal), email);
            c.Id = id;
            board.Update(DC);
            IReadOnlyCollection<Task> businessTasks = board.GetColumnTasks(columnOrdinal);
            return toServiceTasks(businessTasks);
        }
        /// <summary>
        /// move the column at the ordinal specifiend to the next ordinal. do nothing for last column.
        /// </summary>
        /// <param name="email">board email</param>
        /// <param name="columnOrdinal">column to move</param>
        /// <returns>list of service type tasks</returns>
        public IReadOnlyCollection<ServiceLayer.Task> MoveColumnRight(string email, int columnOrdinal)
        {
            Board board = GetUserBoard(email);
            board.MoveColumnRight(columnOrdinal);
            board.Update(DC);
            IReadOnlyCollection<Task> businessTasks = board.GetColumnTasks(columnOrdinal + 1);
            return toServiceTasks(businessTasks);
        }
        /// <summary>
        /// move the column at the ordinal specifiend to the previous ordinal. do nothing for first column.
        /// </summary>
        /// <param name="email">board email</param>
        /// <param name="columnOrdinal">column to move</param>
        /// <returns>list of service type tasks</returns>
        public IReadOnlyCollection<ServiceLayer.Task> MoveColumnLeft(string email, int columnOrdinal)
        {
            Board board = GetUserBoard(email);
            board.MoveColumnLeft(columnOrdinal);
            board.Update(DC);
            IReadOnlyCollection<Task> businessTasks = board.GetColumnTasks(columnOrdinal - 1);
            return toServiceTasks(businessTasks);
        }
        /// <summary>
        /// change list of business tasks to service tasks.
        /// </summary>
        /// <param name="businessTasks">list of business type tasks</param>
        /// <returns>list of service type tasks</returns>
        private IReadOnlyCollection<ServiceLayer.Task> toServiceTasks(IReadOnlyCollection<Task> businessTasks)
        {
            List<ServiceLayer.Task> serviceTasks = new List<ServiceLayer.Task>();
            foreach (Task task in businessTasks)
            {
                ServiceLayer.Task serviceTask = new ServiceLayer.Task(task.Id, task.CreationDate, task.Title, task.Description, task.DueDate);
                serviceTasks.Add(serviceTask);
            }
            return serviceTasks;
        }
    }
}
