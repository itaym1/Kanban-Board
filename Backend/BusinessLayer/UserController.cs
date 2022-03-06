using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    class UserController
    {
        private Dictionary<string, User> Users;
        private DataAccessLayer.DalController DC;
        private const int MIN_PASS_LENGTH = 5;
        private const int MAX_PASS_LENGTH = 25;
        /// <summary>
        /// basic constractor
        /// </summary>
        public UserController()
        {
            Users = new Dictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);
            DC = new DataAccessLayer.DalController();
        }

        /// <summary>
        /// registers a user if input is legal and the email does not allready exists
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">user's password</param>
        /// <param name="nickname">user's nicname</param>
        internal void Register(string email, string password, string nickname)
        {
            if(Users.ContainsKey(email))
            {
                throw new ArgumentException("Email: " + email + " already exists in the system");
            }
            try 
            {
                isValidEmail(email);
                isValidPassword(password);
                isValidNickname(nickname);
                User u = new User(email, password, nickname);
                Users.Add(email, u);
                u.Insert(DC);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.StackTrace);
            }
        }
        /// <summary>
        /// Check for validation of the password
        /// </summary>
        /// <param name="pass">The password needed to be check</param>
        /// <returns>An empty string for valid password or error message for invalid password</returns>
        private void isValidPassword(string pass)
        {
            if (pass == null | pass.Length < MIN_PASS_LENGTH | pass.Length > MAX_PASS_LENGTH)
                throw new ArgumentOutOfRangeException ("Password must be in length of 4 to 20 characters.");
            bool upperCase = false;
            bool number = false;
            bool lowerCase = false;
            bool hasSpace = false;
            for (int i = 0; i < pass.Length & (!upperCase | !number | !lowerCase | !hasSpace); i++)
            {
                if (pass[i] >= 'A' & pass[i] <= 'Z')
                    upperCase = true;
                else if (pass[i] >= 'a' & pass[i] <= 'z')
                    lowerCase = true;
                else if (pass[i] >= '0' & pass[i] <= '9')
                    number = true;
                else if (pass[i].Equals(' '))
                    hasSpace = true;
            }
            if (!upperCase | !number | !lowerCase | hasSpace)
                throw new ArgumentOutOfRangeException ("Password must include at least one uppercase letter, one small character and a number.");
        }

        /// <summary>
        /// checks if the email is valid
        /// </summary>
        /// <param name="email">email to check</param>
        private void isValidEmail(string email)
        {
            if (email == null)
            {
                throw new Exception("Email cannot be null.");
            }
            Regex emailReg = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z",
                  RegexOptions.IgnoreCase); //lettrs, @, lettters, . and 2 to 4 letters.
            if (!emailReg.IsMatch(email))
            {
                throw new Exception("Illegal email.");
            }
        }

        /// <summary>
        /// checks if the nickname is valid
        /// </summary>
        /// <param name="nickname">nickname to check</param>
        private void isValidNickname(string nickname)
        {
            if (nickname == null || nickname.Length == 0)
            {
                throw new Exception("nickname cannot be null or empty.");
            }
        }

        /// <summary>
        /// logges the user in if input is valid and the user is already registered
        /// </summary>
        /// <param name="email">email to log in</param>
        /// <param name="password">password to log in</param>
        /// <returns>returns the users nickname if logged in successfully</returns>
        internal string Login(string email, string password)
        {
            User u;
            if (Users.TryGetValue(email, out u))
            {
                if (!u.GetPassword().Equals(password))
                {
                    throw new ArgumentException("Password does not match the email: " + email);
                }
            }
            else
            {
                throw new ArgumentException("Email: " + email + " does not registered");
            }
            return u.GetNickname();
        }

        /// <summary>
        /// loads all the users
        /// </summary>
        public void LoadData()
        {
           List<DataAccessLayer.User> list= DC.LoadUsers();
            foreach (DataAccessLayer.User user in list)
            {
                Users.Add(user.Email, new User(user));
            }
        }
        /// <summary>
        /// Delete all users from data base and clear users list.
        /// </summary>
        public void DeleteData()
        {
            DC.deleteAllUsers();
            Users = new Dictionary<string, User>(StringComparer.InvariantCultureIgnoreCase);
        }

    }
}
