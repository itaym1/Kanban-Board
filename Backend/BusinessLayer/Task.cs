using IntroSE.Kanban.Backend.DataAccessLayer;
using System;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Task : Saveable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int MAX_TITLE_LENGTH = 50;
        private const int MAX_DESC_LENGTH = 300;

        private string title;
        public string Title
        {
            get { return title;}
            set {
                if(value == null || value.Length == 0 || value.Length > MAX_TITLE_LENGTH)
                {
                    throw new Exception("Illegal title!.");
                }
                title = value;
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set {
                if(value != null & value.Length > MAX_DESC_LENGTH)
                {
                    throw new Exception("Illegal Description!");
                }
                description = value;
            }
        }

        private DateTime creationDate;
        public DateTime CreationDate
        {
            get { return creationDate; }
        }

        private DateTime dueDate;
        public DateTime DueDate
        {
            get { return dueDate; }
            set {
                if (value.CompareTo(DateTime.Now) < 0)
                {
                    throw new Exception("Duedate has passed.");
                }
                dueDate = value;
            }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// basic task constractor
        /// </summary>
        /// <param name="title">title of task</param>
        /// <param name="description">description of task</param>
        /// <param name="dueDate">due date of task</param>
        /// <param name="id">id of task</param>
        public Task(string title, string description, DateTime dueDate, int id)
        {
            this.Title = title;
            if(description == null)
            {
                description = "";
            }
            this.Description = description;
            this.creationDate = DateTime.Now;
            this.DueDate = dueDate;
            this.id = id;
            log.Info(String.Format("created new task, title : {0}, description{1}", title, description));
        }

        /// <summary>
        /// constractor from a DAL task
        /// </summary>
        /// <param name="task">DAL task to convert to buissnes layer task</param>
        public Task(DataAccessLayer.Task task)
        {
            this.Title = task.title;
            this.Description = task.description;
            this.dueDate = task.dueDate;
            this.creationDate = task.creationDate;
            this.id = task.id;
        }
        /// <summary>
        /// implement Savable. 
        /// </summary>
        /// <returns>this task as DAL task</returns>
        public override DalObject ToDalObject()
        {
            return new DataAccessLayer.Task(title, description, creationDate, dueDate, id);
        }
    }
}
