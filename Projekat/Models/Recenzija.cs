using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public enum StatusReceznija
    {
        Kreirana,
        Odobrena,
        Odbijena
    }

    public class Recenzija
    {
        [JsonIgnore]
        public Korisnik recezent;
        [JsonIgnore]
        public Aviokompanija aviokompanija;

        public bool isDeleted;
        public string idleta;
        public string recezentusername;
        public string nazivkompanija;

        public string naslov;
        public string sadrzaj;
        public string putanjaslike;
        public StatusReceznija status;

        public Recenzija() { }

        public Recenzija(string recezentusername,string idrecenzija, string nazivkompanija, string naslov, string sadrzaj, string putanjaslike, StatusReceznija status)
        {
            this.isDeleted = false;
            this.idleta = idrecenzija;
            this.recezentusername = recezentusername;
            this.nazivkompanija = nazivkompanija;
            this.naslov = naslov;
            this.sadrzaj = sadrzaj;
            this.putanjaslike = putanjaslike;
            this.status = status;
        }

        public Recenzija(Korisnik recezent, Aviokompanija aviokompanija, string naslov, string sadrzaj, string putanjaslike, StatusReceznija status)
        {
            this.isDeleted = false;
            this.recezentusername = recezent.korisnickoime;
            this.nazivkompanija = aviokompanija.naziv;
            this.recezent = recezent;
            this.aviokompanija = aviokompanija;
            this.naslov = naslov;
            this.sadrzaj = sadrzaj;
            this.putanjaslike = putanjaslike;
            this.status = status;
        }
    }
}