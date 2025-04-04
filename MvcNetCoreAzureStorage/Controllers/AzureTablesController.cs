﻿using Microsoft.AspNetCore.Mvc;
using MvcNetCoreAzureStorage.Models;
using MvcNetCoreAzureStorage.Services;

namespace MvcNetCoreAzureStorage.Controllers
{
    public class AzureTablesController : Controller
    {
        private ServiceStorageTables service;

        public AzureTablesController(ServiceStorageTables service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<Cliente> clientes = await this.service.GetClientesAsync();
            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            await this.service.CreateClientAsync(cliente.IdCliente, cliente.Nombre, cliente.Empresa, cliente.Salario, cliente.Edad);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await this.service.DeleteClientAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            Cliente cliente = await this.service.FindClientAsync(partitionKey, rowKey);
            return View(cliente);
        }

        public IActionResult ClientesEmpresa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientesEmpresa(string empresa)
        {
            List<Cliente> clientes = await this.service.GetClientesEmpresaAsync(empresa);
            return View(clientes);
        }
    }
}
