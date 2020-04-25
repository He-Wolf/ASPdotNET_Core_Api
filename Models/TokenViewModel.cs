using System;

namespace TodoApi.Models
{
    public class TokenViewModel
    {
        public string token { get; set; }
        public string message { get; set; }
        public DateTime currentDate { get; set; }
        public TokenViewModel(string token, string message, DateTime datcurrentDatee)
        {
            this.token = token;
            this.message = message;
            this.currentDate = currentDate;
        }
    }
}