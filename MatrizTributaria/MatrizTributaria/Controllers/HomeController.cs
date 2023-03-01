using MatrizTributaria.Areas.Cliente.Models;
using MatrizTributaria.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Mime;
//homecontroller BRANCH VITOR
namespace MatrizTributaria.Controllers
{
    public class HomeController : Controller
    {
        readonly MatrizDbContext db;
        /*so um teste*/
        List<AnaliseTributaria> analise = new List<AnaliseTributaria>();
        List<AnaliseTributaria> trib = new List<AnaliseTributaria>();
        List<AnaliseTributaria2> trib2 = new List<AnaliseTributaria2>();
        List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>();
        List<Produto> prodMTX = new List<Produto>();
        Usuario user;
      //  Empresa emp;

        Usuario usuario;
        Empresa empresa;

        public HomeController()
        {
            db = new MatrizDbContext();
        }

        public ActionResult Index()
        {
            
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                //verificar
                if(!(Session["cnpjEmp"].ToString() == "30.272.433/0001-67"))
                {
                    //verificar
                    if (!(Session["cnpjEmp"].ToString() == "15.381.712/0001-75"))
                    {
                        return RedirectToRoute("cliente");
                    }
                    
                }
               
            }
            //string cnpj = Session["cnpjEmp"].ToString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login(string param)
        {
            try
            {
                //pegar a ultima versão
                var versao = db.Versoes.ToList();
                var item = versao[versao.Count - 1];
                ViewBag.Versao = item.versao;
                ViewBag.Nota = item.nota;
            }
            catch(Exception e)
            {
                var erro = e.ToString();
                int par = 7;
                ViewBag.Empresas = db.Empresas;
                return RedirectToAction("../Erro/ErroLogin", new { param = par });
            }
            if(param != "")
            {
                if (param != null)
                {
                    ViewBag.Message = param;
                }
                
            }

            ViewBag.Empresas = db.Empresas;
           
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "email, senha, primeiro_acesso, idEmpresa")] Usuario usuario)
        {
            //int empresaEscolhida = usuario.idEmpresa;

            string hashTxtSenha = null;
            var hash = new Hash(SHA512.Create());
            

            try
            {
                this.user = db.Usuarios.Where(x => x.email == usuario.email).FirstOrDefault();
            }
            catch (Exception e)
            {
                var erro = e.ToString();
                int par = 7;
                ViewBag.Empresas = db.Empresas;
                return RedirectToAction("../Erro/ErroLogin", new { param = par });
            }

            //Usuario user = db.Usuarios.Where(x => x.email == usuario.email).FirstOrDefault();
            //Usuario user = (from u in db.Usuarios where u.email.Equals(usuario.email) select u).FirstOrDefault<Usuario>();
            if (user == null)
            {
                Session["usuario"] = null;
                ViewBag.Message = "Usuário não encontrato!";
                ViewBag.Empresas = db.Empresas;
                return View();
            }
            else
            {
                if(usuario.senha == null)
                {
                    Session["usuario"] = null;
                    ViewBag.Message = "Por favor digite sua senha";
                    ViewBag.Empresas = db.Empresas;
                    return View();
                }

                hashTxtSenha = hash.CriptografarSenha(usuario.senha);

                if (user.senha.Equals(hashTxtSenha) || user.senha.Equals(usuario.senha))
                {
                    if (this.user.primeiro_acesso == 1)
                    {
                        //chamar o modal na view
                        ViewBag.Message = "PRIMEIRO ACESSO";
                        ViewBag.Identificador = this.user.id;
                        ViewBag.Empresas = db.Empresas;
                        return View();
                    }


                    ////se ele nao escolher a empresa, passa com a empresa que ele tem no cadastro
                    //if(empresaEscolhida == 0) //VERIFICA A EMPRESA QUE ELE ESCOLHEU
                    //{
                    Session["idEmpresa"] = user.empresa.id; //se nao esclhou nenhum  a session é com a propria empresa
                    Session["cnpjEmp"] = user.empresa.cnpj;
                    Session["empresa"] = user.empresa.fantasia;

                    //03/08/2022 - verificar se é simples nacional
                    Session["simplesNacional"] = user.empresa.simples_nacional.ToString();
                    TempData["UfOrigem"] = user.empresa.estado.ToString();
                    TempData["UfDestino"] = user.empresa.estado.ToString();
                    

                    if (user.acesso_empresas == 1)
                    {
                        Session["acessoEmpresas"] = "sim";
                    }
                    else
                    {
                        Session["acessoEmpresas"] = null;
                    }
                    //}
                    //else
                    //{
                    //    if (this.user.acesso_empresas == 1)
                    //    {
                    //        this.emp = db.Empresas.Where(x => x.id == empresaEscolhida).FirstOrDefault();
                    //        //se nao, o sistema busca a empresa selecionado e aplica nas sessoes
                    //        Session["idEmpresa"] = this.emp.id; //se nao esclhou nenhum  a session é com a propria empresa
                    //        Session["cnpjEmp"] = this.emp.cnpj;
                    //        Session["empresa"] = this.emp.fantasia;
                    //    }
                    //    else
                    //    {
                    //        //verificar a empresa dele
                    //        int empUsuario = this.user.idEmpresa;
                    //        if(empUsuario != empresaEscolhida) 
                    //        {
                    //            Session["usuario"] = null;
                    //            ViewBag.Message = "Usuário não permitido para essa Empresa";
                    //            ViewBag.Empresas = db.Empresas;
                    //            return View();
                    //        }
                    //        else
                    //        {
                    //            Session["idEmpresa"] = user.empresa.id; //se nao esclhou nenhum  a session é com a propria empresa
                    //            Session["cnpjEmp"] = user.empresa.cnpj;
                    //            Session["empresa"] = user.empresa.fantasia;
                    //        }

                    //    }

                    //}


                    ViewBag.Message = "Bem vindo : " + user.nome;

                    Session["usuario"] = user.nome;
                    
                    Session["email"] = user.email;
                    Session["id"] = user.id;
                    Session["nivel"] = user.nivel.descricao;

                    string usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
                    string empresaUsuario = Session["cnpjEmp"].ToString();
                    //this.usuario = (from a in db.Usuarios where a.nome == usuarioSessao select a).FirstOrDefault(); //pega o usuario


                    try
                    {
                        this.usuario = db.Usuarios.Where(x => x.nome == usuarioSessao).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        var erro = e.ToString();
                        int par = 7;
                        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    }


                    //this.empresa = (from a in db.Empresas where a.cnpj == empresaUsuario select a).FirstOrDefault(); //empresa

                    try
                    {
                        this.empresa = db.Empresas.Where(x => x.cnpj == empresaUsuario).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        var erro = e.ToString();
                        int par = 7;
                        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    }


                    Session["usuarios"] = usuario;
                    Session["empresas"] = empresa;

                    

                }
                else
                {
                    Session["usuario"] = null;
                    ViewBag.Message = "Senha incorreta!";
                    ViewBag.Empresas = db.Empresas;
                    return View();
                }

                
            }

            /*Vitor: 08112022*/
            //Verifica se o Usuario é Preciso Ou NorteSYS
            if (user.empresa.id != 4 && user.empresa.id != 42)
            {
                //Verificar pagamento SuperLogica
                int? idSuperLogica = user.empresa.id_superlogica;
                //validar o cliente na superlogica
                var requisicaoWeb = WebRequest.CreateHttp("https://api.superlogica.net/v2/financeiro/clientes/" + idSuperLogica);

                //passar os tokens
                requisicaoWeb.Method = "GET";
                requisicaoWeb.Headers["app_token"] = "6c7a8c42-3291-39d5-bc1c-1e0ce8e1beef";//Adicionando o AuthToken  no Header da requisição
                requisicaoWeb.Headers["access_token"] = "05deedee-25c2-46ab-8fa3-10cd78f3f297";//Adicionando o AuthToken  no Header da requisição

                //verificando se o cadastro do cliente existe e esta ok
                using (HttpWebResponse resposta = (HttpWebResponse)requisicaoWeb.GetResponse())
                {
                    if (resposta.StatusCode == HttpStatusCode.OK)
                    {
                        //pegando os dados do cliente da resposta da api
                        var streamDados = resposta.GetResponseStream();
                        StreamReader reader = new StreamReader(streamDados);

                        int combranças = 0;
                        string objResponse = reader.ReadToEnd();

                        string[] linhas = objResponse.Split(',');

                        foreach (string linha in linhas)
                        {
                            string texto = linha.Replace('"', '|');
                            if (texto.StartsWith("|quantidade_cobs_atrasadas|"))
                            {
                                string[] campo = texto.Split('|');
                                if (campo[3] != "0")
                                {
                                    combranças = int.Parse(campo[3]);
                                }
                            }
                        }
                        if (combranças > 0)
                        {
                            Session["usuario"] = null;
                            ViewBag.Message = "Não foi possível conectar, motivo: Debitos em atraso!";
                            ViewBag.Empresas = db.Empresas;
                            return View();
                        }

                    }

                }
            }
            ViewBag.Message = "Bem vindo : " + user.nome;
            ViewBag.Empresas = db.Empresas;
            return RedirectToAction("Index", "Home");
        }

        //alteração de senha usuario
      
        public ActionResult LoginAlterar(int? identif, string senhaProv, string novaSenha, string senhaRep)
        {
            string hashTxtSenha = null;
            int? Identif = identif;
            string NovaSenha = novaSenha;
            string SenhaRep = senhaRep;

            TempData["identificador"] = Identif ?? TempData["identificador"]; //se opção != null
            Identif = (Identif == null) ? (int?)TempData["identificador"] : Identif;
            TempData.Keep("identificador");
           //pp

            if (Identif == null)
            {
                string par = "Usuário não encontrato!";
                return RedirectToAction("Login", "Home", new { param = par });
            }
            else
            {
                var hash = new Hash(SHA512.Create());
                var usuario = db.Usuarios.Find(Identif);
                
                hashTxtSenha = hash.CriptografarSenha(senhaProv);

                //verifica se a senha padrao esta correta
                if (usuario.senha.Equals(hashTxtSenha) || usuario.senha.Equals(senhaProv)) 
                {
                    //verifica se as senhas batem
                    if (novaSenha != senhaRep)
                    {
                        string par = "As senhas digitadas não são iguais!";
                        return RedirectToAction("Login", "Home", new { param = par });
                    }
                    else
                    {
                        //aqui
                        //criptografar senha
                        usuario.senha = hash.CriptografarSenha(novaSenha);
                        usuario.primeiro_acesso = 0;
                        usuario.ativo = 1;
                        usuario.dataAlt = DateTime.Now;
                

                        try
                        {
                            db.SaveChanges();
                            string par = "Senha alterada com sucesso. Efetue o login.";
                            return RedirectToAction("Login", "Home", new { param = par });
                        }
                        catch
                        {
                            string par = "Problemas ao salvar a senha, tente novamente.";
                            return RedirectToAction("Login", "Home", new { param = par });
                        }


                    }
                }
                else
                {
                    string par = "Senha provisória incorreta";
                    return RedirectToAction("Login", "Home", new { param = par });
                }

                

            }

           // return null;
        }

        public ActionResult SenhaAlterar(string emailUsu)
        {
            //pegar o email
            string e_mail = emailUsu;

           
            //verifica se o email contem carcteres válidos
            if (!IsValidEmail(emailUsu.ToLower()))
            {
                ViewBag.Message = "O e-mail informado é inválido";
                return View("Login");
            }
            //verificar se existe realmente
            Usuario user = db.Usuarios.FirstOrDefault(x => x.email.ToLower().Equals(emailUsu));
            if(user == null)
            {
                ViewBag.Message = "E-mail não encontrado na base de dados.";
                return View("Login");
            }
            else //se não, encaminha o email
            {
                //gerar uma senha provisória
                string senha_provisoria = alfanumericoAleatorio(8);
                user.senha = senha_provisoria;
                user.primeiro_acesso = 1;
                user.dataAlt = DateTime.Now;

                //envio do email com a senha provisória
               
                SmtpClient smtp = new SmtpClient();

                //smtp.Host = "smtp.gmail.com";
                smtp.Host = "smtpout.secureserver.net"; 
                smtp.Port = 587;
                //smtp.Port = 465;

                smtp.EnableSsl = false;

                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new System.Net.NetworkCredential("suporte@precisomtx.com.br", "MTX@12345");
                //smtp.Credentials = new System.Net.NetworkCredential("desenvolvimentomtx@gmail.com", "kzplodtqicuytgpa");



                MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress("suporte@precisomtx.com.br");
                if (!string.IsNullOrWhiteSpace(emailUsu.ToLower()))
                {
                    mail.To.Add(new System.Net.Mail.MailAddress(emailUsu.ToLower()));
                }
                else
                {
                    ViewBag.Message = "O e-mail informado é inválido";
                    return View("Login");
                }
                mail.Subject = "Senha Provisória - PrecisoMtx";
                mail.Body = "Segue informações de usuário e senha provisórios:\n ";
                mail.Body += "Usuário: " + emailUsu.ToLower() + "\n";
                mail.Body += "Senha: " + senha_provisoria + "\n";
                mail.Body += "Obs.: Utilize essa senha provisória no proximo acesso e será solicitado que a mesma seja alterada. \n";
                mail.Body += "Acesse: " + "http://18.223.22.3/Home/Login" + " para efetuar o  Login e alterar a senha \n";
               

                //envio de email
                try
                {

                    try
                    {
                        smtp.Send(mail);
                        db.SaveChanges();
                    }
                    catch (SmtpFailedRecipientException ex)
                    {
                        ViewBag.Message = "Problemas no envio da senha provisória";
                        return View("Login");
                    }

                    ViewBag.Message = "Uma senha provisória foi encaminhada para seu e-mail.";
                    return View("Login");



                }
                catch (SmtpException e)
                {
                    ViewBag.Message = "Problemas no envio da senha provisória";
                    return View("Login");
                }


            }



            return null;
        }
        public ActionResult LogOut()
        {
            if (Session["usuario"] != null)
            {
                Session["usuario"] = null;
                Session["empresa"] = null;
                Session["email"] = null;
                TempData["analise"] = null;
                TempData["tributacaoMTX"] = null;
                TempData["tributacaoProdMTX"] = null;
                TempData["usuarioEmpresa"] = null;
                Session["usuarios"] = null;
                Session["empresas"] = null;
                Session["simplesNacional"] = null;
                TempData["procuraCAT"] = null;
                TempData["procuraPor"] = null;

                //cliente
                TempData["prdInexistente"] = null;
                TempData["analise2"] = null;
                TempData["UfOrigem"] = null;
                TempData["UfDestino"] = null;
                TempData["procuraPor"] = null;
                TempData["analiseSN"] = null;
                TempData["tributacao"] = null;
                TempData["analise_NCM"] = null;
                TempData["tributacaoMTX_NCMView"] = null;
                TempData["linhas"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

        public static string alfanumericoAleatorio(int tamanho)
        {
            var chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz123456789@#$*";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}