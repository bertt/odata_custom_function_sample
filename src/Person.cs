using System;
using System.ComponentModel.DataAnnotations;

namespace odata_custom_function
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

    }
}
