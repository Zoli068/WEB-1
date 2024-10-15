using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Projekat
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        
            loadData();
        }

        private void loadData()
        {
            DataSaver.korisnici = JsonConvert.DeserializeObject<List<Korisnik>>(File.ReadAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\korisnici.json"));
            DataSaver.letovipom = JsonConvert.DeserializeObject<List<Let>>(File.ReadAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\letovi.json"));
            DataSaver.recenzije = JsonConvert.DeserializeObject<List<Recenzija>>(File.ReadAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\recenzije.json"));
            DataSaver.rezervacijepom = JsonConvert.DeserializeObject<List<Rezervacija>>(File.ReadAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\rezervacije.json"));
            DataSaver.aviokompanije = JsonConvert.DeserializeObject<List<Aviokompanija>>(File.ReadAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\aviokompanije.json"));

            foreach(Aviokompanija aviokompanija in DataSaver.aviokompanije)
            {
                foreach(Let let in DataSaver.letovipom)
                {
                    if (let.nazivkompanija.Equals(aviokompanija.naziv))
                    {
                        let.aviokompanija = aviokompanija;
                        aviokompanija.letovi.Add(let);
                    }
                }

                foreach(Recenzija recenzija in DataSaver.recenzije)
                {
                    if (recenzija.nazivkompanija.Equals(aviokompanija.naziv))
                    {
                        recenzija.aviokompanija=aviokompanija;
                        aviokompanija.recenzije.Add(recenzija);
                    }
                }
            }

            foreach(Korisnik k in DataSaver.korisnici)
            {
                foreach(Recenzija recenzija in DataSaver.recenzije)
                {
                    if (recenzija.recezentusername.Equals(k.korisnickoime))
                    {
                        recenzija.recezent = k;
                    }
                }

                foreach(Rezervacija rezervacija in DataSaver.rezervacijepom)
                {
                    if (rezervacija.korisnickoime.Equals(k.korisnickoime)){
                        rezervacija.korisnik = k;
                        k.rezervacije.Add(rezervacija);
                    }
                }
            }

            foreach(Rezervacija rezervacija in DataSaver.rezervacijepom)
            {
                foreach(Let let in DataSaver.letovipom)
                {
                    if (let.identificator.Equals(rezervacija.idleta))
                    {
                        rezervacija.nazivAviokompanija = let.aviokompanija.naziv;
                        rezervacija.let = let;
                    }
                }
            }

            DataSaver.Updater();
        }
    }
}
