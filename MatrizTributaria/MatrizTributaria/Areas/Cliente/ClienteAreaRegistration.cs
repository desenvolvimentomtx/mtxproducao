using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente
{
    public class ClienteAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Cliente";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Cliente_default",
                "Cliente/{controller}/{action}/{id}",
                new { controller = "HomeCliente", action = "HomeCliente", id = UrlParameter.Optional }
            );
        }
    }
}