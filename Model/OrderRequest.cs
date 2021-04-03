using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace central_order_book_poc.Model
{
    public class OrderRequest
    {    
        [Required]
        public String OrderJson { get; set; }        
    }
}
