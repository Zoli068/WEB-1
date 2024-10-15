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
    [RoutePrefix("aviokompanija")]
    public class AviokompanijaController : ApiController
    {
        [HttpGet]
        [Route("")]
        public List<Aviokompanija> Get()
        {
            var headers = Request.Headers;
            List<Aviokompanija> returnList = new List<Aviokompanija>();

            foreach (Aviokompanija aviokompanija in DataSaver.aviokompanije)
            {
                if (aviokompanija.isDeleted == false)
                {
                    returnList.Add(aviokompanija);
                }
            }

            return returnList;
        }

        [HttpGet]
        [Route("")]
        public Aviokompanija Get(string naziv)
        {
            Aviokompanija aviokompanija = null;

            if (naziv == null || naziv.Trim() == "")
            {
                return null;
            }

            foreach (Aviokompanija av in DataSaver.aviokompanije)
            {
                if (av.naziv.ToLower().Equals(naziv.ToLower()))
                {
                    if (av.isDeleted == false)
                    {
                        aviokompanija = av;
                        break;
                    }
                }
            }
            return aviokompanija;
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult Add(Aviokompanija aviokompanija)
        {
            var headers = Request.Headers;
            Korisnik prethodniKorisnik;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (token != null && token != "null" && token != "")
            {
                //provera authentifikaciju
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    if (prethodniKorisnik.tipKorisnika != TipKorisnika.Administator)
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }

            if (aviokompanija.naziv == null || aviokompanija.naziv.Trim() == ""|| aviokompanija.adresa == null || aviokompanija.adresa.Trim() == ""|| aviokompanija.kontaktinformacije == null || aviokompanija.kontaktinformacije.Trim() == "")
            {
                return BadRequest();
            }

            foreach (Aviokompanija avio in DataSaver.aviokompanije)
            {
                if (avio.isDeleted == false)
                {
                    if (avio.naziv.ToLower().Equals(aviokompanija.naziv.ToLower()))
                    {
                        return Conflict();
                    }
                }
            }

            Aviokompanija novAvio = new Aviokompanija();
            novAvio.naziv = aviokompanija.naziv;
            novAvio.adresa = aviokompanija.adresa;
            novAvio.kontaktinformacije = aviokompanija.adresa;

            DataSaver.aviokompanije.Add(novAvio);
            DataSaver.saveAviokompanije();

            return Ok();
        }

        [HttpPut]
        [Route("izmeniaviokompanija")]
        public IHttpActionResult Izmeniaviokompanija(Aviokompanija aviokompanija)
        {
            Korisnik prethodniKorisnik;
            Aviokompanija aviokompanijaZaIzmenu = null;
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
                    if (prethodniKorisnik.tipKorisnika != TipKorisnika.Administator)
                    {
                        return Unauthorized();
                    }

                    if (aviokompanija.naziv == null || aviokompanija.naziv.Trim() == "" || aviokompanija.kontaktinformacije == null || aviokompanija.kontaktinformacije.Trim() == ""|| aviokompanija.adresa == null || aviokompanija.adresa.Trim() == "")
                    {
                        return BadRequest();
                    }

                    bool postoji = false;

                    foreach (Aviokompanija avio in DataSaver.aviokompanije)
                    {
                        if (avio.naziv.ToLower().Equals(aviokompanija.naziv.ToLower()))
                        {
                            if (avio.isDeleted == false)
                            {
                                postoji = true;
                                aviokompanijaZaIzmenu = avio;
                                break;
                            }
                        }
                    }

                    if (postoji)
                    {
                        aviokompanijaZaIzmenu.adresa = aviokompanija.adresa;
                        aviokompanijaZaIzmenu.kontaktinformacije = aviokompanija.kontaktinformacije;
                        DataSaver.saveAviokompanije();
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            return Unauthorized();
        }

        [HttpDelete]
        [Route("deletekompanija")]
        public IHttpActionResult Deletekompanija(Aviokompanija aviokompanija)
        {
            Korisnik prethodniKorisnik;
            Aviokompanija aviokompanijaZaDelete = null;
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
                    if (prethodniKorisnik.tipKorisnika != TipKorisnika.Administator)
                    {
                        return Unauthorized();
                    }

                    if(aviokompanija.naziv==null || aviokompanija.naziv.Trim() == "")
                    {
                        return BadRequest();
                    }

                    bool postoji = false;

                    foreach(Aviokompanija avio in DataSaver.aviokompanije)
                    {
                        if (avio.naziv.ToLower().Equals(aviokompanija.naziv.ToLower()))
                        {
                            if (avio.isDeleted == false)
                            {
                                postoji = true;
                                aviokompanijaZaDelete = avio;
                                break;
                            }
                        }
                    }

                    if (postoji)
                    {
                        bool canBeDeleted = true;

                        foreach(Let let in aviokompanijaZaDelete.letovi)
                        {
                            if (let.status == StatusLeta.Aktivan)
                            {
                                if (let.isDeleted == false)
                                {
                                    canBeDeleted = false;
                                    break;
                                }
                            }
                        }

                        if (!canBeDeleted)
                        {
                            return Conflict();
                        }

                        foreach(Recenzija rec in aviokompanijaZaDelete.recenzije) 
                        {
                            if (rec.isDeleted == false)
                            {
                                rec.isDeleted = true;
                            }
                        }

                        foreach (Let let in aviokompanijaZaDelete.letovi)
                        {
                            if (let.isDeleted == false)
                            {
                                let.isDeleted = true;
                            }
                        }

                        foreach(Rezervacija rez in DataSaver.rezervacije)
                        {
                            if (rez.nazivAviokompanija.Equals(aviokompanijaZaDelete.naziv))
                            {
                                rez.isDeleted = true;
                            }
                        }

                        aviokompanijaZaDelete.isDeleted = true;
                        DataSaver.saveAviokompanije();
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }

                }
            }
            return Unauthorized();
        }
    }
}