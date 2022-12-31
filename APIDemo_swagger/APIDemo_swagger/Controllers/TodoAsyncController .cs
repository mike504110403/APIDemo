using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using APIDemo_swagger.Services;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using System;
using JsonPatchDocument = Microsoft.AspNetCore.JsonPatch.JsonPatchDocument;
using System.Collections.Generic;
using APIDemo_swagger.ModelBinder;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAsyncController : ControllerBase
    {
        private readonly TodoListAsyncService _todoListService;
        public TodoAsyncController(TodoListAsyncService todoListService)
        {
            _todoListService = todoListService;
        }

        // GET api/<TodoController>  
        [HttpGet]
        public async Task<List<TodoListSelectDto>> Get([FromQuery] TodoSelectParameters value)
        {
            return await _todoListService.Selectdb(value);
        }

        // POST api/<TodoController>
        [HttpPost]
        public async Task Post([FromBody] TodoListPostDto value)
        {
            await _todoListService.Postdb(value);
        }
    }
}