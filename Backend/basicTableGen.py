import os

SERVICE_INTERFACE_TEMPLATE = [
	"using Microsoft.AspNetCore.Mvc;",
	"using ComputingEPOS.Common;",
	"",
	"namespace ComputingEPOS.Backend.Services;",
	"",
	"public interface I${1:Plural}Service {",
	"\tpublic Task<ActionResult<List<${2:Single}>>> Get${1:Plural}();",
	"\tpublic Task<ActionResult<${2:Single}>> Get${2:Single}(int id);",
	"\t",
	"\tpublic Task<ActionResult<${2:Single}>> Put${2:Single}(${2:Single} ${3:singleLower});",
	"\t",
	"\tpublic Task<ActionResult<${2:Single}>> Post${2:Single}(${2:Single} ${3:singleLower});",
	"\t",
	"\tpublic Task<IActionResult> Delete${2:Single}(int id);",
	"\t",
	"\tpublic Task<bool> ${2:Single}Exists(int id);",
	"}"
]

SERVICE_TEMPLATE = [
	"using System;",
	"using System.Collections.Generic;",
	"using System.Linq;",
	"using System.Threading.Tasks;",
	"using Microsoft.AspNetCore.Http;",
	"using Microsoft.AspNetCore.Mvc;",
	"using Microsoft.EntityFrameworkCore;",
	"using ComputingEPOS.Common;",
	"using Microsoft.AspNetCore.Mvc.ModelBinding;",
	"",
	"namespace ComputingEPOS.Backend.Services;",
	"",
	"public class ${1:Plural}Service : I${1:Plural}Service {",
	"\tprivate readonly BaseDbContext _context;",
	"",
	"\tpublic ${1:Plural}Service(BaseDbContext context) =>  _context = context;",
	"",
	"\tpublic async Task<ActionResult<List<${2:Single}>>> Get${1:Plural}() =>",
	"\t\tawait _context.${1:Plural}.ToListAsync();",
	"",
	"\tpublic async Task<ActionResult<${2:Single}>> Get${2:Single}(int id) {",
	"\t\t${2:Single}? ${3:singleLower} = await _context.${1:Plural}.FindAsync(id);",
	"\t\treturn ${3:singleLower} != null ? ${3:singleLower} : new NotFoundResult();",
	"\t}",
	"",
	"\tpublic async Task<ActionResult<${2:Single}>> Put${2:Single}(${2:Single} ${3:singleLower}) {",
	"\t\t_context.Entry(${3:singleLower}).State = EntityState.Modified;",
	"",
	"\t\ttry {",
	"\t\t\tawait _context.SaveChangesAsync();",
	"\t\t} catch (DbUpdateConcurrencyException) {",
	"\t\t\tif (!await ${2:Single}Exists(${3:singleLower}.${2:Single}ID)) return new NotFoundResult();",
	"\t\t\telse throw;",
	"\t\t}",
	"",
	"\t\treturn ${3:singleLower};",
	"\t}",
	"",
	"\tpublic async Task<ActionResult<${2:Single}>> Post${2:Single}(${2:Single} ${3:singleLower}) {",
	"\t\t_context.${1:Plural}.Add(${3:singleLower});",
	"\t\tawait _context.SaveChangesAsync();",
	"\t\treturn ${3:singleLower};",
	"\t}",
	"",
	"\tpublic async Task<IActionResult> Delete${2:Single}(int id) {",
	"\t\tActionResult<${2:Single}> ${3:singleLower}Res = await Get${2:Single}(id);",
	"\t\tif (${3:singleLower}Res.Result != null) return ${3:singleLower}Res.Result;",
	"",
	"\t\t_context.${1:Plural}.Remove(${3:singleLower}Res.Value!);",
	"\t\tawait _context.SaveChangesAsync();",
	"",
	"\t\treturn new OkResult();",
	"\t}",
	"",
	"\tpublic async Task<bool> ${2:Single}Exists(int id) => ",
	"\t\tawait _context.${1:Plural}.AnyAsync(e => e.${2:Single}ID == id);",
	"}"
]

CONTROLLER_TEMPLATE = [
	"using System;",
	"using System.Collections.Generic;",
	"using System.Linq;",
	"using System.Threading.Tasks;",
	"using Microsoft.AspNetCore.Http;",
	"using Microsoft.AspNetCore.Mvc;",
	"using Microsoft.EntityFrameworkCore;",
	"using ComputingEPOS.Common;",
	"using ComputingEPOS.Backend.Services;",
	"using System.ComponentModel.DataAnnotations;",
	"",
	"namespace ComputingEPOS.Backend.Controllers;",
	"",
	"[Route(\"api/[controller]\")]",
	"[ApiController]",
	"public class ${1:Plural}Controller : ControllerBase {",
	"\tprivate readonly I${1:Plural}Service m_Service;",
	"\t",
	"\tpublic ${1:Plural}Controller(I${1:Plural}Service service) =>",
	"\t\tm_Service = service;",
	"\t",
	"\t// GET: api/${1:Plural}",
	"\t[HttpGet]",
	"\t[ProducesResponseType(StatusCodes.Status200OK)]",
	"\tpublic async Task<ActionResult<List<${2:Single}>>> Get${1:Plural}() =>",
	"\t\tawait m_Service.Get${1:Plural}();",
	"\t",
	"\t// GET: api/${1:Plural}/5",
	"\t[HttpGet(\"{id}\")]",
	"\t[ProducesResponseType(StatusCodes.Status200OK)]",
	"\t[ProducesResponseType(StatusCodes.Status404NotFound)]",
	"\tpublic async Task<ActionResult<${2:Single}>> Get${2:Single}(int id) =>",
	"\t\tawait m_Service.Get${2:Single}(id);",
	"\t",
	"\t// PUT: api/${1:Plural}/5",
	"\t[HttpPut(\"{id}\")]",
	"\t[ProducesResponseType(StatusCodes.Status200OK)]",
	"\t[ProducesResponseType(StatusCodes.Status404NotFound)]",
	"\t[ProducesResponseType(StatusCodes.Status400BadRequest)]",
	"\tpublic async Task<ActionResult<${2:Single}>> Put${2:Single}(int id, ${2:Single} ${3:singleLower}) {",
	"\t\tif (id != ${3:singleLower}.${2:Single}ID) return BadRequest(); ",
	"\t\treturn await m_Service.Put${2:Single}(${3:singleLower});",
	"\t}",
	"\t",
	"\t// POST: api/${1:Plural}",
	"\t[HttpPost]",
	"\t[ProducesResponseType(StatusCodes.Status201Created)]",
	"\tpublic async Task<ActionResult<${2:Single}>> Post${2:Single}(${2:Single} ${3:singleLower})  {",
	"\t\tActionResult<${2:Single}> new${2:Single}Res = await m_Service.Post${2:Single}(${3:singleLower});",
	"\t\tif (new${2:Single}Res.Result != null) return new${2:Single}Res.Result;",
	"\t\t${2:Single} new${2:Single} = new${2:Single}Res.Value!;",
	"\t\t",
	"\t\treturn CreatedAtAction(nameof(Get${2:Single}), new { id = new${2:Single}.${2:Single}ID }, new${2:Single});",
	"\t}",
	"\t",
	"\t// DELETE: api/${1:Plural}/5",
	"\t[HttpDelete(\"{id}\")]",
	"\t[ProducesResponseType(StatusCodes.Status200OK)]",
	"\t[ProducesResponseType(StatusCodes.Status404NotFound)]",
	"\tpublic async Task<IActionResult> Delete${2:Single}(int id) =>",
	"\t\tawait m_Service.Delete${2:Single}(id);",
	"}"
]

def writeLineToFile(file, line, plural, singular, singular_lower):
	file.write(line.replace("${1:Plural}", plural).replace("${2:Single}", singular).replace("${3:singleLower}", singular_lower) + "\n")

def main():
	abspath = os.path.abspath(__file__)
	dname = os.path.dirname(abspath)
	os.chdir(dname)

	plural = input("Enter the plural of the word: ")
	singular = input("Enter the singular of the word: ")
	singular_lower = singular[0].lower() + singular[1:]

	# Service Interface
	with open(f"Services/Interfaces/I{plural}Service.cs", "w+") as file:
		for line in SERVICE_INTERFACE_TEMPLATE:
			writeLineToFile(file, line, plural, singular, singular_lower)

	# Service
	with open(f"Services/{plural}Service.cs", "w+") as file:
		for line in SERVICE_TEMPLATE:
			writeLineToFile(file, line, plural, singular, singular_lower)

	# Controller
	with open(f"Controllers/{plural}Controller.cs", "w+") as file:
		for line in CONTROLLER_TEMPLATE:
			writeLineToFile(file, line, plural, singular, singular_lower)

if __name__ == '__main__':
	main()