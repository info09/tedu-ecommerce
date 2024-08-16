﻿using System;
using Volo.Abp.Application.Dtos;

namespace TeduEcommerce.Roles
{
    public class RoleInListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
