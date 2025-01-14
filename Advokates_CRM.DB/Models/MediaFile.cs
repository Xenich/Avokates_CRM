﻿using System;
using System.Collections.Generic;

namespace Advokates_CRM.DB.Models
{
    public partial class MediaFile
    {
        public Guid Uid { get; set; }
        public int Id { get; set; }
        public string FilePath { get; set; }
        public Guid? NoteUid { get; set; }
        public byte[] NameCripted { get; set; }

        public virtual Note NoteU { get; set; }
    }
}
