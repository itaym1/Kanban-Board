using IntroSE.Kanban.Backend.BusinessLayer;
using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// handles all the service requests that are related to user.
    /// </summary>
    class UserService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private UserController UC;
        private string LoggedUser;

        /// <summary>
        /// A simple public constructor
        /// </summary>
        public UserService()
        {
            UC = new UserController();
            LoggedUser = null;
        }

        /// <summary>
        /// requests to load all the users data to UC.
        /// </summary>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response LoadData()
        {
            try
            {
                UC.LoadData();
            }
            catch (Exception e)
            {
                log.Error("failed to load data!, exception: " + e.Message);
                return new Response(e.Message);
            }
            return new Response();
        }


        /// <summary>
        /// return true if the user that its email is the email param is true
        /// </summary>
        /// <param name="email">the email of the user to check that is logged in</param>
        /// <returns></returns>
        internal bool IsLoggedIn(string email) {
            return email.Equals(LoggedUser);
        }

        /// <summary>
        /// logins a user to the system.
        /// </summary>
        /// <param name="email">the email of the user to log in</param>
        /// <param name="password">the password of the user to log in</param>
        /// <returns></returns>
        public Response<User> Login(string email, string password)
        {
            try
            {
                if (LoggedUser != null)
                    throw new Exception("User " + LoggedUser + " is already logged in.");
                    string nickname = UC.Login(email, password);
                LoggedUser = email;
                log.Info(String.Format("user {0} logged in at: {1}", email, DateTime.Now.ToString()));
                return new Response<User>(new User(email, nickname));
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to login, exception: {0}, stacktrace: {1}, email: {2}, password: {3} ", e.Message, e.StackTrace, email, password));
                return new Response<User>(e.Message);
            }
        }

        /// <summary>
        /// Registers a user to the system using email, password and nickname
        /// </summary>
        /// <param name="email">the email of the user to register to the system</param>
        /// <param name="password">the password of the user to register to the system</param>
        /// <param name="nickname">the nickname of the user to register to the system</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response Register(string email, string password, string nickname)
        {
            try
            {
                UC.Register(email, password, nickname);
                return new Response();
            }
            catch (Exception e)
            {
                log.Error(String.Format("failed to create new board for email : {0}, nickname: {1}, exception: {2}", email, nickname, e.StackTrace));
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// logsout a user from the system
        /// </summary>
        /// <param name="email">the email of the user to logout</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        public Response Logout(string email)
        {
            LoggedUser = null;
            log.Info(String.Format("user {0} logged out at: {1}", email, DateTime.Now.ToString()));
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
                UC.DeleteData();
            }
            catch (Exception e)
            {
                log.Error("failed to delete data!, exception: " + e.Message);
                return new Response(e.Message);
            }
            return new Response();
        }
    }
}