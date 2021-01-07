using PlateformeConcours.helpers;
using PlateformeConcours.Models;
using PlateformeConcours.Repositories;
using PlateformeConcours.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace PlateformeConcours.Controllers
{
    public class SignupController : Controller
    {
        public string[] ImageEx = new string[] { ".png", ".jpg", ".jpeg", ".jfif", ".pdf" };
        SignupViewModel model;
        ApplicationDbContext applicationDbContext;
        // GET: Signup
        public SignupController()
        {
            applicationDbContext = new ApplicationDbContext();
            model = new SignupViewModel();
            List<Country> countries = GetCountries().ToList();
            List<Filiere> filieres = applicationDbContext.Filieres.ToList();
            model.selectF = new SelectList(filieres, "ID", "Titre");
            model.selectC = new SelectList(countries, "Name", "Name");
            model.countries = countries;
            model.filiers = filieres;
        }
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            //Mailer.send();
            model.etudiant.Password = "";
            model.etudiant.Nom = "";
            return View(model);
        }
        public JsonResult emailExists(string email)
        {
            Etudiant etu = applicationDbContext.Etudiants.Where(s => s.Email == email).FirstOrDefault<Etudiant>();
            if (etu != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        IEnumerable<Country> GetCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                 .Select(x => new Country
                 {
                     ID = new RegionInfo(x.LCID).Name,
                     Name = new RegionInfo(x.LCID).EnglishName
                 })
                                 .GroupBy(c => c.ID)
                                 .Select(c => c.First())
                                 .OrderBy(x => x.Name);
        }

        [HttpPost()]
        public int Submit(String cne, String cin, String email)
        {
            if (ModelState.IsValid)
            {
                return 1;
            }
            /*var exist = db.Etudiants.Where(
				e => e.Email.Equals(email) ||
				e.Cin.Equals(cin) ||
				e.Cne.Equals(cne)
			);
			if(exist.Count()>0)
			{
				model.isError = true;
				model.message = "E-mail, CIN ou CNE existe déjà";
				model.cin = cin;
				model.cne = cne;
				model.email = email;
			}
			else
			{
				model.message = "Votre compte a été créé vérifiez votre email";
				Etudiant e = new Etudiant();
				e.Cin = cin;
				e.Cne = cne;
				e.Email = email;
				e.Password = RandomString(8);
				db.Etudiants.Add(e);
				db.SaveChanges();
			}*/
            //return View("Index");
            return 0;
        }
        public async System.Threading.Tasks.Task<bool> SendEmailAsync(int id)
        {
            string Baseurl = "https://localhost:44344/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", "vDdzzWaXf8866Gbsdzfz788221Afd");

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/email/signup/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var response = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    if (Res.ToString().Equals("ok")) return true;

                }
                return false;
            }

        }
        [HttpPost]
        public ActionResult edit(SignupViewModel std, string civilite, string nationalite, HttpPostedFileBase picture, string filiere, string Choixdannee, string typeDiplome, HttpPostedFileBase fichier)
        {
            StudentRepository studentRepository = new StudentRepository(applicationDbContext);
            std.etudiant.Nationnalite = nationalite;
            std.etudiant.Civilite = civilite;
            std.etudiant.ID_Filiere = Convert.ToInt32(filiere);


            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
            try
            {
                if (ModelState.IsValid)
                {
                    std.etudiant.Etat = "Inscrit";
                    if (Choixdannee == "3eme")
                    {
                        studentRepository.InsertStudent3emeAnnee(std.etudiant);

                    }
                    else if (Choixdannee.Equals("4eme"))
                    {
                        studentRepository.InsertStudent4emeAnnee(std.etudiant);

                    }
                    if (fichier != null)
                    {

                        var v = Request.Files.Count;
                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        string pic = Path.GetFileName(fichier.FileName);
                        string newName = timeStamp + "_" + pic;
                        std.etudiant.Diplome.fichier = newName;
                        string path = Path.Combine(Server.MapPath("~/images/"), newName);
                        // file is uploaded
                        fichier.SaveAs(path);
                        applicationDbContext.SaveChanges();



                    }
                    else
                    {
                        ViewBag.erreur = " vous devez selectionner un fichier";


                    }
                    if (picture != null)
                    {

                        var v = Request.Files.Count;

                        string pic = Path.GetFileName(picture.FileName);
                        string newName = pic;
                        std.etudiant.picture = newName;
                        string path = Path.Combine(Server.MapPath("~/images/"), newName);
                        // file is uploaded
                        picture.SaveAs(path);
                        applicationDbContext.SaveChanges();

                    }
                    else
                    {
                        ViewBag.erreur = " vous devez selectionner une photo";


                    }


                    model.isError = false;
                    model.message = "vous avez bien été inscris, connectez vous à votre compte";
                    SendEmailAsync(std.etudiant.ID);
                    TempData["Message"] = "success";
                    return Redirect("~/");
                    //return "student saved successfully";
                    //return std.etudiant.Cin+" nom "+std.etudiant.Nom +" prenom"+std.etudiant.Prenom +"s1 "+std.etudiant.Note.S1+"filiere "+ filiere + " civil " + nationalite;
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "error";
                return RedirectToAction("index");
            }
            model.isError = true;
            model.message = "une erreur a été rencontré, s'il vous plait essayer une autre fois et vérifiez vos informations!";
            return View("index", model);
            //return "something went wrong";
            //return std.etudiant.Cin + " nom " + std.etudiant.Nom + " prenom" + std.etudiant.Prenom + "s1 " + std.etudiant.Note.S1 + "filiere " + filiere + " natio,amite" + nationalite;
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
