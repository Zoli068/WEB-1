using Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;

namespace Projekat.Controllers
{
    [RoutePrefix("recenzija")]
    public class RecenzijaController : ApiController
    {
        //dobijemo sve recenzije za admin
        [HttpGet]
        [Route("getall")]
        public IHttpActionResult GetAll()
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
                    List<Recenzija> recenzije=new List<Recenzija>();

                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        foreach(Recenzija rec in DataSaver.recenzije)
                        {
                            if (rec.isDeleted == false)
                            {
                                recenzije.Add(rec);
                            }
                        }

                        return Ok(recenzije);
                    }
                }
            }

            return Unauthorized();
        }

        //da dobijemo sve recenzije za kompaniju
        [HttpGet]
        [Route("getbycompany")]
        public IHttpActionResult Getbycompany(string naziv)
        {
            List<Recenzija> recenzije = new List<Recenzija>();

            if(naziv==null || naziv.Trim() == "")
            {
                return BadRequest();
            }

            foreach (Recenzija rec in DataSaver.recenzije)
            {
                if (rec.nazivkompanija.ToLower().Equals(naziv.ToLower())){

                    if (rec.isDeleted == false)
                    {
                        if (rec.status == StatusReceznija.Odobrena)
                        {
                            recenzije.Add(rec);
                        }
                    }
                }
            }

            return Ok(recenzije);         
        }

        //azuriranje,tj admin da odbije/odobri
        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(Recenzija recenzija)
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
                    
                    if(recenzija.idleta==null || recenzija.recezentusername==null|| recenzija.status==StatusReceznija.Kreirana|| recenzija.idleta.Trim() == "" || recenzija.recezentusername.Trim() == "")
                    {
                        return BadRequest();
                    }

                    if (prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                    {
                        foreach (Recenzija rec in DataSaver.recenzije)
                        {
                            if (rec.idleta.Equals(recenzija.idleta) && rec.recezentusername.Equals(recenzija.recezentusername))
                            {
                                if (rec.isDeleted == false)
                                {
                                    if (rec.status != StatusReceznija.Kreirana)
                                    {
                                        return Conflict();
                                    }

                                    rec.status = recenzija.status;
                                    DataSaver.saveRecenzije();
                                    return Ok();
                                }
                            }
                        }

                        return NotFound();
                    }
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("getrecenzija")]
        public IHttpActionResult Getrecenzija(string korisnickoime,string idleta)
        {
            Korisnik prethodniKorisnik = null;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (korisnickoime == null || korisnickoime.Trim() == "" || idleta==null ||idleta.Trim() == "")
            {
                return BadRequest();
            }

            DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik);

            foreach (Recenzija rec in DataSaver.recenzije)
            {
                if (rec.recezentusername.Equals(korisnickoime) && rec.idleta.Equals(idleta))
                {
                    if (rec.isDeleted == false)
                    {
                        //ako rec kreiran ili odbijena onda ovde samo admin moze da pristupi ali inace i bilo koji korisnik
                        if (rec.status == StatusReceznija.Odbijena || rec.status==StatusReceznija.Kreirana)
                        {
                            if(prethodniKorisnik != null && prethodniKorisnik.tipKorisnika == TipKorisnika.Administator)
                            {
                                return Ok(rec);
                            }
                        }
                        else
                        {
                            return Ok(rec);
                        }
                    }
                }
            }

            return Unauthorized();
        }

        //da korisnik moze da upravlja svojim recenzijama
        [HttpGet]
        [Route("postoji")]
        public IHttpActionResult Postoji(string id)
        {
            Recenzija recenzija = null;
            Let let = null;
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
                    if (id == null || id.Trim() == "")
                    {
                        return BadRequest();
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

                    if (let != null)
                    {
                        if (let.status != StatusLeta.Zavrsen)
                        {
                            return Conflict();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }

                    foreach (Recenzija rec in DataSaver.recenzije)
                    {
                        if (rec.idleta.Equals(id) && rec.recezent.korisnickoime.Equals(prethodniKorisnik.korisnickoime)) {
                            if(rec.isDeleted == false)
                            {
                                recenzija = rec;
                                return Ok(rec);
                            }
                        }
                    }

                    return Ok("Nepostoji");
                }
            }

            return Unauthorized();
        }

        //dodavanje recenziju
        [HttpPost]
        [Route("posalji")]
        public IHttpActionResult Posalji(RecenzijaRequestHelper sadrzaj)
        {
            if (sadrzaj == null)
            {
                return BadRequest();
            }

            var headers = Request.Headers;
            Korisnik prethodniKorisnik;
            string token="";
            Let let = null ;

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (token != null && token != "null" && token != "")
            {
                //provera authentifikaciju
                if (DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    //proveru izgled request-a
                    if(sadrzaj.naslov==null || sadrzaj.naslov.Trim() == ""|| sadrzaj.sadrzaj == null || sadrzaj.sadrzaj.Trim() == ""|| sadrzaj.idleta == null || sadrzaj.idleta.Trim() == "")
                    {
                        return BadRequest();
                    }

                    foreach(Let l in DataSaver.letovi)
                    {
                        if (l.identificator.Equals(sadrzaj.idleta))
                        {
                            if (l.isDeleted == false)
                            {
                                let = l;
                                break;
                            }
                        }
                    }
                    //proveru da li postoji let
                    if (let == null)
                    {
                        return NotFound();
                    }

                    bool postojirezervacija = false;

                    //da li korisnik imao rezervaciju za let
                    foreach(Rezervacija rezervacija in DataSaver.rezervacije)
                    {
                        if(rezervacija.korisnik.korisnickoime.Equals(prethodniKorisnik.korisnickoime) && rezervacija.idleta.Equals(sadrzaj.idleta))
                        {
                            if (rezervacija.status == StatusRezervacija.Zavrsena)
                            {
                                if (rezervacija.isDeleted == false)
                                {
                                    postojirezervacija = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!postojirezervacija) 
                    {
                        return Unauthorized();   
                    }
                    //da li vec napravio recenziju
                    foreach(Recenzija recenzija in DataSaver.recenzije)
                    {
                        if (recenzija.idleta.Equals(sadrzaj.idleta) && recenzija.recezentusername.Equals(prethodniKorisnik.korisnickoime))
                        {
                            if (recenzija.isDeleted == false)
                            {
                                return Conflict();
                            }
                        }
                    }

                    string filepath = "D:\\Fakultet\\III godina\\web prog u inf sis\\Veb Projekat\\Projekat\\Projekat\\Images\\" + sadrzaj.idleta +"_"+prethodniKorisnik.korisnickoime+ ".jpg";

                    if (sadrzaj.slika!=null && sadrzaj.slika != "")
                    {
                        try
                        {
                            File.WriteAllBytes(filepath, Convert.FromBase64String(sadrzaj.slika));
                        }
                        catch (Exception) { }
                    }

                    Recenzija rec = new Recenzija();
                    rec.nazivkompanija = let.nazivkompanija;
                    rec.sadrzaj = sadrzaj.sadrzaj;
                    rec.aviokompanija = let.aviokompanija;
                    rec.status = StatusReceznija.Kreirana;
                    rec.idleta= sadrzaj.idleta;
                    if(sadrzaj.slika!=null && sadrzaj.slika != "")
                    {
                    rec.putanjaslike = sadrzaj.idleta + "_" + prethodniKorisnik.korisnickoime+".jpg";
                    }
                    rec.recezent = prethodniKorisnik;
                    rec.naslov=sadrzaj.naslov;
                    rec.recezentusername = prethodniKorisnik.korisnickoime;
                    rec.isDeleted = false;

                    let.aviokompanija.recenzije.Add(rec);
                    DataSaver.recenzije.Add(rec);
                    DataSaver.saveRecenzije();
                    return Ok();
                }
            }
            return Unauthorized();
        }
    }
}
