  using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Projekat.Controllers
{
    [RoutePrefix("rezervacija")]
    public class RezervacijaController : ApiController
    {
        [HttpGet]
        [Route("getall")]
        public IHttpActionResult Getall()
        {
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            List<Rezervacija> responseRezervacije = new List<Rezervacija>();

            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        foreach (Rezervacija rezervacija in DataSaver.rezervacije)
                        {
                            if (rezervacija.isDeleted == false)
                            {
                                responseRezervacije.Add(rezervacija);
                            }
                        }

                        return Ok(responseRezervacije);
                    }
                }
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("userrezervacije")]
        public IHttpActionResult Userletovi()
        {
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            List<Rezervacija> responseRezervacije = new List<Rezervacija>();

            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    foreach (Rezervacija rezervacija in DataSaver.rezervacije)
                    {
                        if (rezervacija.korisnickoime.Equals(prethodniKorisnik.korisnickoime))
                        {
                            if (rezervacija.isDeleted == false)
                            {
                                responseRezervacije.Add(rezervacija);
                            }
                        }
                    }

                    return Ok(responseRezervacije);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("addrezervacija")]
        public IHttpActionResult Addrezervacija(Rezervacija rezervacija)
        {
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";
            Let letZaRezervaciju = null;

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }
            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {

                    if (rezervacija.idleta == null || rezervacija.idleta.Trim() == "" || rezervacija.brojputnika <= 0)
                    {
                        return BadRequest();
                    }

                    foreach (Rezervacija rez in DataSaver.rezervacije)
                    {
                        if (rez.idleta.Equals(rezervacija.idleta))
                        {
                            if (rez.korisnickoime.Equals(prethodniKorisnik.korisnickoime))
                            {
                                if (rez.status == StatusRezervacija.Odobrena || rez.status == StatusRezervacija.Kreirana)
                                {
                                    if (rez.isDeleted == false)
                                    {
                                        return Conflict();
                                    }
                                }
                            }
                        }
                    }

                    bool postojilet = false;

                    foreach (Let let in DataSaver.letovi)
                    {
                        if (let.identificator == rezervacija.idleta)
                        {
                            if (let.isDeleted == false)
                            {
                                postojilet = true;
                                letZaRezervaciju = let;

                                if (rezervacija.brojputnika < 1 || rezervacija.brojputnika > let.slobodnihmesta)
                                {
                                    return Conflict();
                                }

                                break;
                            }
                        }
                    }

                    if (!postojilet)
                    {
                        return NotFound();
                    }

                    Rezervacija newRezervacija = new Rezervacija();
                    newRezervacija.brojputnika = rezervacija.brojputnika;
                    newRezervacija.korisnickoime = prethodniKorisnik.korisnickoime;
                    newRezervacija.idleta = rezervacija.idleta;
                    newRezervacija.nazivAviokompanija = letZaRezervaciju.nazivkompanija;
                    newRezervacija.ukupnacena = rezervacija.brojputnika * letZaRezervaciju.cena;
                    newRezervacija.korisnik = prethodniKorisnik;
                    newRezervacija.let = letZaRezervaciju;
                    newRezervacija.isDeleted = false;
                    newRezervacija.status = StatusRezervacija.Kreirana;

                    letZaRezervaciju.slobodnihmesta -= rezervacija.brojputnika;
                    letZaRezervaciju.zauzetihmesta += rezervacija.brojputnika;

                    DataSaver.rezervacije.Add(newRezervacija);
                    DataSaver.saveRezervacije();
                    return Ok();
                }
            }

            return Unauthorized();
        }

        //nije delete nego otkazivanje za user
        [HttpDelete]
        [Route("deleterezervacija")]
        public IHttpActionResult Deleterezervacija(Rezervacija rezervacija)
        {
            Korisnik prethodniKorisnik;
            Rezervacija rezervacijeZaDelete = null;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (rezervacija.idleta == null || rezervacija.idleta.Trim() == "")
                    {
                        return BadRequest();
                    }

                    bool postoji = false;
                    foreach (Rezervacija rez in DataSaver.rezervacije)
                    {
                        if (rez.idleta.Equals(rezervacija.idleta) && rez.korisnik.korisnickoime.Equals(prethodniKorisnik.korisnickoime))
                        {
                            if (rez.status == StatusRezervacija.Kreirana || rez.status == StatusRezervacija.Odobrena)
                            {
                                if (rez.isDeleted == false)
                                {
                                    rezervacijeZaDelete = rez;
                                    postoji = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (postoji)
                    {
                        DateTime trenutnivreme = DateTime.Now;
                        DateTime vremePolaska = rezervacijeZaDelete.let.vremepolaska;

                        var hours = (vremePolaska - trenutnivreme).TotalHours;

                        if (hours < 24)
                        {
                            return Unauthorized();
                        }

                        rezervacijeZaDelete.let.slobodnihmesta += rezervacijeZaDelete.brojputnika;
                        rezervacijeZaDelete.let.zauzetihmesta -= rezervacijeZaDelete.brojputnika;
                        rezervacijeZaDelete.status = StatusRezervacija.Otkazana;
                        DataSaver.saveRezervacije();
                        return Ok();
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(Rezervacija rezervacija)
        {
            Rezervacija rezervacijaZaIzmenu = null;
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }
 
            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (rezervacija.idleta == null || rezervacija.idleta.Trim() == "" ||rezervacija.status==StatusRezervacija.Kreirana||
                        rezervacija.status == StatusRezervacija.Zavrsena || rezervacija.korisnickoime==null||rezervacija.korisnickoime.Trim() =="")
                    {
                        return BadRequest();
                    }

                   foreach(Rezervacija rez in DataSaver.rezervacije)
                    {
                        if (rez.isDeleted == false)
                        {
                            if(rez.idleta.Equals(rezervacija.idleta) && rez.korisnickoime.Equals(rezervacija.korisnickoime)){
                                if (rez.isDeleted == false)
                                {
                                    if (rez.status == StatusRezervacija.Kreirana)
                                    {
                                        rezervacijaZaIzmenu = rez;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (rezervacijaZaIzmenu == null)
                    {
                        return NotFound();
                    }

                    if (rezervacijaZaIzmenu.status != StatusRezervacija.Kreirana )
                    {
                        return Conflict();
                    }

                    if (rezervacija.status == StatusRezervacija.Otkazana)
                    {
                        var hours = (rezervacijaZaIzmenu.let.vremepolaska - DateTime.Now).TotalHours;

                        if (hours < 24)
                        {
                            return Unauthorized();
                        }
                    }

                    rezervacijaZaIzmenu.status = rezervacija.status;

                    if (rezervacija.status == StatusRezervacija.Otkazana)
                    {
                        rezervacijaZaIzmenu.let.slobodnihmesta += rezervacijaZaIzmenu.brojputnika;
                        rezervacijaZaIzmenu.let.zauzetihmesta -= rezervacijaZaIzmenu.brojputnika;
                    }
                    DataSaver.saveRezervacije();
                    return Ok();
                }
            }

            return Unauthorized();
        }
    }
}
