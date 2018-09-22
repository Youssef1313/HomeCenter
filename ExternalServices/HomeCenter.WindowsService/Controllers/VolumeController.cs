﻿using Microsoft.AspNetCore.Mvc;
using HomeCenter.Model.Adapters.Pc;
using HomeCenter.WindowsService.Core;

namespace HomeCenter.WindowsService.Controllers
{
    [Route("api/[controller]")]
    public class VolumeController : Controller
    {
        private readonly IAudioService _audioService;

        public VolumeController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpGet]
        public float Get()
        {
            return _audioService.GetMasterVolume();
        }

        [HttpPost]
        public IActionResult Post([FromBody] VolumePost volume)
        {
            _audioService.SetMasterVolume((float)volume.Volume);
            return Ok();
        }
    }
}
