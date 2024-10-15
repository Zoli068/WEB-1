using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Projekat.Models
{
    public class Aviokompanija
    {
        [JsonIgnore]
        public List<Let> letovi;
        [JsonIgnore]
        public List<Recenzija> recenzije;

        public bool isDeleted; 

        public string naziv;
        public string adresa;
        public string kontaktinformacije;

        public Aviokompanija()
        {
            letovi= new List<Let>();
            isDeleted=false;
            recenzije =new List<Recenzija>();
        }

        public Aviokompanija(string naziv,string adresa,string kontaktinformacije)
        {
            this.naziv = naziv;
            this.adresa = adresa;
            this.kontaktinformacije = kontaktinformacije;
            isDeleted=false;
            letovi = new List<Let>();
            recenzije = new List<Recenzija>();
        }
    }
}