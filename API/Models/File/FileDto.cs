﻿namespace API.Models.File
{
    public class FileDto
    {
        private int? Id { get; set;}
        public SourceDto Source { get; set; }
        public PropertyDto Properties { get; set; }
        public AccessDto Access { get; set; }

        public static FileDto Stub()
        {
            return new FileDto
            {
                Id = 4,
                Source = SourceDto.Stub(),
                Properties = PropertyDto.Stub(),
                Access = AccessDto.Stub(),
            };
        }
    }
}