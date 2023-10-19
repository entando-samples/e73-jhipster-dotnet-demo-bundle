using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JhipsterDotNetMS.Dto
{

    public class ConferenceDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        // jhipster-needle-dto-add-field - JHipster will add fields here, do not remove
    }
}
