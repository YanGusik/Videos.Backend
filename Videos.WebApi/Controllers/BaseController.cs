using Microsoft.AspNetCore.Mvc;

namespace Videos.WebApi.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int UserId { get; } = 2;
    }
}
