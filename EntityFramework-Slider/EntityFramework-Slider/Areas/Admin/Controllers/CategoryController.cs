using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {   
        private readonly ICategoryService _categoryService;

        private readonly AppDbContext _context;
        public CategoryController(ICategoryService categoryService,
                                  AppDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetAll());
        }
        [HttpGet]
        public IActionResult Create()    /*async-elemirik cunku data gelmir databazadan*/
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)    /*async-elemirik cunku data gelmir databazadan*/
        {
            try  //databazada problem olarsa cathca girsin
            { 
                if (!ModelState.IsValid)  //inputdan gelen data valid deyilse (yani bosdursa) yeniden viewa qayit.yeniden istesin yazmaqi
                {
                    return View();
                }

                var existData = await _context.Categories.FirstOrDefaultAsync(m => m.Name.Trim().ToLower() == category.Name.Trim().ToLower());
                if (existData is not null)  //databazada bu adda data varsa create eleme
                {
                    ModelState.AddModelError("Name", "This data already exist");  //ModelState.AddModelError-error text gonderik viewa(orada asp-validation-for var gebul edir errorlar)
                    return View();
                }
                await _context.Categories.AddAsync(category);  //validdisa(dolu) gelen Category tipden categorini save ele Databazadaya
                await _context.SaveChangesAsync();  //ve yaddasda saxla yeni dataynan
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error",new { msj = "hi there!"});  //basqa actionun  parametrina data gonderik
            }

          
        }
        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;  //oz yazdqimiz texti gonderik error Viewa
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();  //eyer Id(null) gelmirse exseption cixsin

            Category category = await _context.Categories.FindAsync(id);  //eyni id-li datani tap

            if (category is null) return NotFound();  //eyer bucur Id yoxdursa exseption cixsin

             _context.Categories.Remove(category);   //ve silinmis vezyetde yaddasda saxla

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));   //qayit ana seyfeye
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //ACTION SILMEK UCUN YOX SOFDELETE TRUE ETMEK UCUN(DATABAZADA QALSIN)
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest();  //eyer Id(null) gelmirse exseption cixsin

            Category category = await _context.Categories.FindAsync(id);  //eyni id-li datani tap

            if (category is null) return NotFound();  //eyer bucur Id yoxdursa exseption cixsin

            category.SoftDelete = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));   //qayit ana seyfeye

        }


        [HttpGet]  //birinci get edirik edit etdiyimiz gorek deye(UI-da datalari cixsin)
        public async Task<IActionResult> Edit(int? id)  //id gelecek(id-ye gore tapsin ) ve Viewa aparsin
        {
            if (id is null) return BadRequest();  

            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();

             return View(category); //viewa o id-li datani gonderik gorsetmek ucun

        }
       
        
        
        
        
        [HttpPost] //indi Datani Edit edirik
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Category category)  //id gelecek(id-ye gore tapsin )
        {
            if (id is null) return BadRequest();  //eyer Id(null) gelmirse exseption cixsin

            if (!ModelState.IsValid)  
            {
                return View();
            }

            Category dbCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id); //eyni id-li datani tap

            //AsNoTracking()--yalniz update edende lazim olur bir nov maniyeni aradan qaldirmaq uucn yazriq .Databazaynan elaqe kesmek ucun,cunku asagida yene miraciyet etmeliyik databazaya Update yazanda.databaza cavab versin deye mesgul olmasin

            if (dbCategory is null) return NotFound();  //eyer bucur Id yoxdursa exseption cixsin

            if(dbCategory.Name.Trim().ToLower() == category.Name.Trim().ToLower()) //eyer databazada deyismek istediyin Adda data varsa hecne elemesin
            {
                return RedirectToAction("Index");
            }
            //dbCategory.Name = category.Name;   //tapdiqmiz name-(data) = gelen name-(data) beraberlesdirik(asign)

            _context.Categories.Update(category);  //herdefe tek tek asign elememek ucun gelen datanin icinde olanlari

            await _context.SaveChangesAsync();  //save edirik

            return RedirectToAction("Index");

        }



        [HttpGet]  //birinci get edirik edit etdiyimiz gorek deye(UI-da datalari cixsin)
        public async Task<IActionResult> Detail(int? id)  //id gelecek(id-ye gore tapsin ) ve Viewa aparsin
        {
            if (id is null) return BadRequest();

            Category category = await _context.Categories.FindAsync(id);

            if (category is null) return NotFound();

            return View(category); //viewa o id-li datani gonderik gorsetmek ucun

        }
    }
}
