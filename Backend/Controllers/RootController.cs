using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ComputingEPOS.Common;
using ComputingEPOS.Common.Models;
using ComputingEPOS.Backend.Services;

namespace ComputingEPOS.Backend.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase {
    // GET: api/ping
    [HttpGet("Ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Ping() {
        Console.WriteLine("Ping!");
        return Ok("\"Pong!\"");
    }
}
