using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repository;
using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepositry, IMapper mapper)
        {
            this.regionRepository = regionRepositry;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegionsAsync()
        {
            var regions = await regionRepository.GetAllAsync();


            //regions.ToList().ForEach(region =>
            //{
            //    var regionDTO = new Models.DTO.Region()
            //    {
            //        Id = region.Id,
            //        Code = region.Code,
            //        Name = region.Name, 
            //        Area = region.Area,
            //        Lat = region.Lat,
            //        Long = region.Long,
            //        Population = region.Population                    
            //    };
            //    regionsDTO.Add(regionDTO);  
            //});

            var regionsDTO = mapper.Map<List<Models.DTO.Region>>(regions);

            return Ok(regionsDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetRegionAsync")]
        public async Task<IActionResult> GetRegionAsync(Guid id)
        {
            var region = await regionRepository.GetAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            var regionDTO = mapper.Map<Models.DTO.Region>(region);
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegionAsync(Models.DTO.AddRegionRequest addRegionRequest)
        {
            //Validate The Request
            //if (!ValidateAddRegionAsync(addRegionRequest))
            //{
            //    return BadRequest(ModelState);
            //}

            //Request(DTO) to Domain Model
            var region = new Models.Domain.Region()
            {
                Code = addRegionRequest.Code,
                Name = addRegionRequest.Name,
                Area = addRegionRequest.Area,
                Lat = addRegionRequest.Lat,
                Long = addRegionRequest.Long,
                Population = addRegionRequest.Population            
            };

            //Pass details to repository
            region = await regionRepository.AddAsync(region);

            //Convert back to DTO
            var regionDTO = new Models.DTO.Region
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                Area = region.Area,
                Lat = region.Lat,
                Long = region.Long,
                Population = region.Population
            };

            return CreatedAtAction(nameof(GetRegionAsync), new {id = regionDTO.Id}, regionDTO);
            
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegionAsync(Guid id)
        {
            //Get region from database
            var region = await regionRepository.DeleteAsync(id);

            //if null return NotFound()
            if (region == null)
            {
                return NotFound();
            }

            //Convert response back to DTO
            var regionDTO = new Models.DTO.Region
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                Area = region.Area,
                Lat = region.Lat,
                Long = region.Long,
                Population = region.Population
            };

            //return Ok response
            return Ok(regionDTO);
            
        }
         
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegionAsync([FromRoute] Guid id, 
            [FromBody] Models.DTO.UpdateRegionRequest updateRegionRequest)
        {
            //Validate The incoming request
            //if (!ValidateUpdateRegionAsync(updateRegionRequest))
            //{
            //    return BadRequest(ModelState);
            //}

            //Convert DTO to Domain model
            var region = new Models.Domain.Region
            {
                Code = updateRegionRequest.Code,
                Area = updateRegionRequest.Area,
                Lat = updateRegionRequest.Lat,
                Long = updateRegionRequest.Long,
                Name = updateRegionRequest.Name,
                Population = updateRegionRequest.Population

            };

            //Update region using repository
            region = await regionRepository.UpdateAsync(id, region);

            //If Null then NotFound
            if (region == null)
            {
                return NotFound();
            }

            //Convert Domain back to DTO

            var regionDTO = new Models.DTO.Region
            {
                Id = region.Id,
                Code = region.Code,
                Area = region.Area,
                Lat = region.Lat,
                Long = region.Long,
                Name = region.Name,
                Population = region.Population
            };

            //Return Ok response
            return Ok(regionDTO);
        }

        #region Private methods
        private bool ValidateAddRegionAsync(Models.DTO.AddRegionRequest addRegionRequest)
        {
            if (addRegionRequest == null)
            {
                ModelState.AddModelError(nameof(addRegionRequest),
                    $"Add Region Data is required.");
                return false;
            }

            if (string.IsNullOrEmpty(addRegionRequest.Code))
            {
                ModelState.AddModelError(nameof(addRegionRequest.Code),
                    $"{nameof(addRegionRequest.Code)} cannot be null or emptly or white space.");
            }

            if (string.IsNullOrEmpty(addRegionRequest.Name))
            {
                ModelState.AddModelError(nameof(addRegionRequest.Name),
                    $"{nameof(addRegionRequest.Name)} cannot be null or emptly or white space.");
            }

            if (addRegionRequest.Area <= 0)
            {
                ModelState.AddModelError(nameof(addRegionRequest.Area),
                    $"{nameof(addRegionRequest.Area)} cannot be less than or equal to zero.");
            }

            if (addRegionRequest.Population < 0)
            {
                ModelState.AddModelError(nameof(addRegionRequest.Population),
                    $"{nameof(addRegionRequest.Population)} cannot be less than zero.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        private bool ValidateUpdateRegionAsync(Models.DTO.UpdateRegionRequest updateRegionRequest)
        {
            if (updateRegionRequest == null)
            {
                ModelState.AddModelError(nameof(updateRegionRequest),
                    $"Add Region Data is required.");
                return false;
            }

            if (string.IsNullOrEmpty(updateRegionRequest.Code))
            {
                ModelState.AddModelError(nameof(updateRegionRequest.Code),
                    $"{nameof(updateRegionRequest.Code)} cannot be null or emptly or white space.");
            }

            if (string.IsNullOrEmpty(updateRegionRequest.Name))
            {
                ModelState.AddModelError(nameof(updateRegionRequest.Name),
                    $"{nameof(updateRegionRequest.Name)} cannot be null or emptly or white space.");
            }

            if (updateRegionRequest.Area <= 0)
            {
                ModelState.AddModelError(nameof(updateRegionRequest.Area),
                    $"{nameof(updateRegionRequest.Area)} cannot be less than or equal to zero.");
            }

            if (updateRegionRequest.Population < 0)
            {
                ModelState.AddModelError(nameof(updateRegionRequest.Population),
                    $"{nameof(updateRegionRequest.Population)} cannot be less than zero.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
