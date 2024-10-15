using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Web;

namespace Projekat.Models
{
    public enum TipKorisnika
    {
        Putnik,
        Administator
    }

    public class Korisnik
    {
        [JsonIgnore]
        public List<Rezervacija> rezervacije;
        
        public string korisnickoime;
        public string lozinka;
        public string ime;
        public string prezime;
        public string email;
        public string datumrodjenja;
        public bool pol;
        public TipKorisnika tipKorisnika;

        public Korisnik() {
            rezervacije = new List<Rezervacija>();
        }

        public Korisnik(string korisnickoime, string lozinka, string ime, string prezime, string email, string datumrodjenja, bool pol, TipKorisnika tipKorisnika)
        {
            this.korisnickoime = korisnickoime;
            this.lozinka = lozinka;
            this.ime = ime;
            this.prezime = prezime;
            this.email = email;
            this.datumrodjenja = datumrodjenja;
            this.pol = pol;
            this.tipKorisnika = tipKorisnika;
            this.rezervacije = new List<Rezervacija>();
        }
    }
}