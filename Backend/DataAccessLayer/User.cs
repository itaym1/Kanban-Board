using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class User : DalObject
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Simple construction
        /// </summary>
        /// <param name="email"> User email </param>
        /// <param name="nickname"> User nickname </param>
        /// <param name="password"> User Password </param>
		public User(string email, string nickname, string password)
		{
			Email = email;
			Nickname = nickname;
			Password = password;
		}

        /// <summary>
        /// Inserts this user to data base
        /// </summary>
        /// <param name="dal"></param>
        /// <returns>returns -1 if there is no id in data base</returns>
        public override int InsertToDB(DalController dal)
        {
            string query = String.Format("INSERT INTO USERS (Email, Nickname, Password) VALUES ({0}, {1}, {2})", "'" + Email + "'", "'" + Nickname + "'", "'" + Password + "'");
            dal.runQuery(query);
            return -1; //no id in db
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="dal"></param>
        public override void UpdateToDB(DalController dal)
        {
            throw new NotImplementedException(); //not needed for user
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="dal"></param>
        public override void DeleteFromDB(DalController dal)
        {
            throw new NotImplementedException(); //not needed for user
        }
    }
}


