using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookRepository db, ICategoryRepository Db, IWebHostEnvironment webHostEnvironment)
        {
            _bookRepo = db;
            _categoryRepo = Db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var books = _bookRepo.GetAll(includedProperties:"Category").ToList();
            return View(books);
        }

        public IActionResult Upsert(int? id)
        {
            var bookVM = new BookVM
            {
                Book = new Book(),
                CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if(id == 0 || id == null)
            {
                //Create a book
                return View(bookVM);
            }
            else
            {
                //Update a book
                bookVM.Book = _bookRepo.Get(u => u.Id == id);
                return View(bookVM);
            }
        }


        [HttpPost]
        public IActionResult Upsert(BookVM bookVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(bookVM.Book.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, bookVM.Book.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) 
                        { 
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    bookVM.Book.ImageUrl = @"\images\product\" + fileName;
                }
                if (bookVM.Book.Id == 0) 
                {
                    _bookRepo.Add(bookVM.Book);
                }
                else
                {
                    _bookRepo.Update(bookVM.Book);
                }
                _bookRepo.Save();
                TempData["success"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                bookVM.CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(bookVM);
            }
        }


        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var bookFromDb = _bookRepo.Get(x => x.Id == id);
        //    if (bookFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    var bookVM = new BookVM
        //    {
        //        Book = bookFromDb,
        //        CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        })
        //    };

        //    return View(bookVM);
        //}


        //[HttpPost]
        //public IActionResult Edit(BookVM bookVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        bookVM.CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });

        //        return View(bookVM);
        //    }

        //    _bookRepo.Update(bookVM.Book);
        //    _bookRepo.Save();
        //    TempData["success"] = "Book updated successfully";
        //    return RedirectToAction("Index");
        //}


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var bookFromDb = _bookRepo.Get(x => x.Id == id);

            if (bookFromDb == null)
            {
                return NotFound();
            }
            var bookVM = new BookVM
            {
                Book = bookFromDb,
                CategoryName = _categoryRepo.Get(c => c.Id == bookFromDb.CategoryId)?.Name
            };
            return View(bookVM);
        }



        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var bookFromDb = _bookRepo.Get(x => x.Id == id);

            if (bookFromDb == null)
            {
                return NotFound();
            }
            var oldImagePath =  Path.Combine(_webHostEnvironment.WebRootPath,bookFromDb.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _bookRepo.Remove(bookFromDb);
            _bookRepo.Save();
            TempData["success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }


        #region API Calls

        [HttpGet]
        public IActionResult GetAll() 
        {
            var books = _bookRepo.GetAll(includedProperties: "Category").ToList();
            return Json(new {data = books});
        }

        #endregion
    }
}
