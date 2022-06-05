namespace E_Commerce.Areas.Admin.Controllers
{
    public class AuthorizationClass  // controllerdan çalışması için controllerların içinde olması gerekir.
    {
        public bool IsAuthorized(string authorization, ISession session) // tek method yetkiyi sorgulama 
        {          
            string? sessionAuthorization = session.GetString(authorization);
            if(sessionAuthorization == "True")
            {
                return true;
            }
            return false;
        }  
       

    }

}

