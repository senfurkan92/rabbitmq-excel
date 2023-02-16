using BLL.QueueServices;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Channels;
using System.Data;
using ClosedXML.Excel;

namespace EXCEL_WS
{
    public class Worker : BackgroundService, IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientSubscriber _subscriber;
        private readonly AppDbContext _ctx;

		public Worker(ILogger<Worker> logger, RabbitMQClientSubscriber subscriber, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _subscriber = subscriber;
            var scope = serviceProvider.CreateScope();
            _ctx = scope.ServiceProvider.GetService<AppDbContext>();
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
            return base.StartAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriber.DirectConsume("excel-products", Consumer_Received);
            return Task.CompletedTask;
        }

		private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
		{
            //Task.Delay(5000);
			var msg = Encoding.UTF8.GetString(@event.Body.ToArray());
			var userFile = JsonConvert.DeserializeObject<UserFile>(msg);
            var products = _ctx.Products.AsNoTracking().Take(10).ToList();
            var table = GetDataTable(products, "Products");

            using (var ms = new MemoryStream())
            { 
                var ds = new DataSet();
                ds.Tables.Add(table);
				var wb = new XLWorkbook();
                wb.Worksheets.Add(ds);
                wb.SaveAs(ms);

                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(ms.ToArray(),0, ms.ToArray().Length), "file", "test.xlsx");
				content.Add(new StringContent(userFile.Id.ToString()), "fileId");

				using (var client = new HttpClient())
                {
                    var resp = client.PostAsync($"https://localhost:44392/Service/FilesManagement/Upload", content).Result;
                    if (resp.IsSuccessStatusCode)
                    {
                        _subscriber._channel.BasicAck(@event.DeliveryTag,false);
                    }
                    else
                    { 
                    
                    }
                }
			}
		}

        private DataTable GetDataTable(List<Product> data, string tableName) 
        { 
            DataTable table= new DataTable { 
                TableName= tableName,
            };

            table.Columns.Add("ProductId", typeof(int));
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("Explanation", typeof(string));
			table.Columns.Add("Pictures", typeof(string));
			table.Columns.Add("Brand", typeof(string));
			table.Columns.Add("Price", typeof(decimal));
			table.Columns.Add("Stock", typeof(int));

            data.ForEach(x => table.Rows.Add(x.Id, x.Name, x.Explanation, x.Pictures, x.Brand, x.Price, x.Stock));

            return table;
		}
	}
}