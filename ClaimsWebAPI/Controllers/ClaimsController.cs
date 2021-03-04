using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
/*
 Description:
Need a WebApi which returns Claims by Member when passed a Date Param for claims made upto the DateParam

A Member is a person who is enrolled for an Insurance Product. Claim is a result of Health Service which was invoked and is something the Insurance Company pays

Would expect to see a Route which adheres to normal best practices along with a JSON payload which has an array of Member with Claim Details

Attached 2 records each for Member and Claim which you can ingest

Would like to see
1.	A Get REST Endpoint
2.	Model for Member and Claim
3.	Ingest that data from the CSV attached and hydrate/assign the model(You can use CSVHelper nuget package to hydrate the model)
4.	Create a JSON response when the Get REST Invoked which delivers the Member and Claim Details as a ClaimsArray
5.	A Zip of the project excluding the binaries after complete
6.	Needs to be done in .Net 5(or atleast .Net Core 3.1)

End Notes:
•	Feel free to make assumptions and state them. 
•	No additional records are need
•	No error handling is needed.
•	Do not need a UnitTest
•	Feel free to use VS Community Edition
•	Anon Authentication is fine, please do not worry about security
•	Typically, if you have done this before, this can be done in around 2 hours max.
*/
namespace ClaimsWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        // GET: api/<ClaimsController>
        [HttpGet]
        [Route("{claimDate:datetime}")]
        //[Route("{claimDate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        //[Route("date/{*claimDate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]
        public JsonResult GetClaims(DateTime claimDate) //Assuming date format to be MM-DD-YYYY
        {
            //DateTime claimDate = DateTime.Parse("12/22/2020");

            //3.	Ingest that data from the CSV attached and hydrate/assign the model(You can use CSVHelper nuget package to hydrate the model)
            // using CSVHelper
            using (var cReader = new StreamReader("Claim.csv"))
            using (var mReader = new StreamReader("Member.csv"))
            using (var csvReader = new CsvReader(cReader, CultureInfo.InvariantCulture))
            {
                using (var csvReader1 = new CsvReader(mReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    var cRecords = csvReader.GetRecords<Claim>();

                    csvReader1.Read();
                    csvReader1.ReadHeader();
                    var mRecords = csvReader1.GetRecords<Member>();

                    var claim = from c in cRecords
                                join m in mRecords on c.MemberID equals m.MemberID
                                where c.ClaimDate <= claimDate
                                select new { c.MemberID, m.FirstName, m.LastName, c.ClaimDate, c.ClaimAmount, m.EnrollmentDate };
                    //JSON Response with ClaimsArray
                    return new JsonResult(claim.ToArray());
                }
            }
        }

    }
}
