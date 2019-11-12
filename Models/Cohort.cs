using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesMVC.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string CohortName { get; set; }

        public List<Student> StudentList { get; set; } = new List<Student>();

        public List<Instructor> InstructorList { get; set; } = new List<Instructor>();
    }
}
