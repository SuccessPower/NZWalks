using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepositry;

        public RegionsController (IRegionRepository regionRepositry)
        {
            this.regionRepositry = regionRepositry;
        }

        [HttpGet]
        public IActionResult GetAllRegions()
        {
            var regions = regionRepositry.GetAll();

            var regionsDTO = new List<Models.DTO.Region>();

            regions.ToList().ForEach(region =>
            {
                var regionDTO = new Models.DTO.Region()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name, 
                    Area = region.Area,
                    Lat = region.Lat,
                    Long = region.Long,
                    Population = region.Population                    
                };
                regionsDTO.Add(regionDTO);  
            });

            return Ok(regionsDTO);
        }
    }
}
