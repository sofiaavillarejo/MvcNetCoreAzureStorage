using System.Globalization;
using Azure.Data.Tables;
using MvcNetCoreAzureStorage.Models;

namespace MvcNetCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient tableService)
        {
            this.tableClient = tableService.GetTableClient("clientes");
        }

        public async Task CreateClientAsync(int id, string nombre, string empresa, int salario, int edad)
        {
            Cliente cliente = new Cliente
            {
                IdCliente = id,
                Nombre = nombre,
                Empresa = empresa,
                Salario = salario,
                Edad = edad
            };
            await this.tableClient.AddEntityAsync<Cliente>(cliente);
        }
        //INTERNAMENTE, SE PUEDE BUSCAR CLIENTES POR CUALQUIER CAMPO DE LA TABLA
        //SI VAMOS A REALIZAR UNA BUSQUEDA (POR EJEMPLO, PARA DETAILS), NO SE PUEDE BUSCAR SOLO POR SU ROW KEY -> SE DEBE GENERAR UNA COMBINACION DE ROW KEY Y PARTITION KEY 
        //PARA BUSCAR POR ENTIDAD UNICA
        public async Task<Cliente> FindClientAsync(string partitionKey, string rowKey)
        {
            Cliente cliente = await this.tableClient.GetEntityAsync<Cliente>(partitionKey, rowKey);
            return cliente;
        }

        //PARA ELIMINAR UN ELEMENTO UNICO TAMBIEN NECESITAMOS PK Y RK
        public async Task DeleteClientAsync(string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        //metodo para recuperar todos los clientes
        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            //para buscar, necesitamos un objeto query con un filter
            var query = this.tableClient.QueryAsync<Cliente>(filter: "");//si no indicamos nada en el filter -> devuelve todos
            //debemos extraer todos los datos del query
            await foreach (var item in query)
            {
                clientes.Add(item);
            }
            return clientes;
        }
        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            //tenemos dos tipos de filter -> ambos se usan con query
            // * si realizamos el filter con QueryAsync -> debemos usar una sintaxis y extraer los datos manualmente
            //string filtro = "Campo eq valor"
            //string filtro = "campo eq valor and Campo2 gt valor2";
            //string filtroSalario = "Salario gt 250000 and Salario lt 300000";
            // * si realizamos la consulta con Query -> podemos usar lambda y extraer los datos directametne, pero no es asincrono
            var query = this.tableClient.Query<Cliente>(x => x.Empresa == empresa);
            return query.ToList();
        }
    }
}
