using DataModel;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProcessController : ControllerBase
{
    private static List<ProcessStatusEvent> _data = new();


    [HttpGet]
    public ActionResult<IEnumerable<ProcessStatusEvent>> Get()
    {
        return Ok(InMemoryStore.ProcessEvents);
    }

    [HttpPost]
    public IActionResult Post([FromBody] ProcessStatusEvent data)
    {
        {
            if (data == null)
                return BadRequest();

            InMemoryStore.ProcessEvents.Add(data);

            return Ok();
        }
    }
}
