using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Column : Saveable
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int maxTasks;
        public int MaxTasks
        {
            get { return maxTasks; }
            set {
                if(value < tasks.Count)
                {
                    throw new Exception("Cannot limit a column to an amount lower than its amount of tasks.");
                }
                maxTasks = value;
            }
        }

        private List<Task> tasks;
        public List<Task> Tasks
        {
            get { return tasks; }
        }

        /// <summary>
        /// basic constractor
        /// </summary>
        /// <param name="name">name of the column</param>
        /// <param name="maxTasks">the max tasks of the column</param>
        public Column(string name, int maxTasks)
        {
            this.name = name;
            this.maxTasks = maxTasks;
            this.tasks = new List<Task>();
        }

        /// <summary>
        /// constructs a column from a DAL column
        /// </summary>
        /// <param name="col">DAL column to convert to buisness layer column</param>
        public Column(DataAccessLayer.Column col)
        {
            name = col.name;
            maxTasks = col.maxTasks;
            id = col.id;
            this.tasks = new List<Task>();
        }

        /// <summary>
        /// adds a task to the column
        /// </summary>
        /// <param name="task">task to add</param>
        public void AddTask(Task task)
        {
            if (tasks.Count >= maxTasks)
            {
                throw new Exception(String.Format("{0} column has reached the maximum amount of  tasks", Name));
            }
            tasks.Add(task);
        }

        /// <summary>
        /// updates task description
        /// </summary>
        /// <param name="taskId">the id of the tasks to update</param>
        /// <param name="newDescription">the new description</param>
        public Task UpdateTaskDescription(int taskId, string newDescription)
        {
            Task task = GetTaskById(taskId);
            task.Description = newDescription;
            return task;
        }

        /// <summary>
        /// updates task title
        /// </summary>
        /// <param name="taskId">the id of the task to update</param>
        /// <param name="newTitle">the new title</param>
        public Task UpdateTaskTitle(int taskId, string newTitle)
        {
            Task task = GetTaskById(taskId);
            task.Title = newTitle;
            return task;
        }

        /// <summary>
        /// updates task due date
        /// </summary>
        /// <param name="taskId">the id of the task to update</param>
        /// <param name="dueDate">the new due date</param>
        public Task UpdateTaskDueDate(int taskId, DateTime dueDate)
        {
            Task task = GetTaskById(taskId);
            task.DueDate = dueDate;
            return task;
        }
        
        /// <summary>
        /// gets task from column
        /// </summary>
        /// <param name="id">id of the task to get</param>
        /// <returns>task with the wanted id</returns>
        public Task GetTaskById(int id)
        {
            foreach (Task task in tasks)
            {
                if (task.Id == id)
                {
                    return task;
                }
            }
            throw new Exception(String.Format("This task does not exist or is not in the {0} column", name.ToString()));
        }

        /// <summary>
        /// removes task from column
        /// </summary>
        /// <param name="id">id of task to remove</param>
        /// <returns>the removed task</returns>
        public Task RemoveTask(int id)
        {
            Task taskToRemove = GetTaskById(id);
            if (taskToRemove == null)
            {
                return null;
            }
            tasks.Remove(taskToRemove);
            return taskToRemove;
        }
        /// <summary>
        /// return current column as DAL type.
        /// </summary>
        /// <param name="pos">column position at board</param>
        /// <returns>DAL column with same parameters</returns>
        public DataAccessLayer.Column ToDalColumn(int pos)
        {
            List<DataAccessLayer.Task> dataTasks = new List<DataAccessLayer.Task>();
            foreach (Task t in Tasks)
            {
                dataTasks.Add(new DataAccessLayer.Task(t.Title, t.Description, t.CreationDate, t.DueDate, t.Id));
            }
            return new DataAccessLayer.Column(name, maxTasks, dataTasks, pos, id);
        }
        /// <summary>
        /// implement Savable. 
        /// </summary>
        /// <returns>this column as DAL column</returns>
        public override DalObject ToDalObject()
        {
            List<DataAccessLayer.Task> dataTasks = new List<DataAccessLayer.Task>();
            foreach (Task t in Tasks)
            {
                dataTasks.Add(new DataAccessLayer.Task(t.Title, t.Description, t.CreationDate, t.DueDate, t.Id));
            }
            return new DataAccessLayer.Column(name, maxTasks, dataTasks, -1, id);
        }
    }
}
