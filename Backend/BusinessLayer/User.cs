using IntroSE.Kanban.Backend.DataAccessLayer;
using System;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User : Saveable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string Email;
        private readonly string Nickname;
        private readonly string Password;

        /// <summary>
        /// basic user constractor
        /// </summary>
        /// <param name="email">email of user</param>
        /// <param name="password">password of user</param>
        /// <param name="nickname">nuckname of user</param>
        public User(string email, string password, string nickname)
        {
            Email = email;
            Nickname = nickname;
            Password = password;
            log.Info(String.Format("created new user, email : {0}, nickname: {1}", Email, Nickname));
        }

        public string GetEmail()
        {
            return Email;
        }
        public string GetNickname()
        {
            return Nickname;
        }
        public string GetPassword()
        {
            return Password;
        }

        /// <summary>
        /// converts the User to a DAL user
        /// </summary>
        /// <returns>same user but DAL user</returns>
        public override DalObject ToDalObject()
        {
            return new DataAccessLayer.User(Email, Nickname, Password);
        }

        /// <summary>
        /// user constractor from DAL user
        /// </summary>
        /// <param name="dalUser">DAL user to convert to buissnes layer user</param>
        public User(DataAccessLayer.User dalUser)
        {
            Email = dalUser.Email;
            Nickname = dalUser.Nickname;
            Password = dalUser.Password;
        }
    }
}
