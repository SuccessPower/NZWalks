using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper, 
            IRegionRepository regionRepository, IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            // Fetch data from database - domain walks
            var walks = await walkRepository.GetAllAsync();

            //Alternative to using mapper.

            //var walksDTO = new List<Models.Domain.Walk>();
            //walks.ToList().ForEach(walk =>
            //{
            //    var walkDTO = new Models.Domain.Walk()
            //    {
            //        Id= walk.Id,
            //        Name = walk.Name,
            //        Length = walk.Length,
            //        RegionId = walk.RegionId,
            //        WalkDifficultyId = walk.WalkDifficultyId,
            //    };

            //    walksDTO.Add(walkDTO);
            //});


            //Convert domain walks to DTO walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);

            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            // Get walkDomain object from database
            var walkDomain = await walkRepository.GetAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }

            // Convert Domain object to DTO
            var WalkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Return response 
            return Ok(WalkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {
            // Validate Incoming request
            if(!(await ValidateAddWalkAsync(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId

            };

            // Pass domain object to repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            // Convert the domain object back to DTO
            var walkDTO = new Models.Domain.Walk
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };


            // Send DTO response back to Client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);


        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id,
            [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            // Update incoming request
            if(!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId,
                RegionId = updateWalkRequest.RegionId,
            };

            // Pass details to Repository - Get Domain Object in response (or Null)
            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            // Handle Null (not found)
            if (walkDomain == null)
            {
                return NotFound();
            }

            // Convert domain object back to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                WalkDifficultyId = walkDomain.WalkDifficultyId,
                RegionId = walkDomain.RegionId,
            };

            // Return Respone
            return Ok(walkDTO);

        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            //Call Repositroy to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }

            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        }

        // to create a private method type #region then press tab key twice
        #region Private methods
        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            //if(addWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest), 
            //        $"{nameof(addWalkRequest)} cannot be empty.");
            //    return false;
            //}

            //if(string.IsNullOrWhiteSpace(addWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.Name),
            //        $"{nameof(addWalkRequest.Name)} is required");
            //    return false;
            //}

            //if (addWalkRequest.Length <= 0)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.Length),
            //        $"{nameof(addWalkRequest.Length)} has to be greater than zero");
            //    return false;
            //}

            var region = await regionRepository.GetAsync(addWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId),
                    $"{nameof(addWalkRequest.RegionId)} is invalid");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId),
                    $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid");
            }

            if(ModelState.ErrorCount > 0)
            { 
                return false;
            }
            return true;    
        }

        private async Task<bool> ValidateUpdateWalkAsync(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //if (updateWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest),
            //        $"{nameof(updateWalkRequest)} cannot be empty.");
            //    return false;
            //}

            //if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.Name),
            //        $"{nameof(updateWalkRequest.Name)} is required");
            //    return false;
            //}

            //if (updateWalkRequest.Length <= 0)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.Length),
            //        $"{nameof(updateWalkRequest.Length)} has to be greater than zero");
            //    return false;
            //}

            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
                    $"{nameof(updateWalkRequest.RegionId)} is invalid");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId),
                    $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid");
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
