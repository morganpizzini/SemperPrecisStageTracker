﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SemperPrecisStageTracker.API.Controllers
{
    /// <summary>
    /// Controller for configuration
    /// </summary>
    [Route("api/[Controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ConfigController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        /// <summary>
        /// Register contact
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpGet]
        [Route("GetConfig")]
        [ProducesResponseType(typeof(object), 200)]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)]
        public IActionResult GetConfig()
        {
            return Ok(
                new
                {
                    recaptcha = new
                    {
                        recaptchaToken = _configuration["recaptchaToken"]
                    }
                });
        }

        /// <summary>
        /// Register contact
        /// </summary>
        /// <returns>Returns action result</returns>
        [HttpGet]
        [Route("GetAllConfig")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetAllConfig()
        {
            if (!_env.IsDevelopment())
                return NotFound();
            return Ok(_configuration.AsEnumerable());
        }
    }
}