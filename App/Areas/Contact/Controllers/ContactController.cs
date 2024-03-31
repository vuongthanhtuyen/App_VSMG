﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using ContactModel = App.Models.Contacts.Contact;
using Microsoft.AspNetCore.Authorization;
using App.Data;


namespace App.Areas.Contact.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Contact")]
    [Route("/Contact/[action]")]

    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        [TempData]
        public string StatusMessage { set; get; }


        public ContactController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }


		[HttpGet("/contact/")]
		public IActionResult SendContact()
		{

			return View();
		}

		[HttpPost("/contact")]
		public async Task<IActionResult> SendContact([Bind("Message, FullName, Address, Phone")] ContactModel contact)
		{
			if (ModelState.IsValid)
			{
				contact.DateSent = DateTime.Now;
				_context.Add(contact);
				await _context.SaveChangesAsync();

				StatusMessage = "Liên hệ của bạn đã được gửi";
				return RedirectToAction("Index", "Home");
			}
			return View(contact);
		}

        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FirstOrDefaultAsync(m=>m.Id==id);
            if (contact == null)
            {
                return NotFound();

            }
            return View(contact) ;
        }



        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        




    }
}
