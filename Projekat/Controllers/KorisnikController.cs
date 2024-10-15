using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;

namespace Projekat.Controllers
{
    [RoutePrefix("")]
    public class KorisnikController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(Korisnik korisnik)
        {
            Korisnik logedKorisnik = null;
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();    
            }
            
            if (token!=null && token!="null" && token != "")
            {
                return Unauthorized();
            }

            if(korisnik.korisnickoime==null || korisnik.korisnickoime.Trim() == "" || korisnik.lozinka==null || korisnik.lozinka.Trim() == "")
            {
                return BadRequest("Invalid values for login attempt");
            }

            foreach(Korisnik k in DataSaver.korisnici)
            {
                if(k.korisnickoime.Equals(korisnik.korisnickoime) && k.lozinka.Equals(korisnik.lozinka))
                {
                    logedKorisnik = k;
                    break;
                }
            }

            if (logedKorisnik == null)
            {
                return Unauthorized();
            }

            string authtoken=Guid.NewGuid().ToString();
            DataSaver.logovaniKorisnici.Add(authtoken, logedKorisnik);

            HttpResponseMessage response=new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("AuthToken", authtoken);
            return ResponseMessage(response);
        }        

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(Korisnik korisnik)
        {
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            if (token != null && token != "null" && token != "")
            {
                return Conflict();
            }

            if (korisnik.ime==null || korisnik.korisnickoime==null||korisnik.prezime==null||korisnik.datumrodjenja==null||korisnik.email==null||korisnik.lozinka==null||
                korisnik.ime.Trim()==""   || korisnik.korisnickoime.Trim() == ""  || korisnik.prezime.Trim()==""|| korisnik.email.Trim() == ""||korisnik.lozinka=="")
            {   
                return BadRequest("Invalid values for registration attempt");
            }

            foreach (Korisnik k in DataSaver.korisnici)
            {
                if (k.korisnickoime.ToLower().Equals(korisnik.korisnickoime.ToLower()))
                {
                    return BadRequest("PostojiUsername");
                }
                if (k.email.ToLower().Equals(korisnik.email.ToLower()))
                {
                    return BadRequest("PostojiEmail");
                }
            }

            DateTime datumRodj;

            if (!DateTime.TryParse(korisnik.datumrodjenja, out datumRodj))
            {
                return BadRequest("Invalid Date");
            }

            if (datumRodj >= DateTime.Now)
            {
                return BadRequest("U can't be born in the future");
            }


            DataSaver.korisnici.Add(korisnik);
            DataSaver.saveKorisnici();
            string authtoken = Guid.NewGuid().ToString();
            DataSaver.logovaniKorisnici.Add(authtoken, korisnik);
            
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("AuthToken", authtoken);
            return ResponseMessage(response);
        }

        [HttpGet]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();

                if (token != null && token != "null" && token != "")
                {
                    DataSaver.logovaniKorisnici.Remove(token);
                }
            }

            return Ok();
        }

        [HttpGet]
        [Route("checklogin")]
        public IHttpActionResult Checklogin()
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
                if(DataSaver.logovaniKorisnici.TryGetValue(token,out prethodniKorisnik)){
                    return Ok(prethodniKorisnik);
                }
            }

            return Unauthorized();
        }
       
        [HttpPut]
        [Route("updateprofil")]
        public IHttpActionResult Updateprofil(Korisnik korisnik)
        {
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            Korisnik prethodniKorisnik;

            if (token != null && token != "null" && token != "")
            {
                if (!DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    return Unauthorized();
                }
            }
            else
            {
                //no token/invalid token -> no izmena for you
                return Unauthorized();
            }

            if (korisnik.ime == null || korisnik.korisnickoime == null || korisnik.prezime == null || korisnik.datumrodjenja == null || korisnik.email == null || korisnik.lozinka==null||
                korisnik.ime.Trim() == "" || korisnik.korisnickoime.Trim() == "" || korisnik.prezime.Trim() == "" || korisnik.email.Trim() == ""||korisnik.lozinka.Trim()  == "")
            {
                return BadRequest("Invalid values for update attempt");
            }
            DateTime datumRodj;

            if (!DateTime.TryParse(korisnik.datumrodjenja,out datumRodj))
            {
                return BadRequest();
            }

            if (datumRodj >= DateTime.Now)
            {
                return BadRequest();
            }

            foreach(Korisnik k in DataSaver.korisnici)
            {
                if (k.email.ToLower().Equals(korisnik.email.ToLower())){
                    if (k != prethodniKorisnik)
                    {
                        return Conflict();
                    }
                }
            }

            prethodniKorisnik.email = korisnik.email;
            prethodniKorisnik.datumrodjenja = korisnik.datumrodjenja;
            prethodniKorisnik.ime = korisnik.ime;
            prethodniKorisnik.prezime = korisnik.prezime;
            prethodniKorisnik.lozinka = korisnik.lozinka;
            prethodniKorisnik.pol = korisnik.pol;

            DataSaver.saveKorisnici();
            return Ok("Updated");
        }

        [HttpGet]
        [Route("getallkorisnik")]
        public IHttpActionResult Getallkorisnik()
        {
            var headers = Request.Headers;
            string token = "";

            if (headers.Contains("AuthToken"))
            {
                token = headers.GetValues("AuthToken").First();
            }

            Korisnik prethodniKorisnik;

            if (token != null && token != "null" && token != "")
            {
                if(!DataSaver.logovaniKorisnici.TryGetValue(token, out prethodniKorisnik))
                {
                    return Unauthorized();
                }
            }
            else
            {
                //no token/invalid token -> no list for you
                return Unauthorized();
            }

            if(prethodniKorisnik.tipKorisnika==TipKorisnika.Putnik)
            {
                //korisnik je samo putnik
                return Unauthorized();
            }

            return Ok(DataSaver.korisnici);
        }
    }
}
