using JqueryAjaxDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JqueryAjaxDemo.Controllers
{
    public class EmployeeController : Controller
    {
        JqueryEmpEntities db;
        public EmployeeController()
        {
           db = new JqueryEmpEntities();
        }
        // GET: Employe
        public ActionResult Index()
        {
            ViewBag.Cities = (from obj in db.Cities
                              select new SelectListItem()
                              {
                                  Text = obj.Name,
                                  Value = obj.CityId.ToString()
                              }).ToList();
            return View();
        }

        public JsonResult GetAllEmployee()
        {
            var EmployeeRecord = (from objEmp in db.Employees
                                  join objcity in db.Cities on objEmp.CityId equals objcity.CityId
                                  select new
                                  {
                                      EmployeeId = objEmp.EmployeeId,
                                      FirstName = objEmp.FirstName,
                                      LastName = objEmp.LastName,
                                      Department = objEmp.Department,
                                      JobType = objEmp.JobType,
                                      Salary = objEmp.Salary,
                                      CityId = objEmp.CityId,
                                      Name = objcity.Name
                                  }).ToList();
            return Json(new{data = EmployeeRecord, success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddUpdateEmployee(Employee employee)
        {
            string message = "Data Successfully Updated";
            if (!ModelState.IsValid)
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { success = false, Message = "Some problem in Validation", ErrorList=errorList });
            }
            Employee emp= db.Employees.SingleOrDefault(x =>x.EmployeeId == employee.EmployeeId)?? new Employee();
            emp.EmployeeId = employee.EmployeeId;
            emp.FirstName = employee.FirstName;
            emp.LastName = employee.LastName;
            emp.Department = employee.Department;
            emp.JobType = employee.JobType;
            emp.Salary = employee.Salary;
            emp.CityId = employee.CityId;

            if (employee.EmployeeId == 0)
            {
                message = "Data Successfully Added";
                db.Employees.Add(emp);
            }
            db.SaveChanges();
            return Json(new { success= true, message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditEmployee(int empId)
        {
            return Json(db.Employees.SingleOrDefault(x => x.EmployeeId == empId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteEmployee(int empId)
        {
            Employee emp = db.Employees.SingleOrDefault(x => x.EmployeeId.Equals(empId));
            db.Employees.Remove(emp);
            db.SaveChanges();
            return Json(new { success = true, message = "Data Successfully Deleted" }, JsonRequestBehavior.AllowGet);
        }
    }
}