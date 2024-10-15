using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public enum StatusLeta
    {
        Aktivan,
        Otkazan,
        Zavrsen
    }

    public class Let
    {
        [JsonIgnore]
        public Aviokompanija aviokompanija;

        public string identificator;
        public string nazivkompanija;
        public bool isDeleted;

        public string polaznadestinacija;
        public string odredisnadestinacija;
        public DateTime vremepolaska;
        public DateTime vremedolaska;
        public int slobodnihmesta;
        public int zauzetihmesta;
        public double cena;
        public StatusLeta status;

        public Let() { }

        public Let(Aviokompanija aviokompanija, string polaznadestinacija, string odredisnadestinacija, DateTime vremepolaska, DateTime vremedolaska, int slobodnihmesta, int zauzetihmesta, double cena, StatusLeta status)
        {
            this.identificator=Guid.NewGuid().ToString();
            this.isDeleted = false;
            this.aviokompanija = aviokompanija;
            this.nazivkompanija = aviokompanija.naziv;
            this.polaznadestinacija = polaznadestinacija;
            this.odredisnadestinacija = odredisnadestinacija;
            this.vremepolaska = vremepolaska;
            this.vremedolaska = vremedolaska;
            this.slobodnihmesta = slobodnihmesta;
            this.zauzetihmesta = zauzetihmesta;
            this.cena = cena;
            this.status = status;
        }       
        public Let(string nazivaviokompanija, string identificator, string polaznadestinacija, string odredisnadestinacija, DateTime vremepolaska, DateTime vremedolaska, int slobodnihmesta, int zauzetihmesta, double cena, StatusLeta status)
        {
            this.isDeleted = false;
            this.identificator = identificator;
            this.nazivkompanija = nazivaviokompanija;
            this.polaznadestinacija = polaznadestinacija;
            this.odredisnadestinacija = odredisnadestinacija;
            this.vremepolaska = vremepolaska;
            this.vremedolaska = vremedolaska;
            this.slobodnihmesta = slobodnihmesta;
            this.zauzetihmesta = zauzetihmesta;
            this.cena = cena;
            this.status = status;
        }
    }
}