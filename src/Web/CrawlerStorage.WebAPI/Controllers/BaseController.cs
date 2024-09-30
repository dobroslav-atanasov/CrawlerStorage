namespace CrawlerStorage.WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
}