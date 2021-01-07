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
    public class EpreuveController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //1er view :: contient table des filiere avec (button liste des epreuves -> vers une table des pdf)+(button vers form d'insertion des pdf)    
        public ActionResult Index()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Filiere> Filieres = new List<Filiere>();
            return View(db.Filieres.ToList());
        }




        // la liste des fichires d'une filiere
        public ActionResult ListFilesParFiliere(int? id_fil)
        {

            List<Models.File> listfiles = db.Filieres.Where(f => f.ID == id_fil).Single().files.ToList();

            return View(listfiles);
        }


        //upload file
        public ActionResult Get(int ID)
        {


            
            Models.File file = db.files.Find(ID);

            //If file exists....
            ViewBag.Id_fil = new SelectList(db.Filieres, "Id_filiere", "Nom_filiere");
            MemoryStream ms = new MemoryStream(file.Content, 0, 0, true, true);
            Response.ContentType = file.Type;
            Response.AddHeader("content-disposition", "attachment;filename=" + file.Name);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            ViewBag.filieres = new SelectList(db.Filieres, "id_fil", "Nom_fil");


            return new FileStreamResult(Response.OutputStream, file.Type);


        }

        


    }
}
