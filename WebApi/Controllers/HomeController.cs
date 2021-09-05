using Hangfire;
using HangfireWithMongoDb.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HangfireWithMongoDb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHangFireOpRepository hangFireOpRepository;

        public HomeController(IHangFireOpRepository hangFireOpRepository)
        {
            this.hangFireOpRepository = hangFireOpRepository;
        }

        [HttpPost("{message}")]
        public IActionResult Post(string message, int secondsDelay = 1)
        {
            BackgroundJob.Schedule(
            () => hangFireOpRepository.Add(new HangFireOp(message)),
            TimeSpan.FromSeconds(secondsDelay));

            return StatusCode(StatusCodes.Status200OK, "Done");
        }
    }
}
