﻿using System.Globalization;
using Budget.Mvc.Models;
using Budget.Mvc.Models.DTOs;
using Budget.Mvc.Models.ViewModels;
using Budget.Mvc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Budget.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBudgetRepository _budgetRepository;

    public HomeController(ILogger<HomeController> logger, IBudgetRepository budgetRepository )
    {
        _logger = logger;
        _budgetRepository = budgetRepository;
    }

    public IActionResult Index(TransactionViewModel? model)
    {
        var startDate = DateTime.Parse("2022-03-04");

        var transactions = _budgetRepository.GetTransactions();
        if (model.SearchParameters == null)
            transactions = transactions.ToList();

        else if ((model.SearchParameters.CategoryId != 0 && model.SearchParameters.StartDate == null))
            transactions = transactions
                .Where(x => x.CategoryId == model.SearchParameters.CategoryId)
                .ToList();

        else if ((model.SearchParameters.CategoryId == 0 && model.SearchParameters.StartDate != null))
            transactions = transactions
                .Where(x => DateTime.Parse(x.Date) >= DateTime.Parse(model.SearchParameters.StartDate) && DateTime.Parse(x.Date) <= DateTime.Parse(model.SearchParameters.EndDate))
                .ToList();

        else if ((model.SearchParameters.CategoryId != 0 && model.SearchParameters.StartDate != null))
            transactions = transactions
                     .Where(x => DateTime.Parse(x.Date) >= DateTime.Parse(model.SearchParameters.StartDate) && DateTime.Parse(x.Date) <= DateTime.Parse(model.SearchParameters.EndDate) && x.CategoryId == model.SearchParameters.CategoryId)
                     .ToList();


        var categories = _budgetRepository.GetCategories();

        var viewModel = new TransactionViewModel
        {
            Transactions = transactions,
            Categories = categories
        };

        return View(viewModel);
    }


    [HttpPost]
    public IActionResult InsertCategory(TransactionViewModel model)
    {
        if (model.Category.Id > 0)
            _budgetRepository.UpdateCategory(model.Category.Name, model.Category.Id);
        else
            _budgetRepository.AddCategory(model.Category.Name);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult InsertTransaction(TransactionViewModel model)
    {
        var transaction = new Transaction
        {
            Id = model.Id,
            Amount = model.Amount,
            Name = model.Name,
            Date = model.Date,
            TransactionType = model.TransactionType,
            CategoryId = model.CategoryId
        };

        if (transaction.Id > 0)
            _budgetRepository.UpdateTransaction(transaction);
        else
            _budgetRepository.AddTransaction(transaction);

        return RedirectToAction("Index");
    }

    public IActionResult DeleteTransaction(int id)
    {
       
       _budgetRepository.DeleteTransaction(id);

        return RedirectToAction("Index");
    }

    public IActionResult DeleteCategory(int id)
    {

        _budgetRepository.DeleteCategory(id);

        return RedirectToAction("Index");
    }
}
