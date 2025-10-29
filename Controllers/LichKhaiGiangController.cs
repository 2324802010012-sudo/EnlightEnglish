﻿using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class LichKhaiGiangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichKhaiGiangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 🧠 Lấy danh sách khóa học đang mở hoặc đang học
            var danhSach = _context.KhoaHocs
                .Where(k => k.TrangThai == "Đang mở" || k.TrangThai == "Đang học")
                .OrderBy(k => k.NgayBatDau)
                .AsNoTracking()
                .ToList();

            return View(danhSach);
        }

    }
}