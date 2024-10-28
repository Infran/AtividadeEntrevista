using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using Microsoft.Ajax.Utilities;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente boCliente = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();



            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!boCliente.ValidaCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF invalido");
                }

                if (boCliente.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado");
                }

                if (model.Beneficiarios is null)
                    model.Beneficiarios = new List<BeneficiarioModel>();

                if (model.Beneficiarios.FindAll(b => !boCliente.ValidaCPF(b.CPF)).Count > 0)
                {
                    Response.StatusCode = 400;
                    return Json("CPF de um beneficiario invalido");
                };

                model.Id = boCliente.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    CPF = model.CPF,
                    Telefone = model.Telefone
                });

                if (model.Beneficiarios.Count > 0)
                {
                    model.Beneficiarios.ForEach((b) =>
                    {
                        if (boBeneficiario.ValidaCPF(b.CPF))
                            boBeneficiario.Incluir(new Beneficiario
                            {
                                CPF = b.CPF,
                                Nome = b.Nome,
                                IdCliente = model.Id
                            });
                    });
                }



                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente boCliente = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!boCliente.ValidaCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF invalido");
                }

                if (model.Beneficiarios is null)
                    model.Beneficiarios = new List<BeneficiarioModel>();

                if (model.Beneficiarios.FindAll(b => !boCliente.ValidaCPF(b.CPF)).Count > 0)
                {
                    Response.StatusCode = 400;
                    return Json("CPF de um beneficiario invalido");
                };

                boCliente.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    CPF = model.CPF,
                    Telefone = model.Telefone
                });


                if (model.Beneficiarios.Count > 0)
                {
                    var validos = model.Beneficiarios.Where((b) => boCliente.ValidaCPF(b.CPF)).ToList();

                    var original = boBeneficiario
                        .Listar(model.Id)
                        .Select(b => new BeneficiarioModel
                            {
                                Id = b.Id,
                                CPF = b.CPF,
                                Nome = b.Nome,
                                IdCliente = b.IdCliente,
                            }   
                        )
                        .ToList();


                    var alterar = validos.Where((b) => b.Id != 0).ToList();
                    var incluir = validos.Where((b) => b.Id == 0).ToList();
                    
                    var idsAlterados = alterar.Select((b) => b.Id).ToList();
                    
                    var excluir = original.Where((b) => !idsAlterados.Contains(b.Id)).ToList();

                    alterar.ForEach((b) =>
                    {
                        if (boBeneficiario.ValidaCPF(b.CPF))
                            boBeneficiario.Alterar(new Beneficiario
                            {
                                Id = b.Id,
                                CPF = b.CPF,
                                Nome = b.Nome,
                                IdCliente = b.IdCliente
                            });
                    });

                    incluir.ForEach((b) =>
                    {
                        if (boBeneficiario.ValidaCPF(b.CPF))
                            boBeneficiario.Incluir(new Beneficiario
                            {
                                CPF = b.CPF,
                                Nome = b.Nome,
                                IdCliente = model.Id
                            });
                    });

                    excluir.ForEach((b) =>
                    {
                        if (boBeneficiario.ValidaCPF(b.CPF))
                            boBeneficiario.Excluir(b.Id);
                    });
                }

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    CPF = cliente.CPF,
                    Telefone = cliente.Telefone,
                    Beneficiarios = new BoBeneficiario()
                        .Listar(cliente.Id)
                        .Select(x => new BeneficiarioModel
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            CPF = x.CPF,
                            IdCliente = x.IdCliente
                        })
                        .ToList()
                };


            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}