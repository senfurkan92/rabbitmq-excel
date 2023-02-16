using DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WEB.Hubs;

namespace WEB.Areas.Service.Controllers
{
	public class FilesManagementController : ControllerBase
	{
		private readonly AppDbContext _ctx;
		private readonly IHubContext<ExcelHub> _hubContext;

		public FilesManagementController(AppDbContext ctx, IHubContext<ExcelHub> hubContext)
		{
			_ctx= ctx;
			_hubContext = hubContext;
		}

		[HttpPost]
		public IActionResult Upload([FromForm]IFormFile file, [FromForm]int fileId)
		{
			if (file is not { Length: > 0 }) return BadRequest();

			var userFile = _ctx.UserFiles.FirstOrDefault(x => x.Id == fileId);

			using (var fs = new FileStream(userFile.FilePath, FileMode.Create))
			{
				file.CopyTo(fs);
				fs.Close();
				fs.Dispose();
			}

			userFile.CreatedDate = DateTime.Now;
			userFile.FileStatus = DAL.Entities.FileStatus.Existed;

			_ctx.UserFiles.Update(userFile);
			_ctx.SaveChanges();

			_hubContext.Clients.User(userFile.UserId.ToString()).SendAsync("CompletedFile").Wait();

			return Ok();

		}

		[HttpGet]
		public IActionResult Upload()
		{
			return Ok();

		}
	}
}
