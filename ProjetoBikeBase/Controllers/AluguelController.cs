﻿using MySql.Data.MySqlClient;
using ProjetoBikeBase.Models.DAO;
using ProjetoBikeBase.Models.DLL;
using ProjetoBikeBase.Models.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProjetoBikeBase.Controllers
{
    public class AluguelController : Controller
    {
        AluguelDAO aluguelDAO = new AluguelDAO();
        AluguelDTO aluguelDTO = new AluguelDTO();
        AluguelBuscaDLL aluguelBuscaDLL = new AluguelBuscaDLL();
        AluguelBuscaDTO aluguelBuscaDTO = new AluguelBuscaDTO(); 

        //CARREGAR CLIENTES
        public void carregarClientes()
        {
            List<SelectListItem> cliente = new List<SelectListItem>();

            using (MySqlConnection con = new MySqlConnection("server=localhost;port=3307;user id=root;password=361190;database=BdBikeCity"))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("call SelecionarCliente();", con);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cliente.Add(new SelectListItem
                    {
                        Text = rdr[1].ToString(),
                        Value = rdr[0].ToString()
                    });
                }
                con.Close();

            }

            ViewBag.Cli = new SelectList(cliente, "Value", "Text");
        }

        //CARREGAR Produto
        public void carregarProdutos()
        {
            List<SelectListItem> produto = new List<SelectListItem>();

            using (MySqlConnection con = new MySqlConnection("server=localhost;port=3307;user id=root;password=361190;database=BdBikeCity"))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("call SelecionarProduto();", con);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    produto.Add(new SelectListItem
                    {
                        Text = rdr[1].ToString(),
                        Value = rdr[0].ToString()
                    });
                }
                con.Close();

            }

            ViewBag.Prod = new SelectList(produto, "Value", "Text");
        }
        public ActionResult CadastoAluguel()
        {
            carregarClientes();
            carregarProdutos();

            return View();
        }
        [HttpPost]
        public ActionResult CadastoAluguel(AluguelDTO aluguelDTO)
        {

            carregarClientes();
            carregarProdutos();
            aluguelDAO.TestarAgenda(aluguelDTO);

            if (aluguelDTO.confAgendamento == "1")
            {
                aluguelDTO.IdCliente = Request["Cli"];
                aluguelDTO.IdProduto = Request["Prod"];
                aluguelDAO.inserirAluguel(aluguelDTO);
                ViewBag.msg = "Agendamento realizado com sucesso";
                return RedirectToAction(nameof(ListarAluguel));
            }
            else
            {
                ViewBag.msg = "Horário indisponível, por favor escolaher outra data/hora";
            }
            return View();
        }

        //Listar Aluguel
        public ActionResult ListarAluguel()
        {
            return View(aluguelBuscaDLL.listaAluguel());
        }
        //ALUGUEL Detalhes
        public ActionResult ALuguelDetalhes(int id)
        {
            return View(aluguelBuscaDLL.listaAluguelDetalhes().Find(aluguelBuscaDTO => aluguelBuscaDTO.IdAluguel == id));
        }
        
        // CANCELAR ALUGUEL
        public ActionResult CancelarAluguel(int id)
        {
            return View(aluguelBuscaDLL.listaAluguel().Find(aluguelBuscaDTO => aluguelBuscaDTO.IdAluguel == id));
        }
        // CANCELAR ALUGUEL
        [HttpPost]
        public ActionResult CancelarAluguel(AluguelBuscaDTO cl)
        {
            aluguelBuscaDLL.CancelarAlugeul(cl);
            return RedirectToAction(nameof(ListarAluguel));
        }

        // GET: Aluguel
        public ActionResult Index()
        {
            return View();
        }
    }
}