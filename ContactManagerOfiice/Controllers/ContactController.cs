using ContactManagerOfiice.DbContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactManagerOfiice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private ContactRepository contactRepository;
        public ContactController()
        {
            contactRepository = new ContactRepository();
            contactRepository.Initilaize().Wait();
        }
        // GET: api/<ContactController>
        [HttpGet]
        public IEnumerable<Contact> Get()
        {
            var result = contactRepository.QueryItemsAsync("select *  from c").Result;
            return result;
        }

        // GET api/<ContactController>/5
        [Route("{id}")]
        [HttpGet]        
        public Contact Get(string id)
        {
            var result = contactRepository.QueryItemsAsync(id).Result;
            return result.FirstOrDefault();           
        }

        // POST api/<ContactController>
        [HttpPost]
        public Contact Post([FromBody] Contact contact)
        {            
            contact.Id = Guid.NewGuid().ToString();
            var result=contactRepository.AddItemsToContainerAsync(contact).Result;

            return result;
        }

        // PUT api/<ContactController>/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Contact contact)
        {
            var response=contactRepository.UpdateContactItemAsync(contact);
        }

        // DELETE api/<ContactController>/5
        [HttpDelete]
        public void Delete(string id, string partitionKeyValue)
        {
           // var result = contactRepository.QueryItemsAsync(id).Result;
            var deletItem = contactRepository.DeleteContactItemAsync(id,partitionKeyValue);
        }
    }
}
