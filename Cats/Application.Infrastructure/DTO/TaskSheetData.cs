using System;

namespace Application.Infrastructure.DTO
{
    public class TaskSheetData
    {
        public int DiplomProjectId { get; set; }

        public string InputData { get; set; }

        public string RpzContent { get; set; }

        public string DrawMaterials { get; set; }

        public string Consultants { get; set; }

        public string Faculty { get; set; }

        public string HeadCathedra { get; set; }

        public string Univer { get; set; }

        public DateTime? DateEnd { get; set; }

        public string DateEndString => DateEnd?.ToString("dd-MM-yyyy");

        public DateTime? DateStart { get; set; }

        public string DateStartString => DateStart?.ToString("dd-MM-yyyy");
    }
}