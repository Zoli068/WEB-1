using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public enum StatusRezervacija
    {
        Kreirana,
        Odobrena,
        Otkazana,
        Zavrsena
    }

    public class Rezervacija
    {
        [JsonIgnore]
        public Korisnik korisnik;
        [JsonIgnore]
        public Let let;

        public string korisnickoime;
        public string idleta;
        public string nazivAviokompanija;
        public bool isDeleted;

        public int brojputnika;
        public double ukupnacena;
        public StatusRezervacija status;

        public Rezervacija() { }

        public Rezervacija(Korisnik korisnik, Let let, int brojputnika, double ukupnacena, StatusRezervacija status)
        {
            this.isDeleted = false;
            this.korisnickoime = korisnik.korisnickoime;
            this.nazivAviokompanija = let.aviokompanija.naziv;
            this.idleta = let.identificator;
            this.korisnik = korisnik;
            this.let = let;
            this.brojputnika = brojputnika;
            this.ukupnacena = ukupnacena;
            this.status = status;
        }

        public Rezervacija(string korisnickoime, string idleta, int brojputnika, double ukupnacena, StatusRezervacija status)
        {
            this.isDeleted = false;
            this.korisnickoime = korisnickoime;
            this.idleta=idleta;
            this.ukupnacena = ukupnacena;
            this.status=status;
        }
    }
}