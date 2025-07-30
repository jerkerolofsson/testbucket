using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.TestResources;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Domain.TestResources.Mapping;
public static class TestResourceMapping
{
    /// <summary>
    /// Maps a <see cref="TestResourceDto"/> to a <see cref="TestResource"/>.
    /// </summary>
    /// <param name="dto">The source <see cref="TestResourceDto"/>.</param>
    /// <returns>A new <see cref="TestResource"/>.</returns>
    public static TestResource ToDbo(this TestResourceDto dto)
    {
        return new TestResource
        {
            Name = dto.Name,
            ResourceId = dto.ResourceId,
            Owner = dto.Owner,
            Enabled = true, // Default value
            Locked = false, // Default value
            Types = dto.Types,
            Manufacturer = dto.Manufacturer,
            Model = dto.Model,
            Variables = dto.Variables,
            Health = dto.Health
        };
    }

    /// <summary>
    /// Maps a <see cref="TestResource"/> to a <see cref="TestResourceDto"/>.
    /// </summary>
    /// <param name="entity">The source <see cref="TestResource"/>.</param>
    /// <returns>A new <see cref="TestResourceDto"/>.</returns>
    public static TestResourceDto ToDto(this TestResource entity)
    {
        return new TestResourceDto
        {
            Name = entity.Name,
            ResourceId = entity.ResourceId,
            Owner = entity.Owner,
            Types = entity.Types,
            Manufacturer = entity.Manufacturer,
            Model = entity.Model,
            Variables = entity.Variables,
            Health = entity.Health
        };
    }
}
