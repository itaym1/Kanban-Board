using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public abstract class Saveable
    {
        /// <summary>
        /// turns the buisness layer object to DAL object
        /// </summary>
        /// <returns>DalObject User / Board </returns>
        public abstract DalObject ToDalObject();

        /// <summary>
        /// update object at data base.
        /// </summary>
        /// <param name="dc">DAL Controller</param>
        public void Update(DalController dc)
        {
            dc.UpdateObjectToDB(ToDalObject());
        }
        /// <summary>
        /// insert object to data base.
        /// </summary>
        /// <param name="dc">DAL Controller</param>
        public void Insert(DalController dc)
        {
            dc.InsertObjectToDB(ToDalObject());
        }
        /// <summary>
        /// delete object from data base.
        /// </summary>
        /// <param name="dc">DAL Controller</param>
        public void Delete(DalController dc)
        {
            dc.DeleteFromDB(ToDalObject());
        }
    }
}
