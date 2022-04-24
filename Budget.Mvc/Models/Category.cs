﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Budget.Mvc.Models
{
	public class Category
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="Serio?")]
		public string Name { get; set; }
	}
}

