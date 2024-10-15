using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Projekat.Controllers
{
    [RoutePrefix("letovi")]
    public class LetoviController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public List<Let> Get()
        {
            List<Let> returnLetovi = new List<Let>();
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
                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        foreach (Let let in DataSaver.letovi)
                        {
                            if (let.isDeleted == false)
                            {
                                returnLetovi.Add(let);
                            }
                        }
                    }
                }
            }

            return returnLetovi;
        }

        [HttpGet]
        [Route("get")]
        public List<Let> Get(int status)
        {
            List<Let> returnLetovi = new List<Let>();
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (token != null && token != "null" && token != "")
            {
                if (!DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (status != 0)
                    {
                        return null;
                    }
                }
            }

            foreach (Let let in DataSaver.letovi)
            {
                if (((int)let.status) == status)
                {
                    if (let.isDeleted == false)
                    {
                        returnLetovi.Add(let);
                    }
                }
            }

            return returnLetovi;
        }

        [HttpGet]
        [Route("getbyid")]
        public Let Getbyid(string id)
        {
            Let let = null;

            if (id == null || id == "")
            {
                return null;
            }

            foreach (Let l in DataSaver.letovi)
            {
                if (l.identificator.Equals(id))
                {
                    if (l.isDeleted == false)
                    {
                        let = l;
                        break;
                    }
                }
            }

            return let;
        }

        [HttpGet]
        [Route("userletovi")]
        public IHttpActionResult Userletovi()
        {
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            List<Let> responseLetovi = new List<Let>();

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
                                responseLetovi.Add(rezervacija.let);
                            }
                        }
                    }

                    return Ok(responseLetovi);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult Add(Let let)
        {
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
                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        if (let.nazivkompanija == null || let.nazivkompanija.Trim() == "" || let.cena <= 0 || let.slobodnihmesta <= 0 || let.odredisnadestinacija == null || let.odredisnadestinacija.Trim() == ""
                            || let.polaznadestinacija == null || let.polaznadestinacija.Trim() == "" || let.polaznadestinacija.Trim().ToLower().Equals(let.odredisnadestinacija.Trim().ToLower()) || let.vremedolaska == null
                            || let.vremepolaska == null || let.vremepolaska >= let.vremedolaska || let.vremepolaska < DateTime.Now)
                        {
                            return BadRequest();
                        }

                        Aviokompanija aviokompanija = null;

                        foreach (Aviokompanija avio in DataSaver.aviokompanije)
                        {
                            if (avio.naziv.ToLower().Equals(let.nazivkompanija.ToLower()))
                            {
                                if (avio.isDeleted == false)
                                {
                                    aviokompanija = avio;
                                    break;
                                }
                            }
                        }

                        if (aviokompanija == null)
                        {
                            return NotFound();
                        }

                        Let letZaDodavanje = new Let();
                        letZaDodavanje.cena = let.cena;
                        letZaDodavanje.nazivkompanija = aviokompanija.naziv;
                        letZaDodavanje.aviokompanija = aviokompanija;
                        letZaDodavanje.odredisnadestinacija = let.odredisnadestinacija;
                        letZaDodavanje.polaznadestinacija = let.polaznadestinacija;
                        letZaDodavanje.identificator = Guid.NewGuid().ToString();
                        letZaDodavanje.slobodnihmesta = let.slobodnihmesta;
                        letZaDodavanje.zauzetihmesta = 0;
                        letZaDodavanje.status = StatusLeta.Aktivan;
                        letZaDodavanje.vremepolaska = let.vremepolaska;
                        letZaDodavanje.vremedolaska = let.vremedolaska;
                        letZaDodavanje.isDeleted = false;

                        aviokompanija.letovi.Add(letZaDodavanje);
                        DataSaver.letovi.Add(letZaDodavanje);
                        DataSaver.saveLetovi();
                        return Ok();
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(Let let)
        {
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
                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        if (let.nazivkompanija == null || let.nazivkompanija.Trim() == "" || let.cena <= 0 || let.slobodnihmesta < 0 || let.odredisnadestinacija == null || let.odredisnadestinacija.Trim() == ""
                            || let.polaznadestinacija == null || let.polaznadestinacija.Trim() == "" || let.polaznadestinacija.Trim().ToLower().Equals(let.odredisnadestinacija.Trim().ToLower()) || let.vremedolaska == null
                            || let.vremepolaska == null || let.vremepolaska >= let.vremedolaska || let.identificator == null || let.identificator.Trim() == "")
                        {
                            return BadRequest();
                        }
                    }

                    Let letZaIzmenu = null;

                    foreach (Let l in DataSaver.letovi)
                    {
                        if (l.identificator.Equals(let.identificator))
                        {
                            if (l.isDeleted == false)
                            {
                                letZaIzmenu = l;
                                break;
                            }
                        }
                    }

                    if (letZaIzmenu == null)
                    {
                        return NotFound();
                    }

                    Aviokompanija novKompanijaZaLet = null;

                    if (!letZaIzmenu.nazivkompanija.ToLower().Equals(let.nazivkompanija.ToLower()))
                    {
                        foreach (Aviokompanija aviokompanija in DataSaver.aviokompanije)
                        {
                            if (aviokompanija.naziv.ToLower().Equals(let.nazivkompanija.ToLower()))
                            {
                                if (aviokompanija.isDeleted == false)
                                {
                                    novKompanijaZaLet = aviokompanija;
                                    break;
                                }
                            }
                        }

                        if (novKompanijaZaLet == null)
                        {
                            return NotFound();
                        }
                    }

                    if (let.status != letZaIzmenu.status)
                    {
                       if(let.status==StatusLeta.Otkazan && letZaIzmenu.status == StatusLeta.Aktivan)
                        {

                        }
                        else
                        {
                            return Conflict();
                        }
                    }

                    if (letZaIzmenu.zauzetihmesta == 0)
                    {
                        if (let.slobodnihmesta == 0)
                        {
                            return BadRequest();
                        }
                    }

                    if (let.status == StatusLeta.Aktivan)
                    {
                        if (let.vremepolaska < DateTime.Now)
                        {
                            return BadRequest();
                        }
                    }
                    else if (let.status == StatusLeta.Zavrsen)
                    {
                        if (let.vremepolaska > DateTime.Now)
                        {
                            return BadRequest();
                        }
                    }

                    if (let.status != letZaIzmenu.status)
                    {
                        letZaIzmenu.status = let.status;

                        foreach (Rezervacija rez in DataSaver.rezervacije)
                        {
                            if (rez.idleta.Equals(letZaIzmenu.identificator))
                            {
                                if (rez.isDeleted == false)
                                {
                                    rez.status = StatusRezervacija.Otkazana;
                                }
                            }
                        }
                    }

                    letZaIzmenu.slobodnihmesta = let.slobodnihmesta;
                    letZaIzmenu.vremepolaska = let.vremepolaska;
                    letZaIzmenu.vremedolaska = let.vremedolaska;

                    if (letZaIzmenu.zauzetihmesta == 0)
                    {
                        letZaIzmenu.cena = let.cena;
                    }

                    if (novKompanijaZaLet != null)
                    {
                        letZaIzmenu.aviokompanija.letovi.Remove(letZaIzmenu);

                        List<Recenzija> tempList = new List<Recenzija>();

                        foreach (Recenzija rec in letZaIzmenu.aviokompanija.recenzije)
                        {
                            if (rec.idleta.Equals(let.identificator))
                            {
                                if (rec.isDeleted == false)
                                {
                                    rec.nazivkompanija = novKompanijaZaLet.naziv;
                                    rec.aviokompanija = novKompanijaZaLet;
                                    tempList.Add(rec);
                                }
                            }
                        }

                        foreach (Recenzija rec in tempList)
                        {
                            letZaIzmenu.aviokompanija.recenzije.Remove(rec);
                            novKompanijaZaLet.recenzije.Add(rec);
                        }

                        foreach (Rezervacija rez in DataSaver.rezervacije)
                        {
                            if (let.identificator.Equals(rez.idleta))
                            {
                                if (rez.isDeleted == false)
                                {
                                    rez.nazivAviokompanija = novKompanijaZaLet.naziv;
                                }
                            }
                        }

                        letZaIzmenu.aviokompanija = novKompanijaZaLet;
                        letZaIzmenu.nazivkompanija = novKompanijaZaLet.naziv;
                    }

                    DataSaver.saveLetovi();
                    return Ok();
                }
            }
            return Unauthorized();
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(Let let)
        {
            Korisnik prethodniKorisnik;
            var headers = Request.Headers;
            string token = "";
            Let letZaDelete = null;

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }


            if (token != null && token != "null" && token != "")
            {
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        if (let.identificator == null || let.identificator == "")
                        {
                            return BadRequest();
                        }

                        foreach(Let l in DataSaver.letovi)
                        {
                            if (l.isDeleted == false)
                            {
                                if (l.identificator.Equals(let.identificator))
                                {
                                    letZaDelete = l;
                                    break;
                                }
                            }
                        }

                        if (letZaDelete == null)
                        {
                            return NotFound();
                        }

                        if (letZaDelete.status == StatusLeta.Aktivan)
                        {
                            if (letZaDelete.zauzetihmesta != 0)
                            {
                                return Conflict();
                            }
                        }

                        foreach(Rezervacija rez in DataSaver.rezervacije)
                        {
                            if (rez.isDeleted == false)
                            {
                                if (rez.idleta.Equals(let.identificator))
                                {
                                    rez.isDeleted = true;
                                }
                            }
                        }

                        //ako let obrisan i ako rec mora da bude takodje obrisan onda ovako, ako ne onda preskocimo ovaj deo
                        //foreach(Recenzija rec in DataSaver.recenzije)
                        //{
                        //    if (rec.isDeleted == false)
                        //    {
                        //        if (rec.idleta.Equals(let.identificator))
                        //        {
                        //            rec.isDeleted = true;
                        //        }
                        //    }
                        //}


                        letZaDelete.isDeleted = true;
                        DataSaver.saveLetovi();

                        return Ok();
                     
                    }
                }
            }

            return Unauthorized();
        }
    }
}
