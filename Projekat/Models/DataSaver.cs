using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public static class DataSaver
    {
        public static List<Korisnik> korisnici;
        public static List<Recenzija> recenzije;
        public static List<Aviokompanija> aviokompanije;
        public static Dictionary<string, Korisnik> logovaniKorisnici = new Dictionary<string, Korisnik>();
        public static List<Let> letovipom;
        public static List<Rezervacija> rezervacijepom;

        public static List<Let> letovi{
            get
            {
                Updater();
                return letovipom;
            }
            set
            {
                letovipom = value;
            }
        }
        public static List<Rezervacija> rezervacije
        {
            get
            {
                Updater();
                return rezervacijepom;
            }
            set
            {
                rezervacijepom = value;
            }
        }   

        public static void Updater()
        {

            //update  status
            foreach (Let let in DataSaver.letovipom)
            {
                if (let.isDeleted == false)
                {
                    if (let.status == StatusLeta.Aktivan)
                    {
                        if (let.vremedolaska < DateTime.Now)
                        {
                            let.status = StatusLeta.Zavrsen;
                        }
                    }
                }
            }

            foreach (Rezervacija rezervacija in DataSaver.rezervacijepom)
            {
                if (rezervacija.isDeleted == false)
                {
                    if (rezervacija.let.status == StatusLeta.Zavrsen)
                    {
                        rezervacija.status = StatusRezervacija.Zavrsena;
                    }
                    else if (rezervacija.let.status == StatusLeta.Otkazan)
                    {
                        rezervacija.status = StatusRezervacija.Otkazana;
                    }
                }
            }
        }

        public static void saveKorisnici()
        {
            File.WriteAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\korisnici.json", JsonConvert.SerializeObject(korisnici, Formatting.Indented));

        }

        public static void saveLetovi()
        {
           File.WriteAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\letovi.json", JsonConvert.SerializeObject(letovi, Formatting.Indented));
        }

        public static void saveRecenzije()
        {
            File.WriteAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\recenzije.json", JsonConvert.SerializeObject(recenzije, Formatting.Indented));
        }

        public static void saveRezervacije()
        {
            saveLetovi();
            File.WriteAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\rezervacije.json", JsonConvert.SerializeObject(rezervacije, Formatting.Indented));
        }

        public static void saveAviokompanije()
        {
            File.WriteAllText("D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Podatke\\aviokompanije.json", JsonConvert.SerializeObject(aviokompanije, Formatting.Indented));
        }
    }
}