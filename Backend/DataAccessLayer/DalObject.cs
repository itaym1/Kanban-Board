
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Respresent savable data acccess layer objects
    /// </summary>
    public abstract class DalObject
    {   
        /// <summary>
        /// write the json string of the object at the right path
        /// </summary>
        public abstract int InsertToDB(DalController dal);
        public abstract void UpdateToDB(DalController dal);
        public abstract void DeleteFromDB(DalController dal);
    }
}
