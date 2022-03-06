using System;
using System.Collections.Generic;


namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// handles all the serivce requests that are related to board.
    /// </summary>
    public class BoardService {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BusinessLayer.BoardController BC;

        /// <summary>
        /// A simple public constructor 
        /// </summary>
        internal BoardService() {
            BC = new BusinessLayer.BoardController();
        }

        /// <summary>
        /// requests to load all the boards from the hard drive to the board controller/
        /// </summary>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LoadData() {
            try
            {
                BC.LoadData();
            }
            catch (Exception e)
            {
                log.Error(string.Format("failed to load data!, exception: {0}, stacktrace: {1}", e.Message, e.StackTrace));
                return new Response(e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// returns a board object with all the columns name of the board that belongs to the user
        /// </summary>
        /// <param name="email">the email of the requesting user</param>
        /// <returns>return the board in service level object that belongs to the requesting user</returns>
        public Response<Board> GetBoard(string email) {
                try {
                    IReadOnlyCollection<string> columns = BC.GetBoard(email);
                    return new Response<Board>(new Board(columns));
                }
                catch (Exception e) {
                log.Error(string.Format("failed to get user's board, email: {0}, exception: {1}, stacktrace: {2}", email, e.Message, e.StackTrace));
                return new Response<Board>(e.Message);
                }
        }

        /// <summary>
        /// Adds a limit to the amount of tasks allowed to a column
        /// </summary>
        /// <param name="email">the email of the requesting user</param>
        /// <param name="columnOrdinal">the column ordinal of the column we wanna limit</param>
        /// <param name="limit">the new limit of tasks for the desired column</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LimitColumnTasks(string email, int columnOrdinal, int limit) {
                try {
                    BC.LimitColumnTasks(email, columnOrdinal, limit);
                }
                catch (Exception e) {
                log.Error(string.Format("failed to limit a column, email: {0}, column orindal: {1}, limit : {2}, exception: {3}, stacktrace: {4}", email, columnOrdinal, limit, e.Message, e.StackTrace));
                return new Response(e.Message);
                }
                return new Response();
        }

        /// <summary>
        /// Adds a new task to the requesting user's board
        /// </summary>
        /// <param name="email">the email of the requesting user</param>
        /// <param name="title">the title of the new task</param>
        /// <param name="description">the description of the new task</param>
        /// <param name="dueDate">the due date of the new task</param>
        /// <returns>returns the added task or an error if one occurs</returns>
        public Response<Task> AddTask(string email, string title, string description, DateTime dueDate) {
                try {
                   int id = BC.AddTask(email, title, description, dueDate);
                   return new Response<Task>(new Task(id, DateTime.Now, title, description, dueDate));
                }
                catch (Exception e) {
                log.Error(String.Format("failed to add task, error: {0}, exception trace: {1}, email: {2}, title: {3}, description: {4}, duedate: {5}", e.Message, e.StackTrace, email, title, description, dueDate.ToString()));
                    return new Response<Task>(e.Message);
                }
        }

        /// <summary>
        /// Updates the desired task's description.
        /// </summary>
        /// <param name="email">The email of the requesting user</param>
        /// <param name="columnOrdinal">The ordinal of the column where the task to modify its description should be</param>
        /// <param name="taskId">The id of the task we should modify the description of</param>
        /// <param name="title">The value we change the description to</param>
        /// <returns></returns>
        public Response UpdateTaskDescription(string email, int columnOrdinal, int taskId, string description)
        {
            try
            {
                BC.UpdateTaskDescription(email, columnOrdinal, taskId, description);
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to update task description, exception: {0}, exception trace: {1}, email: {2}, column ordinal: {3}, task id: {4}, description: {5}", e.Message, e.StackTrace, email, columnOrdinal, taskId, description));
                return new Response(e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// Updates the desired task's due date.
        /// </summary>
        /// <param name="email">The email of the requesting user</param>
        /// <param name="columnOrdinal">The ordinal of the column where the task to modify its due date should be</param>
        /// <param name="taskId">The id of the task we should modify the due date of</param>
        /// <param name="title">The value we change the due date to</param>
        /// <returns></returns>
        public Response UpdateTaskDueDate (string email, int columnOrdinal, int taskId, DateTime dueDate)
        {
                try
                {
                    BC.UpdateTaskDueDate(email, columnOrdinal, taskId, dueDate);
                }
                catch (Exception e)
                {
                log.Error(String.Format("failed to update task due date, exception: {0}, exception trace: {1}, email: {2}, column ordinal: {3}, task id: {4}, due date: {5}", e.Message, e.StackTrace, email, columnOrdinal, taskId, dueDate.ToString()));
                return new Response(e.Message);
                }
                return new Response();
        }

        /// <summary>
        /// Updates the desired task's title.
        /// </summary>
        /// <param name="email">The email of the requesting user</param>
        /// <param name="columnOrdinal">The ordinal of the column where the task to modify its title should be</param>
        /// <param name="taskId">The id of the task we should modify the title of</param>
        /// <param name="title">The value we change the title to</param>
        /// <returns></returns>
        public Response UpdateTaskTitle(string email, int columnOrdinal, int taskId, string title)
        {
                try
                {
                    BC.UpdateTaskTitle(email, columnOrdinal, taskId, title);
                }
                catch (Exception e)
                {
                log.Error(String.Format("failed to update task title, exception: {0}, exception trace: {1}, email: {2}, column ordinal: {3}, task id: {4}, title: {5}", e.Message, e.StackTrace, email, columnOrdinal, taskId, title));
                return new Response(e.Message);
                }
                return new Response();
        }


        /// <summary>
        /// advances a task from a column to the next one.
        /// </summary>
        /// <param name="email">The email of the requesting user</param>
        /// <param name="columnOrdinal">The column where the task to advance should be found</param>
        /// <param name="taskId">The id of the task we should advance</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response AdvanceTask(string email, int columnOrdinal, int taskId)
        {
                try
                {
                    BC.AdvanceTask(email, columnOrdinal, taskId);
                }
                catch (Exception e)
                {
                log.Error(String.Format("failed to advance task, exception: {0}, exception trace: {1}, email: {2}, column ordinal: {3}, task id: {4}", e.Message, e.StackTrace, email, columnOrdinal, taskId));
                return new Response(e.Message);
                }
                return new Response();
        }

        /// <summary>
        /// returns a service layer column by column name that belongs to the user that goes by the email param.
        /// </summary>
        /// <param name="email">The email address of the requesting user</param>
        /// <param name="columnName">The column name of the desired column</param>
        /// <returns>A service layer column</returns>
        public Response<Column> GetColumn(string email, string columnName)
        {
                try
                {
                    IReadOnlyCollection<Task> tasks = BC.GetColumn(email, columnName);
                    return new Response<Column>(new Column(tasks, columnName, BC.GetColumnLimit(email ,columnName)));
                }
                catch (Exception e)
                {
                log.Error(String.Format("failed to get column by ordinal, exception: {0}, exception trace: {1}, email: {2}, column name: {3}", e.Message, e.StackTrace, email, columnName));
                return new Response<Column>(e.Message);
                }
        }

        /// <summary>
        /// returns a service layer column by column ordinal that belongs to the user that goes by the email param.
        /// </summary>
        /// <param name="email">The email address of the requesting user</param>
        /// <param name="columnOrdinal">The column ordinal of the desired column</param>
        /// <returns>A service layer column</returns>
        public Response<Column> GetColumn(string email, int columnOrdinal)
        {
                try
                {
                    IReadOnlyCollection<Task> tasks = BC.GetColumn(email, columnOrdinal);
                    return new Response<Column>(new Column(tasks, BC.GetColumnName(email, columnOrdinal), BC.GetColumnLimit(email, columnOrdinal)));
                }
                catch (Exception e)
                {
                log.Error(String.Format("failed to get column by name, exception: {0}, exception trace: {1}, email: {2}, column ordinal: {3}", e.Message, e.StackTrace, email, columnOrdinal));
                return new Response<Column>(e.Message);
                }
        }

        /// <summary>
        /// creates board for the user that the email param belongs to
        /// </summary>
        /// <param name="email">The email address of the requesting user</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response CreateBoard(string email)
        {
            try
            {
                BC.CreateNewBoard(email);
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to create new board for email : {0},  exception: {1}", email, e.Message));
                return new Response(e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// Remove all persistent data
        /// </summary>
        /// <returns>A response object, The response should contain a error message in case of an error</returns>
        public Response DeleteData()
        {
            try
            {
                BC.DeleteData();
            }
            catch (Exception e)
            {
                log.Error(string.Format("failed to Delete data!, exception: {0}, stacktrace: {1}", e.Message, e.StackTrace));
                return new Response(e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// Removes a column given it's identifier.
        /// The first column is identified by 0, the ID increases by 1 for each column
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Column ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string email, int columnOrdinal)
        {
            try
            {
                BC.RemoveColumn(email, columnOrdinal);
            }
            catch(Exception e)
            {
                log.Error(String.Format("failed to RemoveColumn for email : {0},  exception: {1}", email, e.Message));
                return new Response(e.Message);
            }
            return new Response();
        }

        /// <summary>
        /// Adds a new column, given it's name and a location to place it.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Location to place to column</param>
        /// <param name="Name">new Column name</param>
        /// <returns>A response object with a value set to the Column, the response should contain a error message in case of an error</returns>
        public Response<Column> AddColumn(string email, int columnOrdinal, string Name)
        {
            try
            {
                BC.AddColumn(email, columnOrdinal, Name);
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to Add Column for email : {0},  exception: {1}", email, e.Message));
                return new Response<Column>(e.Message);
            }
            return new Response<Column>(new Column(new List<Task>(), Name, int.MaxValue));
        }

        /// <summary>
        /// Moves a column to the right, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column        
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the columns</param>
        /// <returns>A response object with a value set to the column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnRight(string email, int columnOrdinal)
        {
            try
            {
                string Name = BC.GetColumnName(email, columnOrdinal);
                int limit = BC.GetColumnLimit(email, columnOrdinal);
                IReadOnlyCollection<Task> tasks = BC.MoveColumnRight(email, columnOrdinal);
                return new Response<Column>(new Column(tasks, Name, limit));
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to move Column for email : {0},  exception: {1}", email, e.Message));
                return new Response<Column>(e.Message);
            }


        }

        /// <summary>
        /// Moves a column to the left, swapping it with the column wich is currently located there.
        /// The first column is identified by 0, the ID increases by 1 for each column.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="columnOrdinal">Current location of the columns</param>
        /// <returns>A response object with a value set to the column, the response should contain a error message in case of an error</returns>
        public Response<Column> MoveColumnLeft(string email, int columnOrdinal)
        {
            try
            {
                string Name = BC.GetColumnName(email, columnOrdinal);
                int limit = BC.GetColumnLimit(email, columnOrdinal);
                IReadOnlyCollection<Task> tasks = BC.MoveColumnLeft(email, columnOrdinal);
                return new Response<Column>(new Column(tasks, Name, limit));
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to move Column for email : {0},  exception: {1}", email, e.Message));
                return new Response<Column>(e.Message);
            }
        }
    }
}
