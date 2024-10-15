using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class RecenzijaRequestHelper
    {
        public string idleta;
        public string naslov;
        public string sadrzaj;
        public string slika;

        public RecenzijaRequestHelper() { }

        public RecenzijaRequestHelper(string idleta, string naslov, string sadrzaj, string slika)
        {
            this.idleta = idleta;
            this.naslov = naslov;
            this.sadrzaj = sadrzaj;
            this.slika = slika;
        }
    }
}