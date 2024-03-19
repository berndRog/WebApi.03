using System;
using System.Collections.Generic;
namespace WebApi.Core.DomainModel.Entities; 

public interface IEntity {
   Guid Id { get; set; }
}     
