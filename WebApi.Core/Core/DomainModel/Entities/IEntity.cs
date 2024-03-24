using System;
namespace WebApi.Core.DomainModel.Entities; 

public interface IEntity {
   Guid Id { get; init; }
}     
