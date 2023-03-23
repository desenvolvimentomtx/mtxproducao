using System.Web;
using System.Web.Optimization;

namespace MatrizTributaria
{
    public class BundleConfig
    {
        // Para obter mais informações sobre o agrupamento, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use a versão em desenvolvimento do Modernizr para desenvolver e aprender. Em seguida, quando estiver
            // pronto para a produção, utilize a ferramenta de build em https://modernizr.com para escolher somente os testes que precisa.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerydatatables").Include(
                     "~/Scripts/jquery.dataTables.min.js"));



            bundles.Add(new ScriptBundle("~/bundles/jquerymaskedinput").Include(
                     "~/Scripts/jquery.maskedinput.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymask").Include(
                    "~/Scripts/jquery.mask.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                     "~/Scripts/popper.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/wztooltip").Include(
                     "~/Scripts/wz_tooltip.js"));

            /*Script do adm*/
            bundles.Add(new ScriptBundle("~/bundles/adm-script").Include(
                     "~/Scripts/admin/adm-script.js",
                     "~/Scripts/bootnavbar.js"));
            bundles.Add(new ScriptBundle("~/bundles/adm-script-produto").Include(
                "~/Scripts/admin/adm-script-produto.js",
                "~/Scripts/bootnavbar.js"));

            /*Script do login*/
            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                     "~/Scripts/vendor/jquery/jquery.min.js",
                     "~/Scripts/vendor/bootstrap/js/bootstrap.bundle.min.js",
                     "~/Scripts/vendor/jquery-easing/jquery.easing.min.js",
                     "~/Scripts/sb-admin-2.min.js",
                     "~/Scripts/toastr.js"));

            bundles.Add(new StyleBundle("~/Content/cssLogin").Include(
                      "~/Content/fontawesome-free/css/all.min.css",
                      "~/Content/sb-admin-2.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/fontawesome-all.css",
                      "~/Content/admin/adm-css.css",
                      "~/Content/bootnavbar.css",
                      "~/Content/toastr.css"));


        }
    }
}
