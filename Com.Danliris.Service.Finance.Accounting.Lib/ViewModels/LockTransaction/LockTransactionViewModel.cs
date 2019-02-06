﻿using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.LockTransaction
{
    public class LockTransactionViewModel : BaseViewModel, IValidatableObject
    {
        public DateTimeOffset? LockDate { get; set; }
        public string Description { get; set; }
        public bool? IsActiveStatus { get; set; }
        public string Type { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LockDate == null || LockDate > DateTimeOffset.Now)
                yield return new ValidationResult("Tanggal harus lebih kecil atau sama dengan tanggal hari ini", new List<string> { "LockDate" });

            if (string.IsNullOrWhiteSpace(Type))
                yield return new ValidationResult("Tipe harus diisi", new List<string> { "Type" });
        }
    }
}
