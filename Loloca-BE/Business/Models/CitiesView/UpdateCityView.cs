﻿namespace Loloca_BE.Business.Models.CitiesView
{
    public class UpdateCityView
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public bool Status { get; set; }
    }
}
