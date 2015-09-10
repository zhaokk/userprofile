using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class REFEREEqualViewModel
    {
        public REFEREEqualViewModel()
        {

            this.srqvm = new selectRefQuliViewModel();

        }
        public REFEREEqualViewModel(Raoconnection db)
        {

            this.srqvm = new selectRefQuliViewModel(db);

        }


        public REFEREE re { get; set; }

        public selectRefQuliViewModel srqvm { get; set; }



    }
    public class selectRefQuliViewModel
    {
        public selectRefQuliViewModel()
        {


        }
        public selectRefQuliViewModel(Raoconnection db)
        {


            this.quals = new List<SelectQualEditorViewModel>();
            foreach (var qual in db.QUALIFICATIONS)
            {
                var qvm = new SelectQualEditorViewModel(qual);
                this.quals.Add(qvm);
            }

        }


        public selectRefQuliViewModel(REFEREE refe, Raoconnection db)
        {
            this.quals = new List<SelectQualEditorViewModel>();
            var allquals = db.QUALIFICATIONS;
            foreach (var qual in allquals)
            {
                var qvm = new SelectQualEditorViewModel(qual);
                this.quals.Add(qvm);
            }
            foreach (var qual in refe.USERQUALs)
            {
               // var checkqual = this.quals.Find(q => q.qualName == qual.name);
               // checkqual.Selected = true;


            }


        }

        public List<SelectQualEditorViewModel> quals { get; set; }



    }
    public class selectRefQuliEditViewModel
    {
        public selectRefQuliEditViewModel()
        {


        }
        public selectRefQuliEditViewModel(Raoconnection db)
        {


            this.quals = new List<SelectQualEditorViewModel>();
            foreach (var qual in db.QUALIFICATIONS)
            {
                var qvm = new SelectQualEditorViewModel(qual);
                this.quals.Add(qvm);
            }

        }


        public selectRefQuliEditViewModel(REFEREE refe, Raoconnection db)
        {
            this.refeid = refe.refId;
            this.quals = new List<SelectQualEditorViewModel>();
            var allquals = db.QUALIFICATIONS;
            foreach (var qual in allquals)
            {
                var qvm = new SelectQualEditorViewModel(qual);
                this.quals.Add(qvm);
            }
            /*foreach (var qual in refe.QUALIFICATIONS)
            {
                var checkqual = this.quals.Find(q => q.qualName == qual.name);
                checkqual.Selected = true;


            }*/


        }

        public List<SelectQualEditorViewModel> quals { get; set; }

        public int refeid { get; set; }

    }
    public class SelectQualEditorViewModel
    {
        public SelectQualEditorViewModel()
        {

        }
        public SelectQualEditorViewModel(QUALIFICATION qual)
        {
            
            this.qualName = qual.name;
        }
        public bool Selected { get; set; }

     
        
        [Required]
        public String qualName { get; set; }

    }
    //public class SelectQualEditorViewModel
    //{
    //    public SelectQualEditorViewModel()
    //    {

    //    }
    //    public SelectQualEditorViewModel(QUALIFICATION qual)
    //    {
    //        this.qual = qual;
    //        this.qualName = qual.name;
    //    }
    //    public bool Selected { get; set; }

    //    [Required]
    //    public QUALIFICATION qual { get; set; }
    //    [Required]
    //    public String qualName { get; set; }

    //}
}